using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Windows.Forms; [Doesnt work for VS Code]

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;


[assembly: CommandClass(typeof(FINDTAG.Program))]

/*
    VERSION 4.0.1

    This script prompts the User to input a Tag Number,
    then searches the MODEL SPACE for the corresponding Number

    This version tries to Explode the MText Object 
        Entity.Explode()  or MText.ExplodeFragments() <--

    ROAD BLOCKS:   

        1. Tag Numbers are usually written in as the second line in MTEXT objects in BD
           so might have to explode MTEXT Objects to access the the Tag Numbers individually 
           OR do a wildcard search [FIXED]
            

        2. How to highlight / Notify / Zoom to where the selected Tag is located if it is found?  [zoom kinda works]
            --> NEW PROBLEM: What to do if there are MORE than ONE instance of the Tag? 
        
        3. Return the number of times the Tag Number appears in the drawing. [FIXED]

        4. If there were multiple instances of a Tag, how to show them all on one list?  [semi FIXED]

        
        Isaac Ang, November 2019
 */
namespace FINDTAG
{
        public class Program
    {

        [CommandMethod("FINDTAG")]
        public static void Findtag()
        {
            //Get Current Doc and Db
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;
            var ed = akDoc.Editor;
            //Set up variable to zoom to current view -- used later
            var view = ed.GetCurrentView();

            int akCount = 0;

            //Create instance of listbox
            ListBox listBox1 = new ListBox();

            //Dynamic Array to store values in Text Filter
            List<string> fragName = new List<string>();
            //Dynamic Array to store Location values
            List<double> fragX = new List<double>();
            List<double> fragY = new List<double>();

            //Get the User Input
            PromptStringOptions pStrOpt = new PromptStringOptions("\nEnter the Tag Number:");
            pStrOpt.AllowSpaces = true;
            PromptResult pStrRes = ed.GetString(pStrOpt);

            using(Transaction akTr = akDb.TransactionManager.StartTransaction())
            {
                BlockTable akBlkTbl;
                akBlkTbl = akTr.GetObject(akDb.BlockTableId, OpenMode.ForRead) as BlockTable;    
                BlockTableRecord akBlkTblRec;
                akBlkTblRec = akTr.GetObject(akBlkTbl[BlockTableRecord.ModelSpace],  OpenMode.ForWrite) as BlockTableRecord;

                //Collect all exploded objects in a single collection
                DBObjectCollection objs = new DBObjectCollection();

               foreach(ObjectId akObjId in akBlkTblRec)
               {
                   switch(akObjId.ObjectClass.DxfName)
                   {
                       case "MTEXT":
                        //Explode the MTEXT object, then iterate to compare
                        /*
                            Exploding Code taken from a snippet here:
                            https://www.keanw.com/2011/02/comparing-explode-with-explodefragments-on-autocad-mtext.html
                         */
                        Entity ent = (Entity)akTr.GetObject(akObjId, OpenMode.ForRead);
                        MText mt = ent as MText;
                        if(mt != null)
                            {
                                MTextFragmentCallback cb = new MTextFragmentCallback((frag, obj) =>
                                {
                                    if(frag.Text == pStrRes.StringResult)
                                    {
                                        //Text and location information added to dynamic array 
                                        fragName.Add(frag.Text);
                                        fragX.Add(frag.Location.X);
                                        fragY.Add(frag.Location.Y);
                                        
                                        //Zooms the view to where the points are
                                        view.CenterPoint = new Point2d(frag.Location.X, frag.Location.Y);
                                        ed.SetCurrentView(view);
                                        akCount = akCount + 1;
                                        //kX = frag.Location.X;
                                        //kY = frag.Location.Y;
                                        //ed.WriteMessage($"\nTag number {frag.Text} found at {frag.Location}");
                                        //ed.WriteMessage($"\nTag number {frag.Text} found at X:{kX} and Y:{kY}");

                                        //maybe store Texts and Location in a Dynamic Array so we can print at one shot at the end of iterations
                                        //Application.ShowAlertDialog($"\nTag number {frag.Text} found at X:{kX} and Y:{kY}");
                                    }
                                    return MTextFragmentCallbackStatus.Continue;
                                }
                                );
                                mt.ExplodeFragments(cb);
                            }   
                            break;
                        case "TEXT":
                            var text = (DBText)akTr.GetObject(akObjId, OpenMode.ForRead);
                            //if(text.TextString == pStrRes.ToString())
                            //if(text.TextString == pStrRes.Value.ToString())
                            if(text.TextString == pStrRes.StringResult) 
                                {
                                    ed.WriteMessage($"\nTag number {text.TextString} Found at {text.Position}!");
                                }
                            break;
                        }
                }
                //This prints out all the text stored in the fragName, fragX and fragY Dynamic Array
                ed.WriteMessage("\n{0} , \nX: {1}, \nY: {2}" , String.Join(" | ", fragName), String.Join(" | ",fragX) , String.Join(" | ",fragY));

                ed.WriteMessage($"\nTag appears {akCount} times");
                //Save changes and dispose of the transaction 
                akTr.Commit();
            }
        }   
    
    }
}