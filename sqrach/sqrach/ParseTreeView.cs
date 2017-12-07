using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    public class ParseTreeNode : TreeNode
    {
        public Token token;

        public ParseTreeNode(Token t, string label) : base(label)
        {
            token = t;
        }
    }

    public class ParseTreeView : MultiSelectTreeView, IParentChildVisitor
    {
        public string expandMode { get { return S.Get("parseTreeExpandMode", "ExpandRootQueryToPrimaryKeywords"); } }
        public bool canExpand { get { return maxExpandedLevel < maxLevels; } }
        public bool canCollapse { get { return maxExpandedLevel > 0; } }

        Dictionary<Token, TreeNode> nodesAdded = new Dictionary<Token, TreeNode>();
        int maxLevels = 0;
        int maxExpandedLevel = -1;
        int minExpandedLevel = 99;

        public void OnAfterExpand(object sender, TreeViewEventArgs e)
        {
            minExpandedLevel = Math.Min(minExpandedLevel, e.Node.Level);
            maxExpandedLevel = Math.Max(maxExpandedLevel, e.Node.Level);
        }

    
        public void ExpandOneLevel()
        {
            int newValue = maxExpandedLevel + 1;
            this.ExpandToLevel(newValue);
            maxExpandedLevel = newValue;
        }

        public void CollapseOneLevel()
        {
            int newValue = maxExpandedLevel - 1;
            this.ExpandToLevel(newValue);
            maxExpandedLevel = newValue;
        }

        public void UpdateTree(lib.sqlparser.Query q)
        {
            int prevExpandedLevel = maxExpandedLevel;
            maxLevels = 0;
            maxExpandedLevel = -1;
            minExpandedLevel = 99;
            nodesAdded.Clear();
            Nodes.Clear();
            q.Accept(this);
            this.ExpandToLevel(T.MinMax(0, maxLevels, prevExpandedLevel));
        }

        public void Visit(Token parent, TokenList children)
        {
            TreeNode parentNode = null;
            if (nodesAdded.ContainsKey(parent))
                parentNode = nodesAdded[parent];
            else if(parent == lib.sqlparser.Query.rootQuery)
                parentNode = AddNode(null, null, parent);
            if(parentNode != null)
                foreach (Token t in children.tokens)
                {
                    AddNode(parent, parentNode, t);
                    if (IsExpanded(parent))
                        parentNode.Expand();
                }
        }

        bool ShowingType(Token t)
        {
            string showMode = S.Get("parseTreeShow");
            if (showMode == "Custom" && t.tokenType == TokenType.Column && !S.Get("parseTreeShowColumns", true))
                return false;
            if (showMode == "Custom" && t.tokenType == TokenType.Expression && !S.Get("parseTreeShowExpressions", false))
                return false;
            if (showMode == "Sparse" && t.tokenType == TokenType.Column && t.parentToken != t.parentQuery.select)
                return false;
            TokenType[] standardTypes = { TokenType.Table, TokenType.Column, TokenType.Query, TokenType.Select, TokenType.From, TokenType.Group, TokenType.Where, TokenType.Order };
            if (standardTypes.Contains(t.tokenType) == false && (showMode == "Sparse" || (showMode == "Custom" && false == S.Get("parseTreeShowAllTokens", false))))
                return false;

            return true;
        }

        bool IsExpanded(Token t)
        {
            if (expandMode == "ExpandAll")
                return true;
            if (expandMode == "ExpandAllQueriesToPrimaryKeywords")
            {
                if (t.isPrimaryKeyword || t.tokenType == TokenType.Query)
                    return true;
            }
            if (expandMode == "ExpandRootQueryToPrimaryKeywords")
            {
                if (t == lib.sqlparser.Query.rootQuery || (t.parentQuery == lib.sqlparser.Query.rootQuery && t.isPrimaryKeyword))
                    return true;
            }
            return false;
        }

        public bool CanDeleteSelectedNodes()
        {
            return true;
        }

        public void DeleteSelectedNodes(SqlBuilder b)
        {
            foreach (TreeNode n in SelectedNodes)
            {
                ParseTreeNode ptn = n as ParseTreeNode;
                b.Delete(ptn.token);
            }
        }

        TreeNode AddNode(Token parentToken, TreeNode parent, Token t)
        {
            if (nodesAdded.ContainsKey(t))
                return nodesAdded[t];

            if (!ShowingType(t))
                return null;

            string label = t.shortName.LimitToLength(40);
            if (S.Get("parseTreeShow") == "Details")
                label += " - " + t.startOffset + " " + t.expressionLength + " " + t.GetLine();
            ParseTreeNode node = new ParseTreeNode(t, label);
            if (parent == null)
                Nodes.Add(node);
            else
                parent.Nodes.Add(node);
            node.ForeColor = UI.GetTokenForeColor(t);
            nodesAdded.Add(t, node);

            maxLevels = Math.Max(maxLevels, node.Level);

            return node;
        }
    }
}
