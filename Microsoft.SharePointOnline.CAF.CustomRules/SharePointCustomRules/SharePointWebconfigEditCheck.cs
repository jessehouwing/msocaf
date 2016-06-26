namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Linq;

    public class SharePointWebconfigEditCheck : BaseIntrospectionRule
    {
        private string m_sPossibleWebConfigName;

        public SharePointWebconfigEditCheck() : base("SharePointWebconfigEditCheck", "SharePointCustomRules.CustomRules", typeof(SharePointWebconfigEditCheck).Assembly)
        {
            this.m_sPossibleWebConfigName = string.Empty;
        }

        public override ProblemCollection Check(Member member)
        {
            Method method = null;
            int iStringIdForProblem = 0;
            this.m_sPossibleWebConfigName = member.DeclaringType.DeclaringModule.Name + ".exe.config";
            try
            {
                if (member is Method)
                {
                    method = member as Method;
                    this.ValidateForStreamWriter(method, iStringIdForProblem);
                    this.ValidateForXMLDocument(method, iStringIdForProblem);
                    this.ValidateForConfigManager(method, iStringIdForProblem);
                    this.ValidateForXMLTextWriter(method, iStringIdForProblem);
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointWebconfigEditCheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }

        private void ValidateForConfigManager(Method method, int iStringIdForProblem)
        {
            bool flag = false;
            Resolution resolution = null;
            InstructionCollection source = method.Instructions;
            try
            {
                for (int i = 0; i < source.Count<Instruction>(); i++)
                {
                    if (source[i].Value != null)
                    {
                        if (source[i].Value.ToString().Contains("System.Configuration.ConfigurationManager.OpenExeConfiguration"))
                        {
                            flag = true;
                        }
                        if (flag && source[i].Value.ToString().Contains("System.Configuration.Configuration.Save"))
                        {
                            resolution = base.GetResolution(new string[] { method.ToString() });
                            base.Problems.Add(new Problem(resolution, Convert.ToString(iStringIdForProblem)));
                            iStringIdForProblem++;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointWebconfigEditCheck:ValidateForConfigManager() - " + exception.Message);
            }
        }

        private void ValidateForStreamWriter(Method method, int iStringIdForProblem)
        {
            bool flag = false;
            Resolution resolution = null;
            InstructionCollection source = method.Instructions;
            try
            {
                for (int i = 0; i < source.Count<Instruction>(); i++)
                {
                    if ((((i > 0) && (source[i].Value != null)) && (source[i - 1].Value != null)) && ((source[i].Value.ToString().Contains("System.IO.File.CreateText") && source[i - 1].Value.ToString().Equals("web.config")) || source[i - 1].Value.ToString().Equals(this.m_sPossibleWebConfigName)))
                    {
                        flag = true;
                    }
                    if ((flag && (source[i].Value != null)) && source[i].Value.ToString().Contains("System.IO.TextWriter.WriteLine"))
                    {
                        resolution = base.GetResolution(new string[] { method.ToString() });
                        base.Problems.Add(new Problem(resolution, Convert.ToString(iStringIdForProblem)));
                        iStringIdForProblem++;
                        flag = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointWebconfigEditCheck:ValidateForStreamWriter() - " + exception.Message);
            }
        }

        private void ValidateForXMLDocument(Method method, int iStringIdForProblem)
        {
            Resolution resolution = null;
            InstructionCollection source = method.Instructions;
            try
            {
                for (int i = 0; i < source.Count<Instruction>(); i++)
                {
                    if ((((i > 0) && (source[i].Value != null)) && (source[i - 1].Value != null)) && ((source[i].Value.ToString().Contains("System.Xml.XmlDocument.Save") && source[i - 1].Value.ToString().Equals("web.config")) || source[i - 1].Value.ToString().Equals(this.m_sPossibleWebConfigName)))
                    {
                        resolution = base.GetResolution(new string[] { method.ToString() });
                        base.Problems.Add(new Problem(resolution, Convert.ToString(iStringIdForProblem)));
                        iStringIdForProblem++;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointWebconfigEditCheck:ValidateForXMLDocument() - " + exception.Message);
            }
        }

        private void ValidateForXMLTextWriter(Method method, int iStringIdForProblem)
        {
            bool flag = false;
            Resolution resolution = null;
            InstructionCollection source = method.Instructions;
            try
            {
                for (int i = 0; i < source.Count<Instruction>(); i++)
                {
                    if (source[i].Value == null)
                    {
                        continue;
                    }
                    if (source[i].Value.ToString().Equals("web.config") || source[i].Value.ToString().Equals(this.m_sPossibleWebConfigName))
                    {
                        while (source[++i].OpCode != OpCode.Newobj)
                        {
                            if (i > source.Count<Instruction>())
                            {
                                break;
                            }
                        }
                        if ((source[i].Value != null) && (source[i].Value.ToString().Contains("System.Xml.XmlTextWriter") && source[i].OpCode.Equals(OpCode.Newobj)))
                        {
                            flag = true;
                        }
                    }
                    if ((flag && (source[i].Value != null)) && source[i].Value.ToString().Contains("System.Xml.XmlWriter.WriteElementString"))
                    {
                        resolution = base.GetResolution(new string[] { method.ToString() });
                        base.Problems.Add(new Problem(resolution, Convert.ToString(iStringIdForProblem)));
                        iStringIdForProblem++;
                        flag = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointWebconfigEditCheck:ValidateForXMLTextWriter() - " + exception.Message);
            }
        }
    }
}

