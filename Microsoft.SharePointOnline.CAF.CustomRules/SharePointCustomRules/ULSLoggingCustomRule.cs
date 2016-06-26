namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using Microsoft.VisualStudio.CodeAnalysis.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ULSLoggingCustomRule : BaseIntrospectionRule
    {
        private static List<string> fullyQualifiedResolutionStrings = new List<string>();
        private Dictionary<string, Method> methodsThatCallOneOfTraceEventMethods;
        private Dictionary<string, Method> methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly;
        private Dictionary<string, Method> methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly;
        private Dictionary<string, Method> methodsThatCallUnsupportedMethodsDirectly;
        private Dictionary<string, Method> methodsThatCallUnsupportedMethodsIndirectly;
        private Dictionary<string, Method> methodsToProcessFinally;
        private Dictionary<string, Method> otherInitialPassMethods;
        private static Random problemIdGenerator;
        private static object SyncObject = new object();
        private static object SyncObjectForProblemAddition = new object();
        private Dictionary<string, Method> traceEventMethodDeclarations;

        public ULSLoggingCustomRule() : base("ULSLoggingCustomRule", "SharePointCustomRules.CustomRules", typeof(ULSLoggingCustomRule).Assembly)
        {
            this.traceEventMethodDeclarations = new Dictionary<string, Method>();
            this.methodsThatCallOneOfTraceEventMethods = new Dictionary<string, Method>();
            this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly = new Dictionary<string, Method>();
            this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly = new Dictionary<string, Method>();
            this.methodsThatCallUnsupportedMethodsDirectly = new Dictionary<string, Method>();
            this.methodsThatCallUnsupportedMethodsIndirectly = new Dictionary<string, Method>();
            this.otherInitialPassMethods = new Dictionary<string, Method>();
            this.methodsToProcessFinally = new Dictionary<string, Method>();
            problemIdGenerator = new Random(1);
        }

        public override void BeforeAnalysis()
        {
            fullyQualifiedResolutionStrings.Clear();
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            if (module is AssemblyNode)
            {
                Resolution resolution;
                Problem problem;
                MetadataCollection<TypeNode>.Enumerator enumerator = ((AssemblyNode) module).Types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeNode current = enumerator.Current;
                    this.Check(current);
                }
                this.PopulateCallers();
                foreach (Method method in this.methodsToProcessFinally.Values)
                {
                    if (method.Instructions.Any<Instruction>(t => t.OpCode == OpCode._Catch))
                    {
                        bool flag = false;
                        bool flag2 = false;
                        bool flag4 = false;
                        int startLine = 0;
                        int num2 = -1;
                        int num3 = 0;
                        int num4 = 0;
                        Stack<SourceContext> stack = new Stack<SourceContext>();
                        MetadataCollection<Instruction>.Enumerator enumerator3 = method.Instructions.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            Instruction instruction = enumerator3.Current;
                            num2++;
                            if (instruction.OpCode == OpCode._Catch)
                            {
                                flag = true;
                                num3++;
                                stack.Push(instruction.SourceContext);
                            }
                            else if (flag)
                            {
                                if (instruction.OpCode == OpCode._EndHandler)
                                {
                                    flag2 = true;
                                    Instruction instruction2 = method.Instructions[num2 - 1];
                                    startLine = instruction2.SourceContext.StartLine;
                                    num4++;
                                }
                                if (!flag2)
                                {
                                    if ((instruction.OpCode == OpCode.Call) && this.CheckInstructionValue(instruction))
                                    {
                                        flag4 = true;
                                        flag2 = true;
                                        num4++;
                                    }
                                    if ((instruction.OpCode == OpCode.Throw) || (instruction.OpCode == OpCode.Rethrow))
                                    {
                                        flag4 = true;
                                        flag2 = true;
                                        num4++;
                                    }
                                }
                                if (flag2)
                                {
                                    SourceContext sourceContext = stack.Pop();
                                    lock (SyncObjectForProblemAddition)
                                    {
                                        if (!flag4)
                                        {
                                            string str = string.Empty;
                                            bool flag5 = false;
                                            if (string.IsNullOrEmpty(sourceContext.FileName))
                                            {
                                                str = "'[symbols not found to locate the line number]'";
                                            }
                                            else
                                            {
                                                str = sourceContext.StartLine.ToString();
                                                flag5 = true;
                                            }
                                            resolution = new Resolution("The catch block at line number {0} in method {1} must log to ULS Logs. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.", new string[] { str, method.FullName });
                                            if (flag5)
                                            {
                                                if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                                                {
                                                    problem = new Problem(resolution, sourceContext) {
                                                        Certainty = 90,
                                                        FixCategory = FixCategories.NonBreaking,
                                                        Id = this.GetNextId(),
                                                        MessageLevel = MessageLevel.Warning
                                                    };
                                                    base.Problems.Add(problem);
                                                    fullyQualifiedResolutionStrings.Add(resolution.ToString());
                                                }
                                            }
                                            else
                                            {
                                                problem = new Problem(resolution, sourceContext) {
                                                    Certainty = 90,
                                                    FixCategory = FixCategories.NonBreaking,
                                                    Id = this.GetNextId(),
                                                    MessageLevel = MessageLevel.Warning
                                                };
                                                base.Problems.Add(problem);
                                            }
                                        }
                                    }
                                    if (num3 == num4)
                                    {
                                        flag = false;
                                    }
                                    flag2 = false;
                                    flag4 = false;
                                }
                            }
                        }
                    }
                }
                foreach (Method method in this.methodsThatCallUnsupportedMethodsIndirectly.Values)
                {
                    resolution = new Resolution("Method '{0}' calls one or both of the unsupported APIs '{1}' and '{2}' indirectly. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.", new string[] { this.GetOnlyTheMethodName(method.FullName), "Microsoft.Office.Server.Diagnostics.PortalLog.LogString", "Microsoft.Office.Server.Diagnostics.PortalLog.DebugLogString" });
                    problem = new Problem(resolution, method.SourceContext) {
                        Certainty = 90,
                        FixCategory = FixCategories.NonBreaking,
                        Id = this.GetNextId(),
                        MessageLevel = MessageLevel.Warning
                    };
                    base.Problems.Add(problem);
                }
            }
            return base.Problems;
        }

        public override ProblemCollection Check(TypeNode type)
        {
            MetadataCollection<Member>.Enumerator enumerator = type.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (current is Method)
                {
                    this.VisitMethod((Method) current);
                }
            }
            return base.Problems;
        }

        private bool CheckInstructionValue(Instruction instruction)
        {
            string str = instruction.Value.ToString();
            return (((this.traceEventMethodDeclarations.Keys.Contains<string>(str) || this.methodsThatCallOneOfTraceEventMethods.Keys.Contains<string>(str)) || (this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Keys.Contains<string>(str) || this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly.Keys.Contains<string>(str))) || "Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace".Equals(str));
        }

        private bool DoesMethodCallSP2010Method(Method method)
        {
            MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                if ((current.OpCode == OpCode.Call) && current.Value.ToString().Equals("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                {
                    return true;
                }
            }
            return false;
        }

        private bool DoesMethodCallUnsupportedMethods(Method method)
        {
            if (this.methodsThatCallUnsupportedMethodsDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
            {
                return true;
            }
            bool flag = false;
            MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                if ((current.OpCode == OpCode.Call) && (current.Value.ToString().Equals("Microsoft.Office.Server.Diagnostics.PortalLog.LogString") || current.Value.ToString().Equals("Microsoft.Office.Server.Diagnostics.PortalLog.DebugLogString")))
                {
                    string str = string.Empty;
                    bool flag2 = false;
                    if (string.IsNullOrEmpty(current.SourceContext.FileName))
                    {
                        str = "'[symbols not found to locate the line number]'";
                    }
                    else
                    {
                        str = current.SourceContext.StartLine.ToString();
                        flag2 = false;
                    }
                    Resolution resolution = new Resolution("Method '{0}' calls '{1}' at line number {2} which is unsupported. For ULS logging, please use the TraceEvent sample on MSDN or Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.", new string[] { this.GetOnlyTheMethodName(method.FullName), current.Value.ToString(), str });
                    lock (SyncObjectForProblemAddition)
                    {
                        Problem problem;
                        if (flag2)
                        {
                            if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                            {
                                problem = new Problem(resolution, current.SourceContext) {
                                    Certainty = 90,
                                    FixCategory = FixCategories.NonBreaking,
                                    Id = this.GetNextId(),
                                    MessageLevel = MessageLevel.Warning
                                };
                                base.Problems.Add(problem);
                                fullyQualifiedResolutionStrings.Add(resolution.ToString());
                            }
                        }
                        else
                        {
                            problem = new Problem(resolution, current.SourceContext) {
                                Certainty = 90,
                                FixCategory = FixCategories.NonBreaking,
                                Id = this.GetNextId(),
                                MessageLevel = MessageLevel.Warning
                            };
                            base.Problems.Add(problem);
                        }
                        flag = true;
                    }
                }
            }
            return flag;
        }

        private void FindCallersOfTraceEventAPI(Method method)
        {
            MetadataCollection<Method>.Enumerator enumerator = CallGraph.CallersFor(method).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Method current = enumerator.Current;
                this.FindCallersOfTraceEventAPI(current);
            }
            if (!this.methodsThatCallOneOfTraceEventMethods.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
            {
                this.methodsThatCallOneOfTraceEventMethods.Add(this.GetOnlyTheMethodName(method.FullName), method);
            }
        }

        private void FindIndirectCallersOfSP2010LoggingAPI(Method method)
        {
            MetadataCollection<Method>.Enumerator enumerator = CallGraph.CallersFor(method).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Method current = enumerator.Current;
                this.FindIndirectCallersOfSP2010LoggingAPI(current);
            }
            if (!(this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)) || this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName))))
            {
                this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
            }
        }

        private void FindIndirectCallersOfUnsupportedMethod(Method method)
        {
            MetadataCollection<Method>.Enumerator enumerator = CallGraph.CallersFor(method).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Method current = enumerator.Current;
                this.FindIndirectCallersOfUnsupportedMethod(current);
            }
            if (!(this.methodsThatCallUnsupportedMethodsDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)) || this.methodsThatCallUnsupportedMethodsIndirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName))))
            {
                this.methodsThatCallUnsupportedMethodsIndirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
            }
        }

        private string GetNextId()
        {
            lock (SyncObject)
            {
                return problemIdGenerator.Next().ToString();
            }
        }

        private string GetOnlyTheMethodName(string fullMethodName)
        {
            int index = fullMethodName.IndexOf('(');
            if (index > 0)
            {
                return fullMethodName.Substring(0, index);
            }
            return fullMethodName;
        }

        private void PopulateCallers()
        {
            foreach (Method method in this.traceEventMethodDeclarations.Values)
            {
                this.FindCallersOfTraceEventAPI(method);
            }
            foreach (Method method in this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Values)
            {
                this.FindIndirectCallersOfSP2010LoggingAPI(method);
            }
            foreach (Method method in this.methodsThatCallUnsupportedMethodsDirectly.Values)
            {
                this.FindIndirectCallersOfUnsupportedMethod(method);
            }
            foreach (string str in this.methodsThatCallOneOfTraceEventMethods.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.methodsThatCallOneOfTraceEventMethods[str]);
                }
            }
            foreach (string str in this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly[str]);
                }
            }
            foreach (string str in this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceIndirectly[str]);
                }
            }
            foreach (string str in this.methodsThatCallUnsupportedMethodsDirectly.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.methodsThatCallUnsupportedMethodsDirectly[str]);
                }
            }
            foreach (string str in this.methodsThatCallUnsupportedMethodsIndirectly.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.methodsThatCallUnsupportedMethodsIndirectly[str]);
                }
            }
            foreach (string str in this.otherInitialPassMethods.Keys)
            {
                if (!this.methodsToProcessFinally.ContainsKey(str))
                {
                    this.methodsToProcessFinally.Add(str, this.otherInitialPassMethods[str]);
                }
            }
        }

        public override void VisitMethod(Method method)
        {
            if (method.IsExtern)
            {
                if (method.Name.Name.ToLower().Equals("traceevent") && !this.traceEventMethodDeclarations.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
                {
                    this.traceEventMethodDeclarations.Add(this.GetOnlyTheMethodName(method.FullName), method);
                }
            }
            else
            {
                bool flag = this.DoesMethodCallUnsupportedMethods(method);
                bool flag2 = this.DoesMethodCallSP2010Method(method);
                if (!(!flag || this.methodsThatCallUnsupportedMethodsDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName))))
                {
                    this.methodsThatCallUnsupportedMethodsDirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
                }
                if (!(!flag2 || this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName))))
                {
                    this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
                }
                if (!((flag || flag2) || this.otherInitialPassMethods.ContainsKey(this.GetOnlyTheMethodName(method.FullName))))
                {
                    this.otherInitialPassMethods.Add(this.GetOnlyTheMethodName(method.FullName), method);
                }
            }
        }
    }
}

