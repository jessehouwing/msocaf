namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;

    public class SharePointMonitorScopeWebpartCheck : BaseIntrospectionRule
    {
        private int iStringIdForProblem;
        private List<Resolution> ResolutionList;

        public SharePointMonitorScopeWebpartCheck() : base("SharePointMonitorScopeWebpartCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRules.SharePointMonitorScopeWebpartCheck).Assembly)
        {
            this.ResolutionList = new List<Resolution>();
            this.iStringIdForProblem = 0;
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            string str;
            try
            {
                MetadataCollection<TypeNode>.Enumerator enumerator = module.Types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeNode current = enumerator.Current;
                    if (((current is ClassNode) && (null != current.BaseType)) && current.BaseType.FullName.Equals("Microsoft.SharePoint.WebPartPages.WebPart"))
                    {
                        this.VisitWebpartClass(current as ClassNode);
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheck" + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheck") + exception2.InnerException.Message + exception2.Message);
            }
            return base.Problems;
        }

        public void VisitWebpartClass(ClassNode classType)
        {
            Method method = null;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    if ((((!method.Name.ToString().Equals("OnInit") && !method.Name.ToString().Equals("Render")) && (!method.Name.ToString().Equals("OnPreRender") && !method.Name.ToString().Equals(".ctor"))) && !method.Name.ToString().Equals("get_")) && !method.Name.ToString().Equals("set_"))
                    {
                        this.VisitWebpartMethod(method);
                    }
                    this.VisitWebpartMethodWebService(method);
                }
            }
        }

        public void VisitWebpartMethod(Method method)
        {
            try
            {
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                int num = 0;
                Resolution resolution = null;
                MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Problem problem;
                    Instruction current = enumerator.Current;
                    if (!flag)
                    {
                        if (current.OpCode.Equals(OpCode.Call) || current.OpCode.Equals(OpCode.Callvirt))
                        {
                            resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                            problem = new Problem(resolution, current.SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                            problem = new Problem(resolution, current, Convert.ToString(this.iStringIdForProblem));
#endif
                            problem.Certainty = 90;
                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                             base.Problems.Add(problem);
                            this.iStringIdForProblem++;
                            return;
                        }
                        if (current.OpCode.Equals(OpCode.Newobj))
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Utilities.SPMonitoredScope"))
                            {
                                flag = true;
                                continue;
                            }
                            resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                            problem = new Problem(resolution, current.SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                            problem = new Problem(resolution, current, Convert.ToString(this.iStringIdForProblem));
#endif
                            problem.Certainty = 90;
                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                             base.Problems.Add(problem);
                            this.iStringIdForProblem++;
                            return;
                        }
                    }
                    if (current.OpCode.Equals(OpCode._Try) && flag)
                    {
                        flag2 = true;
                        num++;
                    }
                    if (((current.OpCode.Equals(OpCode._EndTry) && flag) && flag2) && (num > 0))
                    {
                        num--;
                        if (num == 0)
                        {
                            flag2 = false;
                            flag3 = true;
                        }
                    }
                    if ((flag3 && (current.OpCode.Equals(OpCode.Call) || current.OpCode.Equals(OpCode.Callvirt))) && !current.Value.ToString().Contains("System.IDisposable.Dispose"))
                    {
                        resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                        problem = new Problem(resolution, current.SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                        problem = new Problem(resolution, current, Convert.ToString(this.iStringIdForProblem));
#endif
                        problem.Certainty = 90;
                        problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                        problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                         base.Problems.Add(problem);
                        this.iStringIdForProblem++;
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheckVisitWebpartMethod" + exception.Message);
            }
        }

        public void VisitWebpartMethodWebService(Method method)
        {
            try
            {
                bool flag = false;
                bool flag2 = false;
                Resolution resolution = null;
                for (int i = 0; i < method.Instructions.Count; i++)
                {
                    Problem problem;
                    if (((null == method.Instructions[i].Value) || (!method.Instructions[i].OpCode.Equals(OpCode.Call) && !method.Instructions[i].OpCode.Equals(OpCode.Callvirt))) || ((!method.Instructions[i].Value.ToString().Contains("System.Data.Common.DbConnection.Open") && !method.Instructions[i].Value.ToString().Contains("System.Data.SqlClient")) && !method.Instructions[i].Value.ToString().Contains("System.Web.Services.Protocols.SoapHttpClientProtocol.Invoke")))
                    {
                        continue;
                    }
                    int num2 = i;
                    while (num2 >= 0)
                    {
                        if (method.Instructions[num2].OpCode.Equals(OpCode.Callvirt) || method.Instructions[num2].OpCode.Equals(OpCode.Call))
                        {
                            resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                            problem = new Problem(resolution, method.Instructions[i].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                            problem = new Problem(resolution, method.Instructions[i], Convert.ToString(this.iStringIdForProblem));
#endif
                            problem.Certainty = 90;
                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                             base.Problems.Add(problem);
                            this.iStringIdForProblem++;
                            break;
                        }
                        if (!flag2)
                        {
                            if (method.Instructions[num2].OpCode.Equals(OpCode._Try))
                            {
                                flag2 = true;
                            }
                            if ((flag2 && method.Instructions[num2].OpCode.Equals(OpCode.Newobj)) && method.Instructions[num2].Value.ToString().Contains("Microsoft.SharePoint.Utilities.SPMonitoredScope"))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (num2 == 1)
                        {
                            resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
                            problem = new Problem(resolution, method.Instructions[i], Convert.ToString(this.iStringIdForProblem));
#if ORIGINAL
                            problem = new Problem(resolution, method.Instructions[i].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                            problem = new Problem(resolution, method.Instructions[i], Convert.ToString(this.iStringIdForProblem));
#endif
                            problem.Certainty = 90;
                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                             base.Problems.Add(problem);
                            this.iStringIdForProblem++;
                            break;
                        }
                        i--;
                    }
                    if (flag)
                    {
                        for (int j = i; j >= method.Instructions.Count; j++)
                        {
                            if (method.Instructions[j].OpCode.Equals(OpCode._EndTry))
                            {
                                break;
                            }
                            if (method.Instructions[j].OpCode.Equals(OpCode.Call) || method.Instructions[j].OpCode.Equals(OpCode.Callvirt))
                            {
                                resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                                problem = new Problem(resolution, method.Instructions[i].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                problem = new Problem(resolution, method.Instructions[i], Convert.ToString(this.iStringIdForProblem));
#endif
                                problem.Certainty = 90;
                                problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                 base.Problems.Add(problem);
                                this.iStringIdForProblem++;
                                break;
                            }
                            if (j == (method.Instructions.Count - 1))
                            {
                                resolution = new Resolution("SPMonitoredScope is absent in {0} Webpart", new string[] { method.FullName });
#if ORIGINAL
                                problem = new Problem(resolution, method.Instructions[i].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                problem = new Problem(resolution, method.Instructions[i], Convert.ToString(this.iStringIdForProblem));
#endif
                                problem.Certainty = 90;
                                problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                 base.Problems.Add(problem);
                                this.iStringIdForProblem++;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheckVisitWebpartMethodWebService" + exception.Message);
            }
        }
    }
}
