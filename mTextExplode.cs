using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

[assembly: CommandClass(typeof(explodemtext.Program))]
namespace explodemtext
{
    public class Program
    {
        [CommandMethod("ExplodeMtextObjects")]
        public static void Main()
        {
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;
            var ed = akDoc.Editor;

            using(Transaction akTr = akDb.TransactionManager.StartTransaction())
            {
              BlockTable akBlkTbl;
              akBlkTbl = akTr.GetObject(akDb.BlockTableId, OpenMode.ForRead) as BlockTable;

              BlockTableRecord akBlkTblRec;
              akBlkTblRec = akTr.GetObject(akBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

              //Collect all the exploded objects in a single collection 
              DBObjectCollection objs = new DBObjectCollection();

              foreach(ObjectId akObjId in akBlkTblRec)
              {
                switch(akObjId.ObjectClass.DxfName)
                {
                  case "MTEXT":
                    Entity ent = (Entity)akTr.GetObject(akObjId, OpenMode.ForRead);
                    MText mt = ent as MText;

                    if(mt!=null)
                    {
                      MTextFragmentCallback cb = new MTextFragmentCallback((frag, obj) =>
                      {
                        ed.WriteMessage("\nName: {0}, \nObject ID: {1}",frag.Text, akObjId);
                        
                        return MTextFragmentCallbackStatus.Continue;
                      }
                      );
                      mt.ExplodeFragments(cb);
                    }
                    break;
                }
              }
              akTr.Commit();
            }
        }
    }
}       