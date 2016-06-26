namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SPDiagnosticsService : BaseIntrospectionRule
    {
        private int iStringIdForProblem;

        public SPDiagnosticsService() : base("SPDiagnosticsService", "SharePointCustomRules.CustomRules", typeof(SPDiagnosticsService).Assembly)
        {
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
                    if (current is ClassNode)
                    {
                        this.VisitWebServiceClass(current as ClassNode);
                    }
                    if ((current is ClassNode) && (null != current.BaseType))
                    {
                        if (current.BaseType.FullName.Equals("Microsoft.SharePoint.Administration.SPJobDefinition"))
                        {
                            this.VisitTimerJobClass(current as ClassNode);
                        }
                        if (((current.BaseType.FullName.Equals("Microsoft.SharePoint.SPWebEventReceiver") || current.BaseType.FullName.Equals("Microsoft.SharePoint.SPListEventReceiver")) || current.BaseType.FullName.Equals("Microsoft.SharePoint.SPItemEventReceiver")) || current.BaseType.FullName.Equals("Microsoft.SharePoint.SPEmailEventReceiver"))
                        {
                            this.VisiteventReceiverClass(current as ClassNode);
                        }
                        if (current.BaseType.FullName.Equals("Microsoft.SharePoint.SPFeatureReceiver"))
                        {
                            this.VisitFeatureReceiverClass(current as ClassNode);
                        }
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPDiagnosticsService" + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SPDiagnosticsService") + exception2.InnerException.Message + exception2.Message);
            }
            return base.Problems;
        }

        public void VisiteventReceiverClass(ClassNode classType)
        {
            Method method = null;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    this.VisitEventReceiverMethod(method);
                }
            }
        }

        public void VisitEventReceiverMethod(Method method)
        {
            Resolution resolution = null;
            try
            {
                Problem problem;
                MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Instruction current = enumerator.Current;
                    if (null != current.Value)
                    {
                        if (current.OpCode.Equals(OpCode.Call))
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local"))
                            {
                                continue;
                            }
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                            break;
                        }
                        if (current.OpCode.Equals(OpCode.Callvirt))
                        {
                            if (!current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                            {
                                resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                            }
                            break;
                        }
                    }
                }
                for (int i = method.Instructions.Count - 1; i >= 0; i--)
                {
                    if (method.Instructions[i].OpCode.Equals(OpCode.Callvirt))
                    {
                        if (!method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                        {
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                        }
                        return;
                    }
                    if (method.Instructions[i].OpCode.Equals(OpCode.Call))
                    {
                        resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPDiagnosticsServiceVisitEventReceiverMethod" + exception.Message);
            }
        }

        public void VisitFeatureReceiverClass(ClassNode classType)
        {
            Method method = null;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    if (method.Name.Name.Equals("FeatureActivated") || method.Name.Name.Equals("FeatureDeactivating"))
                    {
                        this.VisitFeatureReceiverMethod(method);
                    }
                }
            }
        }

        public void VisitFeatureReceiverMethod(Method method)
        {
            Resolution resolution = null;
            try
            {
                Problem problem;
                MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Instruction current = enumerator.Current;
                    if (null != current.Value)
                    {
                        if (current.OpCode.Equals(OpCode.Call))
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local"))
                            {
                                continue;
                            }
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                            break;
                        }
                        if (current.OpCode.Equals(OpCode.Callvirt))
                        {
                            if (!current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                            {
                                resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                            }
                            break;
                        }
                    }
                }
                for (int i = method.Instructions.Count - 1; i >= 0; i--)
                {
                    if (method.Instructions[i].OpCode.Equals(OpCode.Callvirt))
                    {
                        if (!method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                        {
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                        }
                        return;
                    }
                    if (method.Instructions[i].OpCode.Equals(OpCode.Call))
                    {
                        resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of Function {0}", new string[] { method.FullName });
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
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPDiagnosticsServiceVisitFeatureReceiverMethod" + exception.Message);
            }
        }

        public void VisitTimerJobClass(ClassNode classType)
        {
            Method method = null;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    if (method.Name.Name.Equals("Execute"))
                    {
                        this.VisitTimerJobMethod(method);
                    }
                }
            }
        }

        public void VisitTimerJobMethod(Method method)
        {
            Resolution resolution = null;
            try
            {
                Problem problem;
                MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Instruction current = enumerator.Current;
                    if (null != current.Value)
                    {
                        if (current.OpCode.Equals(OpCode.Call))
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local"))
                            {
                                continue;
                            }
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of 'Execute' Function in {0}", new string[] { method.FullName });
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
                            break;
                        }
                        if (current.OpCode.Equals(OpCode.Callvirt))
                        {
                            if (!current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                            {
                                resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of 'Execute' Function in {0}", new string[] { method.FullName });
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
                            }
                            break;
                        }
                    }
                }
                for (int i = method.Instructions.Count - 1; i >= 0; i--)
                {
                    if (method.Instructions[i].OpCode.Equals(OpCode.Callvirt))
                    {
                        if (!method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                        {
                            resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of 'Execute' Function in {0}", new string[] { method.FullName });
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
                        }
                        return;
                    }
                    if (method.Instructions[i].OpCode.Equals(OpCode.Call))
                    {
                        resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of 'Execute' Function in {0}", new string[] { method.FullName });
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
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPDiagnosticsServiceVisitTimerJobMethod" + exception.Message);
            }
        }

        public void VisitWebServiceClass(ClassNode classType)
        {
            Method method = null;
            Resolution resolution = null;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    for (int i = 0; i < method.Instructions.Count; i++)
                    {
                        if (((null != method.Instructions[i].Value) && (method.Instructions[i].OpCode.Equals(OpCode.Call) || method.Instructions[i].OpCode.Equals(OpCode.Callvirt))) && ((method.Instructions[i].Value.ToString().Contains("System.Data.Common.DbConnection.Open") || method.Instructions[i].Value.ToString().Contains("System.Data.SqlClient")) || method.Instructions[i].Value.ToString().Contains("System.Web.Services.Protocols.SoapHttpClientProtocol.Invoke")))
                        {
                            Problem problem;
                            for (int j = i; j >= 0; j--)
                            {
                                if (method.Instructions[j].OpCode.Equals(OpCode.Callvirt))
                                {
                                    if (!method.Instructions[j].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                                    {
                                        resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of every Web Service call - {0}", new string[] { method.FullName });
#if ORIGINAL
                                        problem = new Problem(resolution, method.Instructions[j].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                        problem = new Problem(resolution, method.Instructions[j], Convert.ToString(this.iStringIdForProblem));
#endif
                                        problem.Certainty = 90;
                                        problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                        problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                         base.Problems.Add(problem);
                                        this.iStringIdForProblem++;
                                    }
                                    break;
                                }
                                if (j == 1)
                                {
                                    resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of every Web Service call - {0}", new string[] { method.FullName });
#if ORIGINAL
                                    problem = new Problem(resolution, method.Instructions[j].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                    problem = new Problem(resolution, method.Instructions[j], Convert.ToString(this.iStringIdForProblem));
#endif
                                    problem.Certainty = 90;
                                    problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                    problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                     base.Problems.Add(problem);
                                    this.iStringIdForProblem++;
                                    break;
                                }
                            }
                            for (int k = i; k >= method.Instructions.Count; k++)
                            {
                                if (method.Instructions[k].OpCode.Equals(OpCode.Callvirt))
                                {
                                    if (!method.Instructions[k].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                                    {
                                        resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of every Web Service call - {0}", new string[] { method.FullName });
#if ORIGINAL
                                        problem = new Problem(resolution, method.Instructions[k].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                        problem = new Problem(resolution, method.Instructions[k], Convert.ToString(this.iStringIdForProblem));
#endif
                                        problem.Certainty = 90;
                                        problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                        problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                         base.Problems.Add(problem);
                                        this.iStringIdForProblem++;
                                    }
                                    break;
                                }
                                if (k == (method.Instructions.Count - 1))
                                {
                                    resolution = new Resolution("Please add SPDiagnostics WriteTrace call at start and end of every Web Service call - {0}", new string[] { method.FullName });
#if ORIGINAL
                                    problem = new Problem(resolution, method.Instructions[k].SourceContext, Convert.ToString(this.iStringIdForProblem));
#else
                                    problem = new Problem(resolution, method.Instructions[k], Convert.ToString(this.iStringIdForProblem));
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
            }
        }
    }
}
