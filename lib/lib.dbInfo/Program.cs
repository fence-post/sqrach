using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fp.lib.dbInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            DbInfoMySql db = new DbInfoMySql();
            db.Connect("192.168.0.105", "classicmodels", "dev", "willys");
            db.LoadStructure();
            db.AnalyzeDatabaseStructure();
            while (true)
            {
                string msg;
                if (!db.AnalyzeDatabaseData(out msg))
                    break;
            }
            db.AnalyzeSummarize();
            db.Dump();
        }
    }
}
