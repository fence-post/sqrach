using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;

namespace fp.lib.dbInfo
{
    public class DbTableConstraint
    {
        public string name;
        public string type;
        public DbTable dbTable;
        public DbTable referencedTable;
        public bool isForeignKey { get { return type != "inferred" && referencedTable != null; } }
        public bool isSloppyForeignKey { get { return type == "inferred" || referencedTable != null; } }
        public QDict<string, DbTableConstraintColumn> constraintColumns = new QDict<string, DbTableConstraintColumn>();

        public DbTableConstraint(string n, string t, DbTable ta)
        {
            name = n;
            type = t;
            dbTable = ta;
        }

        public DbTableConstraintColumn AddColumn(DbColumn cInfo, int seq = -1,
            bool non = false, int card = -1)
        {
            DbTableConstraintColumn c = constraintColumns.ContainsKey(cInfo.objectName) ? 
                constraintColumns[cInfo.objectName] : constraintColumns.Add(cInfo.objectName, new DbTableConstraintColumn(this, cInfo));
            c.seqInIndex = seq;
            c.nonUnique = non;
            c.cardinality = card;
            cInfo.constraints.Add(c);
            
            return c;
        }

        public DbTableConstraintColumn AddInferredRelationship(string col, DbTable refTable, DbColumn refColumn)
        {
            DbTableConstraintColumn c = constraintColumns.ContainsKey(col) ? constraintColumns[col] :
                constraintColumns.Add(col, new DbTableConstraintColumn(this, dbTable.columns[col]));
            referencedTable = refTable;
            refTable.references.Add(this);
            if(refColumn != null)
            {
                c.referencedColumn = refColumn;
                refColumn.references.Add(c);
            }
            return c;
        }

        public DbTableConstraintColumn AddReference(string col, DbTable refTable, string refColumn, int ord, int pos)
        {
            DbTableConstraintColumn c = constraintColumns.ContainsKey(col) ? constraintColumns[col] :
                constraintColumns.Add(col, new DbTableConstraintColumn(this, dbTable.columns[col]));
            referencedTable = refTable;
            refTable.references.Add(this);
            c.ordinalPosition = ord;
            c.positionInUniqueConstraint = pos;
            c.referencedColumn = refTable.columns[refColumn];
            refTable.columns[refColumn].references.Add(c);
            return c;
        }

        public string RenderJoinCols(bool includeAlias)
        {
            string joinCols = null;
            foreach (DbTableConstraintColumn col in constraintColumns.Each())
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
        }

    }

    public class DbTableConstraintColumn
    {
        public DbTableConstraint constraint;
        public DbColumn dbColumn;
        public int seqInIndex;
        public int ordinalPosition;
        public DbColumn referencedColumn;
        public int positionInUniqueConstraint;
        public bool nonUnique;
        public int cardinality;
      
        public DbTableConstraintColumn(DbTableConstraint con, DbColumn c)
        {
            constraint = con;
            dbColumn = c;
            c.constraints.Add(this);
        }
    }
}
