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



[assembly: CommandClass(typeof(drawBox.Program))]

/*
    This script draws a 2D box, as well as a 3D box 
    by getting User Input about the Height and Width.

    Program should draw the box from the Bottom Left Corner 
    in the Model Spaces

 */
namespace drawBox
{
    public class Program
    {
        [CommandMethod("DRAW2DBOX")]
        public static void draw2D()
        {
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;
            var ed = akDoc.Editor;
            
            //Get user Input
            PromptIntegerOptions widthOne = new PromptIntegerOptions("\nEnter the Width:");
            PromptIntegerResult pResWidth = ed.GetInteger(widthOne);
            PromptIntegerOptions heightOne = new PromptIntegerOptions("\nEnter the Height: ");
            PromptIntegerResult pResHeight = ed.GetInteger(heightOne);

            double heightBox = pResHeight.Value;
            double widthBox = pResWidth.Value;


            using(Transaction tr = akDb.TransactionManager.StartTransaction())
            {
                BlockTable akBlkTbl;
                akBlkTbl = tr.GetObject(akDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord akBlkTblRec;
                akBlkTblRec = tr.GetObject(akBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // DRAW THE BOX
                //Left Vertical [0,0 -> 0,heightBox]
                using(Line leftVert = new Line(new Point3d(0,0,0), new Point3d(0,heightBox,0)))
                {
                    akBlkTblRec.AppendEntity(leftVert);
                    tr.AddNewlyCreatedDBObject(leftVert,true);
                }
                //Right Vertical [widthBox,0 -> widthBox, heightBox]
                using(Line rightVert = new Line(new Point3d(widthBox,0,0), new Point3d(widthBox,heightBox,0)))
                {
                    akBlkTblRec.AppendEntity(rightVert);
                    tr.AddNewlyCreatedDBObject(rightVert,true);
                }
                //Bottom Horizontal [0,0 -> widthBox,0]
                using(Line bottomHoriz = new Line(new Point3d(0,0,0), new Point3d(widthBox,0,0)))
                {
                    akBlkTblRec.AppendEntity(bottomHoriz);
                    tr.AddNewlyCreatedDBObject(bottomHoriz,true);
                }
                //Top Horizontal [0, heightBox -> widthBox, heightBox]
                using(Line topHoriz = new Line(new Point3d(0, heightBox,0), new Point3d(widthBox,heightBox,0)))
                {
                    akBlkTblRec.AppendEntity(topHoriz);
                    tr.AddNewlyCreatedDBObject(topHoriz,true);
                }
                //Commit the  new Objects to the Database 
                tr.Commit();
            }
        }

        [CommandMethod("TESTBOX")]
        public static void draw3D()
        {
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;
            var ed = akDoc.Editor;
            

            //Get user Input
            PromptIntegerOptions widthOne = new PromptIntegerOptions("\nEnter the Width: ");
            PromptIntegerResult boxWidth = ed.GetInteger(widthOne);
            PromptIntegerOptions heightOne = new PromptIntegerOptions("\nEnter the Height: ");
            PromptIntegerResult boxHeight = ed.GetInteger(heightOne);

            double VH = boxHeight.Value;
            double VW = boxWidth.Value;

            ed.WriteMessage("\nBefore Transaction ... ");
            using(Transaction tr = akDb.TransactionManager.StartTransaction())  
            {
                BlockTable akBlkTbl;
                akBlkTbl = tr.GetObject(akDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord akBlkTblRec;
                akBlkTblRec = tr.GetObject(akBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                ed.WriteMessage($"\n{boxWidth}, {boxHeight} ... ");
                using(Line vert = new Line(new Point3d(0,0,0), new Point3d(0,VH,0)))
                {
                    akBlkTblRec.AppendEntity(vert);
                    tr.AddNewlyCreatedDBObject(vert,true);
                }
                using(Line horiz = new Line(new Point3d(VW,0,0), new Point3d(VW,VH,0)))
                {
                    akBlkTblRec.AppendEntity(horiz);
                    tr.AddNewlyCreatedDBObject(horiz, true);
                }
                tr.Commit();
            }
        }
    }
}