using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.dbInfo;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    public class QuerySuggestions
    {
        lib.sqlparser.Query query = null;

        public QuerySuggestions(lib.sqlparser.Query q)
        {
            query = q;
        }

        public IEnumerable<DbColumn> selectColumnSuggestions
        {
            get
            {
                return GetColumnSuggestions("selecty", query.select == null ? null : query.select.columns);
            }
        }

        public IEnumerable<DbColumn> orderColumnSuggestions
        {
            get
            {
                return GetColumnSuggestions("ordery", query.order == null ? null : query.order.columns);
            }
        }

        public IEnumerable<DbColumn> groupColumnSuggestions
        {
            get
            {
                return GetColumnSuggestions("groupy", query.group == null ? null : query.group.columns);
            }
        }


        IEnumerable<DbColumn> GetColumnSuggestions(string affinityTable, ColumnList columnsAlready)
        {
            HashSet<DbColumn> list = new HashSet<DbColumn>();

            if(query.from != null)
            {
                foreach (Table t in query.from.tables.tokens)
                    if(t.dbTable.queryColumns.ContainsKey(affinityTable))
                    {
                        foreach (DbColumn c in t.dbTable.queryColumns[affinityTable])
                            if (columnsAlready == null || columnsAlready.GetColumnForDbColumn(c) == null)
                                yield return c;
                    }
            }

            /*
            // Dictionary<DbColumn, double> columns = new Dictionary<DbColumn, double>();
            foreach (DbColumn c in columns.KeysSortedByValue())
                if (columnsAlready == null || columnsAlready.GetColumnForDbColumn(c) == null)
                    yield return c;
                */
        }

        void SaveQueryParts()
        {
            // given table
            //  save columns, save order, save group
            // given 
        }
    }
}
