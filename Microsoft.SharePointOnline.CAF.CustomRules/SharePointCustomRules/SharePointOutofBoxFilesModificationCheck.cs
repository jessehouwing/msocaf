namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class SharePointOutofBoxFilesModificationCheck : BaseIntrospectionRule
    {
        private List<string> m_ExistingListOOBFileNames;
        private int m_iStringIdForProblem;

        public SharePointOutofBoxFilesModificationCheck() : base("SharePointOutofBoxFilesModificationCheck", "SharePointCustomRules.CustomRules", typeof(SharePointOutofBoxFilesModificationCheck).Assembly)
        {
            this.m_ExistingListOOBFileNames = new List<string>();
            this.m_iStringIdForProblem = 0;
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            string str2;
            try
            {
                if (this.m_ExistingListOOBFileNames.Count.Equals(0))
                {
                    StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointCustomRules.SPOutOfBoxFileNames.txt"));
                    string item = string.Empty;
                    while (!reader.EndOfStream)
                    {
                        item = reader.ReadLine();
                        this.m_ExistingListOOBFileNames.Add(item);
                    }
                    reader.Close();
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(module.Directory);
                if (directoryInfo.Exists)
                {
                    this.ProcessDirectory(directoryInfo);
                    this.SearchDirectory(directoryInfo);
                }
            }
            catch (IOException exception)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointOutofBoxFilesModificationCheck:Check() - " + exception.Message);
            }
            catch (NullReferenceException exception2)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointOutofBoxFilesModificationCheck:Check() - " + exception2.Message);
            }
            catch (Exception exception3)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointOutofBoxFilesModificationCheck:Check() - " + exception3.Message);
            }
            return base.Problems;
        }

        private void ProcessDirectory(DirectoryInfo directoryInfo)
        {
            try
            {
                FileInfo[] files = directoryInfo.GetFiles();
                if (files.Count<FileInfo>() > 0)
                {
                    Resolution resolution = null;
                    foreach (FileInfo info in files)
                    {
                        if (this.m_ExistingListOOBFileNames.Contains(info.Name))
                        {
                            resolution = base.GetResolution(new string[] { info.Name });
#if ORIGINAL
                            base.Problems.Add(new Problem(resolution, Convert.ToString(this.m_iStringIdForProblem)));
#else
                            base.Problems.Add(new Problem(resolution, info.FullName, 0, Convert.ToString(this.m_iStringIdForProblem)));
#endif

                            this.m_iStringIdForProblem++;
                        }
                    }
                }
            }
            catch (IOException exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointOutofBoxFilesModificationCheck:ProcessDirectory() - " + exception.Message);
            }
        }

        private void SearchDirectory(DirectoryInfo directoryInfo)
        {
            foreach (DirectoryInfo info in directoryInfo.GetDirectories())
            {
                this.ProcessDirectory(info);
                this.SearchDirectory(info);
            }
        }
    }
}
