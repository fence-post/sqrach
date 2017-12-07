using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.mysql;

namespace fp.lib.dbInfo
{
    #region TablePath class

    public class TablePath
    {
        public bool isReference;
        public DbTable referencedTable { get { return isReference ? constraint.dbTable : constraint.referencedTable; } }
        public TablePath pathThrough;
        public DbTableConstraint constraint;

        public TablePath(DbTableConstraint c, TablePath p, bool r = false)
        {
            constraint = c;
            isReference = r;
            pathThrough = p;
        }

        public string RenderJoinCols(bool includeAlias)
        {
            return constraint.RenderJoinCols(includeAlias);
            /*
            string joinCols = null;
            foreach (DbTableConstraintColumn col in constraint.constraintColumns.Each())
            {
                if (col.referencedColumn != null)
                {
                    string leftCol = col.dbColumn.objectName;
                    string rightCol = col.referencedColumn.objectName;
                    if (includeAlias)
                    {
                        leftCol = col.dbColumn.table.GetAlias(true) + "." + leftCol;
                        rightCol = col.referencedColumn.table.GetAlias(true) + "." + rightCol;
                    }
                    joinCols = T.AppendTo(joinCols, leftCol + "=" + rightCol, " and ");
                }
            }
            return joinCols;
            */
        }

        public void RenderJoin(ref string result, bool includeAlias)
        {
            if (pathThrough != null)
                pathThrough.RenderJoin(ref result, includeAlias);
            string refAlias = includeAlias ? referencedTable.GetAlias(true) : "";
            string joinExpr = " inner join " + T.AppendTo(referencedTable.objectName, refAlias, " ") + " on ";
            if (!result.Contains(joinExpr))
            {
                string joinCols = RenderJoinCols(includeAlias);
                T.Assert(joinCols != null);
                result += joinExpr + joinCols;
            }
        }
    }

    #endregion

    #region TablePaths collection

    public class DbTablePaths
    {
        public DbTable table;
        public QDict<string,TablePath> paths = new QDict<string, TablePath>();

        public DbTablePaths(DbTable t)
        {
            table = t;
            GetAccessibleTables(null);
            GetAccessibleTablesThroughReference(null);
        }

        public IEnumerable<string> accessibleTableNames
        {
            get
            {
                foreach (string t in paths.Keys)
                    yield return t;
            }
        }

        public IEnumerable<DbTable> accessibleTables
        {
            get
            {
                foreach (TablePath p in paths.Values)
                    yield return p.referencedTable;
            }
        }

        public IEnumerable<string> accessiblePathNames
        {
            get
            {
                foreach (string t in paths.Keys)
                    yield return GetPathLabel(t);
            }
        }

        public string GetPathLabel(string table)
        {
            string result = "";
            TablePath path = paths[table];
            while(path != null)
            {
                result = T.AppendTo(path.referencedTable.objectName, result, ".");
                path = path.pathThrough;
            }
            return result;
        }

        void GetAccessibleTables(TablePath path)
        {
            DbTable t = path == null ? table : path.referencedTable;

            foreach (DbTableConstraint constraint in t.constraints.Values)
            {
                if (constraint.isForeignKey || constraint.type == "inferred")
                {
                    if (!paths.ContainsKey(constraint.referencedTable.objectName))
                    {
                        if (constraint.referencedTable != table)
                        {
                            TablePath p = new TablePath(constraint, path);
                            paths.Add(constraint.referencedTable.objectName, p);
                            GetAccessibleTables(p);
                        }
                    }
                }
            }
        }

        void GetAccessibleTablesThroughReference(TablePath path)
        {
            DbTable t = table;

            foreach (DbTableConstraint constraint in t.references.Each())
            {
                if (constraint.isForeignKey || constraint.type == "inferred")
                {
                    if (!paths.ContainsKey(constraint.dbTable.objectName))
                    {
                        if (constraint.dbTable != table)
                        {
                            TablePath p = new TablePath(constraint, path, true);
                            paths.Add(constraint.dbTable.objectName, p);
                            // GetAccessibleTablesThroughReference(p);
                        }
                    }
                }
            }
        }
    }
    #endregion
}


/*
 * 
        public bool IsColumnPartOfForeignKey(ColumnInfo c)
        {
            T.Assert(c.tableInfo == table);
            foreach (TablePath p in paths.Values)
                if (p.constraint.constraintColumns.ContainsKey(c.objectName))
                    return true;

            return false;
        }

        
public void GetAccessibleTables()
{

}

public TablePath GetOrAdd(TableInfo t)
{
    if (accessibleTables.ContainsKey(t.objectName))
        return accessibleTables[t.objectName];
    TablePath path = new TablePath(t);
    return path;

}
*/
