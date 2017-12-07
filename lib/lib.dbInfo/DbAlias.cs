using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fp.lib.dbInfo
{
    public class DbAlias : DbObject
    {
        [Persistent]
        public string alias;
        public DbTable table;

        [Persistent]
        public string tableName { get { return table.name; } set { table = DbTable.dbInfo.tables[value];  } }

        public DbAlias()
        {

        }

        public DbAlias(string a, DbTable t)
        {
            alias = a;
            table = t;
        }
    }
}
