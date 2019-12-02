using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;


[assembly: CommandClass(typeof(AutoCAD.Program))]

namespace AutoCAD
{
    public class Program
    {
        [CommandMethod("MakeMText")]
        public static void Main()
        {
            Document akDoc = Application.DocumentManager.MdiActiveDocument;
            Database akDb = akDoc.Database;

            using(Transaction akTrans = akDb.TransactionManager.StartTransaction())
            {
                BlockTable akBlkTbl;
                akBlkTbl = akTrans.GetObject(akDb.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord akBlkTblRec;
                akBlkTblRec = akTrans.GetObject(akBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // creating new MText and assigning location and such
                MText objText = new MText();
                objText.SetDatabaseDefaults();
                objText.Location = new Autodesk.AutoCAD.Geometry.Point3d(12,12,0);
                objText.Contents = "hello";
                objText.TextStyleId = akDb.Textstyle;

                //append new MText object to the Model Space
                akBlkTblRec.AppendEntity(objText);

                //append the new MText object to the active Transaction
                akTrans.AddNewlyCreatedDBObject(objText, true);

                akTrans.Commit();
            }
        }
    }
}
