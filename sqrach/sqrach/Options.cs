using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using fp.lib;

namespace fp.sqratch
{
    class Options
    {
        #region helpers

        protected bool Get(string method, bool def)
        {
            return Settings.Get(method.Replace("get_", ""), def);
        }

        protected void Set(string method, bool val)
        {
            Settings.Set(method.Replace("set_", ""), val);
        }

        protected int Get(string method, int def)
        {
            return Settings.Get(method.Replace("get_", ""), def);
        }

        protected void Set(string method, int val)
        {
            Settings.Set(method.Replace("set_", ""), val);
        }
        
        #endregion

        #region editor

        [DescriptionAttribute("Automatically add end parenthesis."), CategoryAttribute("Editor"), DefaultValueAttribute(false)]
        public bool AutocompleteParenthesis
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Automatically add end quotes."), CategoryAttribute("Editor"), DefaultValueAttribute(false)]
        public bool AutocompleteQuotes
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Enable autocomplete."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool Autocomplete
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Autocomplete keywords."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool AutocompleteKeywords
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Enable autocomplete for tables."), CategoryAttribute("Editor"),DefaultValueAttribute(true)]
        public bool AutocompleteTables
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Suggest join expressions for tables."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool AutocompleteJoins
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Enable autocomplete for table aliases."),CategoryAttribute("Editor"),DefaultValueAttribute(true)]
        public bool AutocompleteAliases
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Enable autocomplete for columns."),CategoryAttribute("Editor"),DefaultValueAttribute(true)]
        public bool AutocompleteColumns
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Automatically suggest table aliases for columns and tables in autocomplete."), CategoryAttribute("Editor"), DefaultValueAttribute(false)]
        public bool AutocompleteInsertAliases
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Automatically insert tables and joins when columns are added by autocomplete."), CategoryAttribute("Editor"), DefaultValueAttribute(false)]
        public bool AutocompleteInsertTables
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Remember recently used aliases."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool RememberAliases
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Suggest aliases based on table naming convention."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool SuggestAliases
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Show dropdowns in editor header to choose columns and tables that can be added to the query."), CategoryAttribute("Editor"), DefaultValueAttribute(true)]
        public bool QuerySuggestions
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Show data graph in background of columns in autocomplete showing data distribution (requires EnableExtendedDataProfiling)."), CategoryAttribute("Editor"), DefaultValueAttribute(false)]
        public bool AutoCompleteDataGraph
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        #endregion

        [DescriptionAttribute("Show log if query execution fails."), CategoryAttribute("Query Execution"), DefaultValueAttribute(true)]
        public bool ShowLogOnError
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Quietly handle query execute errors."), CategoryAttribute("Query Execution"), DefaultValueAttribute(true)]
        public bool QuietExecutionErrors
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Don't allow writable sql statements to run."), CategoryAttribute("Query Execution"), DefaultValueAttribute(true)]
        public bool BlockWritableQueries
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Command timeout in seconds."), CategoryAttribute("Query Execution"), DefaultValueAttribute(30)]
        public int CommandTimeout
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, 30); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Save the query when executed."), CategoryAttribute("Query Execution"), DefaultValueAttribute(true)]
        public bool SaveQueryOnExecute
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        #region Parser
        
        [DescriptionAttribute("Run parser in strict mode."),CategoryAttribute("Parser"),DefaultValueAttribute(false)]
        public bool StrictMode
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Enable background parser."),CategoryAttribute("Parser"),DefaultValueAttribute(true)]
        public bool EnableParser
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Quietly handle parser errors."),CategoryAttribute("Parser"),DefaultValueAttribute(true)]
        public bool QuietParserErrors
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }
        #endregion

        #region Database Structure
        [DescriptionAttribute("Do basic data profiling of table and column data."), CategoryAttribute("Database Structure"), DefaultValueAttribute(true)]
        public bool EnableDataProfiling
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Do extended profiling of column data."), CategoryAttribute("Database Structure"), DefaultValueAttribute(false)]
        public bool EnableExtendedDataProfiling
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Show data profiling progress in the log."), CategoryAttribute("Database Structure"), DefaultValueAttribute(false)]
        public bool LogDataProfiling
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, false); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Use foreign key relationships."), CategoryAttribute("Database Structure"), DefaultValueAttribute(true)]
        public bool UseExplicitRelationships
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }

        [DescriptionAttribute("Infer relationships by table and column naming convention."), CategoryAttribute("Database Structure"), DefaultValueAttribute(true)]
        public bool UseInferredRelationships
        {
            get { return Get(MethodBase.GetCurrentMethod().Name, true); }
            set { Set(MethodBase.GetCurrentMethod().Name, value); }
        }
        #endregion
    }
}
