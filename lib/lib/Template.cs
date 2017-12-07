using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace fp.lib
{
    public class Template
    {
        protected string template = "";
        bool indexMode;
        protected Dictionary<string, string> tokens = new Dictionary<string, string>();

        public Template(string s, bool sequenceMode = false)
        {
            indexMode = sequenceMode;
            template = s;
        }

        public void SetTemplate(string s)
        {
            template = s;
        }

        /*
        public void LoadLocals(MethodBase method)
        {
            MethodBody mb = method.GetMethodBody();
            Console.WriteLine();
            foreach (LocalVariableInfo lvi in mb.LocalVariables)
            {
                SetToken(lvi, lvi.ToString());
            }
        }
         * */

        public void SetToken(string key, string val)
        {
            if (tokens.ContainsKey(key))
                tokens.Remove(key);
            tokens.Add(key, val);
        }

        
        public void SetTokens(QSqlBase s)
        {
            for(int i = 0; i < s.FieldCount; i++)
            {
                SetToken(s.GetColumnName(i), s.GetString(i));
            }

        }
        

        public void SetTokensFromObject(Object o, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var prop in o.GetType().GetFields(flags))
                SetToken(prop.Name, prop.GetValue(o).ToString());
            
        }

        public void SetTokens(Dictionary<string, string> attribs)
        {
            foreach (string key in attribs.Keys)
            {
                string val = attribs[key];
                SetToken(key, val);
            }
        }
        
        public virtual string Render()
        {
            string txt = template;

            if(indexMode)
            {
                foreach (string s in tokens.Keys)
                    txt = txt.Replace("@" + s, tokens[s]);
            }
            else
            {
                foreach (string s in tokens.Keys)
                    txt = txt.Replace("#" + s + "#", tokens[s]);
            }

            // in oya2 make a subclass of Template to handle this

            // replace inline category names

            /*
            if(p != null)
            {
                int start = txt.IndexOf("#c(");
                while (start >= 0)
                {
                    int end = txt.IndexOf(")", start);
                    if (end <= start)
                        break;
                    string match = txt.Substring(start, end - (start - 2));
                    // p.debug.Add("match", match);
                    start += 3;
                    string categoryName = txt.Substring(start, end - (start));
                    // p.debug.Add("categoryName", categoryName);
                    int categoryId = Page.GetCategoryId(categoryName.Replace(" Recruiters", ""));// QMySql.SqlInt("select id from categories where name='" + categoryName + "'");
                    if (categoryId > 0)
                        categoryName = p.l(categoryName, "Category", categoryId);
                    txt = txt.Replace(match, categoryName);
                    start = txt.IndexOf("#c(");
                }

                // replace inline category block
                start = txt.IndexOf("#cb(");
                while (start >= 0)
                {
                    int end = txt.IndexOf(")#", start);
                    if (end <= start)
                        break;
                    string match = txt.Substring(start, end - (start - 2));
                    // p.debug.Add("match", match);
                    start += 4;
                    string categoryBlock = txt.Substring(start, end - (start));
                    // p.debug.Add("categoryName", categoryName);

                    categoryBlock = RenderCategoryBlock(p, categoryBlock);
                   
                    txt = txt.Replace(match, categoryBlock);
                    start = txt.IndexOf("#cb(");
                }
            }
                  */


            return txt;
        }
        /*
        protected string RenderCategoryBlock(Page p, string buf)
        {
            string html = "";
            string[] subBlocks = buf.Split(new char[] { '\n' });
            foreach (string s in subBlocks)
            {
                html += RenderCategories(p, s);
            }

            if (html != "")
                html = "<table border=0 cellpadding=3 cellspacing=2 class='mySpecialtiesTable'>" + html + "</table>";

            return html;
        }

        protected string RenderCategories(Page p, string cats, string cats2 = "", string cats3 = "")
        {
            cats = cats.Trim();
            cats2 = cats2.Trim();

            if (cats == "")
                return "";

            string result = "";

            string cool = "";
            List<string> bigCats;
            List<string> coolCats;
            List<string> smallCats;
            if (cats2 == "")
            {
                smallCats = cats.ToListOfStrings();
                bigCats = new List<string>();
                bigCats.Add(smallCats[0]);
                smallCats.RemoveAt(0);
            }
            else
            {
                bigCats = cats.ToListOfStrings();
                smallCats = cats2.ToListOfStrings();
            }

            if (cats3 != "")
            {
                coolCats = cats3.ToListOfStrings();
                cool = p.RenderCatBlock(coolCats, false, "<br>");
            }

            string left = p.RenderCatBlock(bigCats);
            string right = p.RenderCatBlock(smallCats);

            if(cool != "")
                cool = "<td class='mySpecialtyCool'>" + cool + "</td>";
            result += "<tr><td class='mySpecialtyBig'>" + left + "</td><td class='mySpecialtySmall'>" + right + "</td>" + cool + "</tr>";

            // result += "<tr><td class='mySpecialtyBig'>" + left + "</td><td class='mySpecialtySmall'>" + right + "</td><td class='mySpecialtyCool'>" + cool + "</td></tr>";
            //            result += @"<div class='vertBig'>" + left + "</div><div class='vertSmall'>" + right + "</div>"; // <div class='vertSmall'>" + cool + "</div>";
            // result += @"<p><b>" + left + "</b><br>" + right + "</p>"; // <div class='vertSmall'>" + cool + "</div>";

            return result;
        }
     */
    }

}