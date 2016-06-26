namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.IO;
    using System.Linq;

    public class SharePointInlineCodeSupportCheck : BaseIntrospectionRule
    {
        private int m_iStringIdForProblem;

        public SharePointInlineCodeSupportCheck() : base("SharePointInlineCodeSupportCheck", "SharePointCustomRules.CustomRules", typeof(SharePointInlineCodeSupportCheck).Assembly)
        {
            this.m_iStringIdForProblem = 0;
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(module.Directory);
            if (directoryInfo.Exists)
            {
                this.ProcessDirectory(directoryInfo);
                this.SearchDirectory(directoryInfo);
            }
            return base.Problems;
        }

        private bool CheckIfInlineCodeExists(StreamReader streamReader)
        {
            short num = 0;
            string str = string.Empty;
            try
            {
                while (!streamReader.EndOfStream)
                {
                    str = streamReader.ReadLine();
                    if (str.Contains("<script runat=\"server\"") || str.Contains("<script language=\"c#\" runat=\"server\">"))
                    {
                        num = (short) (num + 1);
                        while (streamReader.ReadLine().Equals(string.Empty))
                        {
                            str = streamReader.ReadLine();
                            if (str.Contains("</script>"))
                            {
                                return false;
                            }
                        }
                    }
                    if (str.Contains("</script>"))
                    {
                        num = (short) (num + 1);
                    }
                    if (num.Equals((short) 2))
                    {
                        return true;
                    }
                }
            }
            catch (IOException exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:CheckIfInlineCodeExists() - " + exception.Message);
            }
            return false;
        }

        private bool CheckIfPageParserExclusionExists(DirectoryInfo directoryInfo)
        {
            string str3;
            string path = string.Empty;
            string str2 = string.Empty;
            short num = 0;
            bool flag = false;
            try
            {
                FileInfo[] files = directoryInfo.GetFiles("web.config");
                foreach (FileInfo info in files)
                {
                    path = directoryInfo.Name + @"\" + info;
                    if (File.Exists(path))
                    {
                        StreamReader reader = File.OpenText(path);
                        while (!reader.EndOfStream)
                        {
                            str2 = reader.ReadLine();
                            if (str2.Contains("<PageParserPath VirtualPath="))
                            {
                                num = (short) (num + 1);
                            }
                            if (str2.Contains("AllowServerSideScript=\"true\""))
                            {
                                num = (short) (num + 1);
                            }
                            if (2 == num)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (IOException exception)
            {
                str3 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:CheckIfPageParserExclusionExists() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str3 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:CheckIfPageParserExclusionExists() - " + exception2.Message);
            }
            return flag;
        }

        private void ProcessDirectory(DirectoryInfo directoryInfo)
        {
            string str3;
            bool flag = false;
            bool flag2 = false;
            try
            {
                FileInfo[] files;
                string str;
                string str2;
                Resolution resolution;
                if (directoryInfo.Name.Equals("LAYOUT"))
                {
                    flag2 = true;
                }
                else if (this.CheckIfPageParserExclusionExists(directoryInfo))
                {
                    flag = true;
                }
                if (flag2 || flag)
                {
                    files = directoryInfo.GetFiles("*.aspx");
                    if (files.Count<FileInfo>() > 0)
                    {
                        str = string.Empty;
                        str2 = string.Empty;
                        resolution = null;
                        foreach (FileInfo info in files)
                        {
                            str = directoryInfo.FullName + @"\" + info;
                            if (File.Exists(str) && this.CheckIfInlineCodeExists(File.OpenText(str)))
                            {
                                resolution = base.GetResolution(new string[] { str });
                                base.Problems.Add(new Problem(resolution, Convert.ToString(this.m_iStringIdForProblem)));
                                this.m_iStringIdForProblem++;
                            }
                        }
                    }
                }
                else if (!flag2)
                {
                    files = directoryInfo.GetFiles("*.aspx");
                    if (files.Count<FileInfo>() > 0)
                    {
                        str = string.Empty;
                        str2 = string.Empty;
                        resolution = null;
                        foreach (FileInfo info in files)
                        {
                            str = directoryInfo.FullName + @"\" + info;
                            if (File.Exists(str) && this.CheckIfInlineCodeExists(File.OpenText(str)))
                            {
                                resolution = base.GetResolution(new string[] { str });
                                base.Problems.Add(new Problem(resolution, Convert.ToString(this.m_iStringIdForProblem)));
                                this.m_iStringIdForProblem++;
                            }
                        }
                    }
                }
            }
            catch (IOException exception)
            {
                str3 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:ProcessDirectory() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str3 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:ProcessDirectory() - " + exception2.Message);
            }
        }

        private void SearchDirectory(DirectoryInfo directoryInfo)
        {
            try
            {
                foreach (DirectoryInfo info in directoryInfo.GetDirectories())
                {
                    this.ProcessDirectory(info);
                    this.SearchDirectory(info);
                }
            }
            catch (IOException exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointInlineCodeSupportCheck:SearchDirectory() - " + exception.Message);
            }
        }
    }
}

