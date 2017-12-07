using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using fp.lib;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class Column : Token
    {
        public enum ColumnType { Db, DynamicTableColumnReference, ScalarSubquery, Proposed, WildCard, Reference }
        public enum ColumnSortDirection { None, Asc, Desc }

        public ColumnType columnType;
        public DbColumn dbColumn = null;
        public Query scalarSubquery = null;
        public Identifier columnNameToken = null;
        public Identifier tableAliasToken = null;
        public Identifier columnAliasToken = null;
        public Keyword asToken = null;
        public Expression columnExpression = null;
        public Table table = null;
        public string proposedColumnName = null;
        public string proposedTableAlias = null;
        public string proposedTable = null;

        public string tableName { get { return table != null && table.dbTable != null ? table.dbTable.name : proposedTable;  } }
        public string tableAlias { get { return table == null ? proposedTableAlias : table.tableAlias; } }
        public string columnAlias { get { return columnAliasToken == null ? "" : columnAliasToken.name; } }
        public string columnName
        {
            get
            {
                if (columnAliasToken != null)
                    return columnAliasToken.name;
                if (table != null && dbColumn != null)
                    return dbColumn.name;
                return proposedColumnName;
            }
        }

        public Column(Identifier tTableAlias, Table t) : base(TokenType.Column, tTableAlias.startOffset, "*")
        {
            charAfter = '.';
            columnType = ColumnType.WildCard;
            table = t;
            tableAliasToken = tTableAlias;
            UpdateBounds();
        }

        public Column(Identifier tCol, Identifier tTableAlias, Table t, DbColumn dbc) : base(TokenType.Column, tCol.startOffset, tCol.name)
        {
            parentQuery = tCol.parentQuery;
            charAfter = tCol.charAfter;
            columnNameToken = tCol;
            tableAliasToken = tTableAlias;
            table = t;
            columnType = ColumnType.Reference;
            if (table.subquery != null)
            {
                proposedColumnName = tCol.name;
                proposedTableAlias = t.tableAlias;
                proposedTable = t.tableAlias;
            }
            else
            {
                T.Assert(table.dbTable != null);
                dbColumn = dbc;
            }
            UpdateBounds();
        }

        public Column(Keyword tAs, Identifier tAlias, Identifier tCol, Identifier tTableAlias, Table t) : base(TokenType.Column, tCol.startOffset, tCol.name)
        {
            parentQuery = tCol.parentQuery;
            charAfter = tCol.charAfter;
            columnAliasToken = tAlias;
            columnNameToken = tCol;
            tableAliasToken = tTableAlias;
            asToken = tAs;
            table = t;
            if (table.subquery != null)
            {
                proposedColumnName = tCol.name;
                proposedTableAlias = t.tableAlias;
                proposedTable = t.tableAlias;
                columnType = ColumnType.DynamicTableColumnReference;
            }
            else
            {
                T.Assert(table.dbTable != null);
                dbColumn = table.dbTable.columns[tCol.name];
                columnType = ColumnType.Db;
            }
            UpdateBounds();
        }

        public Column(Keyword tAs, Identifier tAlias, Identifier t, Identifier tTableAlias, DbTable table, string tableAlias) : base(TokenType.Column, t.startOffset, t.name)
        {
            parentQuery = t.parentQuery;
            asToken = tAs;
            columnAliasToken = tAlias;
            tableAliasToken = tTableAlias;
            columnNameToken = t;
            charAfter = t.charAfter;
            columnType = ColumnType.Proposed;
            proposedTableAlias = tableAlias;
            proposedTable = table.name;
            UpdateBounds();
        }

        public Column(Identifier n, Keyword a, Query q) : base(TokenType.Column, q.startOffset, q.expression + " as " + n.name)
        {
            parentQuery = q.parentQuery;
            asToken = a;
            columnAliasToken = n;
            scalarSubquery = q;
            q.parentToken = this;
            columnType = ColumnType.ScalarSubquery;
            UpdateBounds();
        }

        private void UpdateBounds(Token t, ref int leftMostExtent, ref int rightMostExtent)
        {
            if(t != null)
            {
                leftMostExtent = Math.Min(t.startOffset, leftMostExtent);
                rightMostExtent = Math.Max(rightMostExtent, t.rightExtent);

                if(leftMostExtent < parentQuery.startOffset)
                    leftMostExtent = parentQuery.startOffset;
                if (rightMostExtent > parentQuery.rightExtent)
                    rightMostExtent = parentQuery.rightExtent;

            }
        }

        private void UpdateBounds()
        {
            int leftMostExtent = startOffset;
            int rightMostExtent = rightExtent;
            UpdateBounds(asToken, ref leftMostExtent, ref rightMostExtent);
            UpdateBounds(columnNameToken, ref leftMostExtent, ref rightMostExtent);
            UpdateBounds(scalarSubquery, ref leftMostExtent, ref rightMostExtent);
            UpdateBounds(columnAliasToken, ref leftMostExtent, ref rightMostExtent);
            UpdateBounds(columnExpression, ref leftMostExtent, ref rightMostExtent);
            UpdateBounds(tableAliasToken, ref leftMostExtent, ref rightMostExtent);
            startOffset = leftMostExtent;
            SetExpression(rootQuery.expression.Substring(startOffset, rightMostExtent - leftMostExtent));
        }

        public override TokenList GetChildren()
        {
            TokenList result = new TokenList();
            result.Add(asToken);
            result.Add(columnNameToken);
            result.Add(scalarSubquery);
            result.Add(columnAliasToken);
            result.Add(columnExpression);
            result.Add(tableAliasToken);
            return result;
        }

        public int significance
        {
            get
            {
                int score = 0;
                switch (columnType)
                {
                    case Column.ColumnType.Db:
                        score = 100;
                        break;
                    case Column.ColumnType.ScalarSubquery:
                        score = 80;
                        break;
                    case Column.ColumnType.DynamicTableColumnReference:
                        score = 70;
                        break;
                    case Column.ColumnType.WildCard:
                        score = 60;
                        break;
                    case Column.ColumnType.Proposed:
                        score = 30;
                        break;
                    case Column.ColumnType.Reference:
                        score = 20;
                        break;
                }

                if(dbColumn != null)
                {
                    if (dbColumn.primaryKey)
                        score += 20;
                }

                return score;
            }
        }
     
        protected override string GetDebugText()
        {
            return tokenType.ToString() + " " + columnName;
        }
    }
}
