using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

[assembly: CommandClass(typeof(AutoCAD.Program))]
namespace AutoCAD
{
    public class Program
    {
        [CommandMethod("LAYERLIST")]
        public static void Main()
        {
            //get the current document and database
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;
            
            // start a transaction
            using(Transaction akTrans = akDb.TransactionManager.StartTransaction())
            {
                //returns the layer table for the current database
                LayerTable akLyrTbl;
                akLyrTbl = akTrans.GetObject(akDb.LayerTableId, OpenMode.ForRead) as LayerTable;

                //Step through the layer Table and print each layer name
                foreach (ObjectId akObjId in akLyrTbl)
                {
                    LayerTableRecord akLyrTblRec;
                    akLyrTblRec = akTrans.GetObject(akObjId, OpenMode.ForRead) as LayerTableRecord;

                    akDoc.Editor.WriteMessage("\n" + akLyrTblRec.Name);
                }
                // Disposes the transacation
            }
        }
    }
}
