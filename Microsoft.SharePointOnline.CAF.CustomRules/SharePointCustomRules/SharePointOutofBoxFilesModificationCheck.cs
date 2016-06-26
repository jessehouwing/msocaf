namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public class SharePointOutofBoxFilesModificationCheck : BaseIntrospectionRule
    {
        private List<string> assembliesfrommanifest;
        private static List<string> fullyQualifiedResolutionStrings = new List<string>();
        private List<string> m_ExistingListOOBFileNames;
        private int m_iStringIdForProblem;

        public SharePointOutofBoxFilesModificationCheck() : base("SharePointOutofBoxFilesModificationCheck", "SharePointCustomRules.CustomRules", typeof(SharePointOutofBoxFilesModificationCheck).Assembly)
        {
            this.assembliesfrommanifest = new List<string>();
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
                    while (!reader.EndOfStream)
                    {
                        this.m_ExistingListOOBFileNames.Add(reader.ReadLine().ToUpperInvariant());
                    }
                    reader.Close();
                }
                DirectoryInfo info = new DirectoryInfo(module.Directory);
                if (info.Exists)
                {
                    FileInfo[] files = info.GetFiles("manifest.xml");
                    if (files.Count<FileInfo>() > 0)
                    {
                        this.ReadManifestFile(files[0]);
                        this.searchFiles();
                    }
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
                            base.Problems.Add(new Problem(resolution, Convert.ToString(this.m_iStringIdForProblem)));
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

        private void ReadManifestFile(FileInfo fileInfo)
        {
            XDocument document = XDocument.Load(fileInfo.FullName);
            XNamespace namespace2 = document.Root.Name.Namespace;
            this.assembliesfrommanifest = (from doc in document.Descendants((XName) (namespace2 + "TemplateFile")) select (doc.Attribute("Location") != null) ? doc.Attribute("Location").Value : string.Empty).ToList<string>();
        }

        private void SearchDirectory(DirectoryInfo directoryInfo)
        {
            foreach (DirectoryInfo info in directoryInfo.GetDirectories())
            {
                this.ProcessDirectory(info);
                this.SearchDirectory(info);
            }
        }

        private void searchFiles()
        {
            foreach (string str in this.assembliesfrommanifest)
            {
                Resolution resolution = null;
                if (this.m_ExistingListOOBFileNames.Contains(str.ToUpperInvariant()))
                {
                    resolution = base.GetResolution(new string[] { str });
                    if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                    {
                        base.Problems.Add(new Problem(resolution, Convert.ToString(this.m_iStringIdForProblem)));
                        this.m_iStringIdForProblem++;
                        fullyQualifiedResolutionStrings.Add(resolution.ToString());
                    }
                }
            }
        }
    }
}

