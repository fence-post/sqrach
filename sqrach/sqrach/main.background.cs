using System;
using System.ComponentModel;
using System.Windows.Forms;
using fp.lib;
using fp.lib.dbInfo;
using fp.lib.mysql;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    public partial class main : Form
    {
        #region background
        void BackgroundInitialize(object sender, DoWorkEventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
            
            BackgroundWorkLoop();
        }

        void LoadData()
        {
            if (A.db != null)
            {
                A.db.LoadStructure();
                A.db.FindExplicitRelationships();
            }
            Settings.Load();
            DbInfo.analyzeColumnDataContext = S.Get("EnableExtendedDataProfiling", false);
            DbInfo.findInferredRelationships = S.Get("UseInferredRelationships", true);
            lib.sqlparser.Query.InitializeForParsing();
            Background.status = BackgroundStatus.None;
        }

        public void BackgroundWorkLoop()
        {
            while (true)
            {
                bool analyzing = false;

                if (Background.status == BackgroundStatus.ShuttingDown)
                    break;
                else if (Background.status == BackgroundStatus.Loading)
                {
                    LoadData();
                }
                else if (Background.status == BackgroundStatus.QueryQueued)
                {
                    background.RunQuery();
                    queryHistory.dirty = true;
                }
                else if (A.db != null && A.db.databaseStructureAnalyzed && Parser.suggestionsNeedUpdating && S.Get("QuerySuggestions", true))
                {
                    A.AddToLog("updating suggestions");
                    Parser.UpdateSuggestions();
                    A.AddToLog("done");
                }
                else if (A.db != null && A.db.databaseType == "MySql" && A.db.databaseStructureAnalyzed == false)
                {
                    A.db.AnalyzeDatabaseStructure();
                    objectTreesDirty = true;
                }
                else if (Environment.TickCount - lastEditorChange > 5000)
                {
                    if(A.db != null && A.db.databaseType == "MySql" && A.db.databaseStructureAnalyzed && S.Get("EnableDataProfiling", true))
                    {
                        bool logAnalyzeData = S.Get("LogDataProfiling", false);
                        string msg;
                        analyzing = A.db.AnalyzeDatabaseData(out msg);
                        if(msg != null && logAnalyzeData)
                            A.AddToLog(msg);

                        if(!analyzing)
                        {
                            if(Environment.TickCount - lastSaveSettings > (1000*60*5))
                            {
                                lastSaveSettings = Environment.TickCount;
                                Settings.SaveSettings(false);
                            }
                        }
                    }
                }
                T.Sleep(analyzing ? 50 : 500);
            }
            Background.status = BackgroundStatus.Stopped;
        }

        int lastSaveSettings = 0;

        #endregion      
    }
}
