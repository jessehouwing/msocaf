namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
using System.Runtime.CompilerServices;

    public class SharePointFeatureReceiverCheck : BaseIntrospectionRule
    {
        private static List<string> fullyQualifiedResolutionStrings = new List<string>();
        private Dictionary<string, Method> methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly;
        private static Random problemIdGenerator;
        private static object SyncObject = new object();

        public SharePointFeatureReceiverCheck() : base("SharePointFeatureReceiverCheck", "SharePointCustomRules.CustomRules", typeof(SharePointFeatureReceiverCheck).Assembly)
        {
            this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly = new Dictionary<string, Method>();
            problemIdGenerator = new Random(1);
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            if (module is AssemblyNode)
            {
                MetadataCollection<TypeNode>.Enumerator enumerator = module.Types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeNode current = enumerator.Current;
                    if (((current is ClassNode) && (null != current.BaseType)) && current.BaseType.FullName.Equals("Microsoft.SharePoint.SPFeatureReceiver"))
                    {
                        this.VisitFeatureReceiverClass(current as ClassNode);
                    }
                }
            }
            return base.Problems;
        }

        private bool CheckInstructionValue(Instruction instruction)
        {
            string str = instruction.Value.ToString();
            bool flag = false;
            if (!"Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace".Equals(str))
            {
                Method method = instruction.Value as Method;
                if ((method == null) || (method.Instructions.Count <= 0))
                {
                    return flag;
                }
                if (!this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
                {
                    if (this.VisitMethodInCatch(method))
                    {
                        flag = true;
                    }
                    return flag;
                }
            }
            return true;
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

        private bool HasProblem(Method method)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            int num = 0;
            int num2 = 0;
            MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                switch (current.OpCode)
                {
                    case OpCode._Locals:
                    case OpCode.Nop:
                    case OpCode.Ret:
                        break;

                    case OpCode._Try:
                        if (num2 == 0)
                        {
                            num++;
                        }
                        num2++;
                        break;

                    case OpCode._EndTry:
                    case OpCode._EndFilter:
                    case OpCode._EndHandler:
                    case OpCode.Endfinally:
                        num2--;
                        break;

                    case OpCode._Filter:
                    case OpCode._Finally:
                        num2++;
                        break;

                    case OpCode._Catch:
                        if (num2 == 0)
                        {
                            flag3 = true;
                        }
                        num2++;
                        break;

                    default:
                        flag = true;
                        if (num2 == 0)
                        {
                            flag2 = true;
                        }
                        break;
                }
            }
            bool flag4 = false;
            if (flag && ((!flag3 || flag2) || (num > 1)))
            {
                flag4 = true;
            }
            return flag4;
        }

        private bool MethodFromMicrosoftAssemblies(Method method)
        {
            bool flag = false;
            if (method != null)
            {
                if (method.DeclaringType == null)
                {
                    return flag;
                }
                if (method.DeclaringType.DeclaringModule == null)
                {
                    return flag;
                }
                if (method.DeclaringType.DeclaringModule.ContainingAssembly == null)
                {
                    return flag;
                }
                if (method.DeclaringType.DeclaringModule.ContainingAssembly.StrongName == null)
                {
                    return flag;
                }
                string strongName = method.DeclaringType.DeclaringModule.ContainingAssembly.StrongName;
                string[] separator = new string[] { "," };
                string[] strArray2 = new string[] { "=" };
                string[] strArray3 = strongName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str2 in strArray3)
                {
                    string[] source = str2.Split(strArray2, StringSplitOptions.RemoveEmptyEntries);
                    if ((((source.Count<string>() > 0) && (source.Count<string>() == 2)) && source[0].ToLower(CultureInfo.InvariantCulture).Trim().Equals("publickeytoken")) && ((((source[1] == "71e9bce111e9429c") || (source[1] == "31bf3856ad364e35")) || (source[1] == "b77a5c561934e089")) || (source[1] == "b03f5f7f11d50a3a")))
                    {
                        return true;
                    }
                }
            }
            return flag;
        }

        public void VisitFeatureReceiverClass(ClassNode classType)
        {
            Method method = null;
            string str = string.Empty;
            bool flag = false;
            MetadataCollection<Member>.Enumerator enumerator = classType.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Member current = enumerator.Current;
                if (null != (current as Method))
                {
                    method = current as Method;
                    if (((method.Name.Name.Equals("FeatureActivated") || method.Name.Name.Equals("FeatureDeactivating")) || (method.Name.Name.Equals("FeatureInstalled") || method.Name.Name.Equals("FeatureUninstalling"))) || method.Name.Name.Equals("FeatureUpgrading"))
                    {
                        if (this.HasProblem(method))
                        {
                            Problem problem;
                            flag = false;
                            Resolution resolution = new Resolution("The method {0} does not implement a top level try catch block", new string[] { this.GetOnlyTheMethodName(method.FullName) });
                            if (string.IsNullOrEmpty(method.SourceContext.FileName))
                            {
                                str = "'[symbols not found to locate the line number]'";
                            }
                            else
                            {
                                str = method.SourceContext.StartLine.ToString();
                                flag = true;
                            }
                            if (flag)
                            {
                                if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                                {
#if ORIGINAL
                                    problem = new Problem(resolution, method.SourceContext);
#else
                                    problem = new Problem(resolution, method);
#endif
                                    problem.Certainty = 90;
                                    problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                    problem.Id = this.GetNextId();
                                    problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                     base.Problems.Add(problem);
                                    fullyQualifiedResolutionStrings.Add(resolution.ToString());
                                }
                            }
                            else
                            {
#if ORIGINAL
                                problem = new Problem(resolution, method.SourceContext);
#else
                                problem = new Problem(resolution, method);
#endif
                                problem.Certainty = 90;
                                problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                problem.Id = this.GetNextId();
                                problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                 base.Problems.Add(problem);
                            }
                        }
                        this.VisitFeatureReceiverMethod(method);
                    }
                }
            }
        }

        public void VisitFeatureReceiverMethod(Method method)
        {
            try
            {
                if (Enumerable.Any(method.Instructions, i => (i.OpCode == OpCode._Catch)))
                {
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    int startLine = 0;
                    int num2 = -1;
                    int num3 = 0;
                    int num4 = 0;
#if ORIGINAL
                    Stack<SourceContext> stack = new Stack<SourceContext>();
#else
                    Stack<Node> stack = new Stack<Node>();
#endif

                    MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Instruction current = enumerator.Current;
                        num2++;
                        if (current.OpCode == OpCode._Catch)
                        {
                            flag = true;
                            num3++;
#if ORIGINAL
                            stack.Push(current.SourceContext);
#else
                            stack.Push(current);
#endif
                        }
                        else if (flag)
                        {
                            if (current.OpCode == OpCode._EndHandler)
                            {
                                flag2 = true;
                                Instruction instruction2 = method.Instructions[num2 - 1];
                                startLine = instruction2.SourceContext.StartLine;
                                num4++;
                            }
                            if (!flag2)
                            {
                                if (((current.OpCode == OpCode.Call) || (current.OpCode == OpCode.Callvirt)) && this.CheckInstructionValue(current))
                                {
                                    flag4 = true;
                                }
                                if ((current.OpCode == OpCode.Throw) || (current.OpCode == OpCode.Rethrow))
                                {
                                    flag3 = true;
                                    flag2 = true;
                                    num4++;
                                }
                            }
                            if (flag2)
                            {
                                string str;
                                bool flag5;
                                Resolution resolution;
                                Problem problem;
#if ORIGINAL
                                SourceContext context = stack.Pop();
#else
                                Node context = stack.Pop();
#endif

                                if (!flag4)
                                {
                                    str = string.Empty;
                                    flag5 = false;
#if ORIGINAL
                                    if (string.IsNullOrEmpty(context.FileName))
#else
                                    if (string.IsNullOrEmpty(context.SourceContext.FileName))
#endif
                                    {
                                        str = "'[symbols not found to locate the line number]'";
                                    }
                                    else
                                    {
#if ORIGINAL
                                        str = context.StartLine.ToString();
#else
                                        str = context.SourceContext.StartLine.ToString();
#endif
                                        flag5 = true;
                                    }
                                    resolution = new Resolution("The catch block at line number {0} in method {1} must log to ULS Logs. For ULS logging, please use the Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.", new string[] { str, method.FullName });
                                    if (flag5)
                                    {
                                        if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                                        {
                                            problem = new Problem(resolution, context);
                                            problem.Certainty = 90;
                                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                            problem.Id = this.GetNextId();
                                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                             base.Problems.Add(problem);
                                            fullyQualifiedResolutionStrings.Add(resolution.ToString());
                                        }
                                    }
                                    else
                                    {
                                        problem = new Problem(resolution, context);
                                        problem.Certainty = 90;
                                        problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                        problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                         base.Problems.Add(problem);
                                    }
                                }
                                if (!flag3)
                                {
                                    str = string.Empty;
                                    flag5 = false;
#if ORIGINAL
                                    if (string.IsNullOrEmpty(context.FileName))
#else
                                    if (string.IsNullOrEmpty(context.SourceContext.FileName))
#endif
                                    {
                                        str = "'[symbols not found to locate the line number]'";
                                    }
                                    else
                                    {
#if ORIGINAL
                                        str = context.StartLine.ToString();
#else
                                        str = context.SourceContext.StartLine.ToString();
#endif
                                        flag5 = true;
                                    }
                                    resolution = new Resolution("The catch block at line number {0} in method {1} must throw the exception back to SharePoint.", new string[] { str, method.FullName });
                                    if (flag5)
                                    {
                                        if (!fullyQualifiedResolutionStrings.Contains(resolution.ToString()))
                                        {
                                            problem = new Problem(resolution, context);
                                            problem.Certainty = 90;
                                            problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                            problem.Id = this.GetNextId();
                                            problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                             base.Problems.Add(problem);
                                            fullyQualifiedResolutionStrings.Add(resolution.ToString());
                                        }
                                    }
                                    else
                                    {
                                        problem = new Problem(resolution, context);
                                        problem.Certainty = 90;
                                        problem.FixCategory = Microsoft.VisualStudio.CodeAnalysis.Extensibility.FixCategories.NonBreaking;
                                        problem.MessageLevel = Microsoft.VisualStudio.CodeAnalysis.Extensibility.MessageLevel.Warning;
                                         base.Problems.Add(problem);
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
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPFeatureReceiverMethod" + exception.Message);
            }
        }

        private bool VisitMethodInCatch(Method method, Stack<string> callstack)
        {
            if (this.MethodFromMicrosoftAssemblies(method) || method.IsSpecialName)
            {
                return false;
            }

            if (callstack.Contains(this.GetOnlyTheMethodName(method.FullName)) || callstack.Count > 32)
            {
                return false;
            }
            else
            {
                callstack.Push(this.GetOnlyTheMethodName(method.FullName));
            }

            bool flag = false;
            MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                if (current.OpCode == OpCode.Callvirt)
                {
                    if (current.Value.ToString().Equals("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                    {
                        flag = true;
                        break;
                    }
                }
                else if (current.OpCode == OpCode.Call)
                {
                    Method method2 = current.Value as Method;
                    if (!(method2 == null || method2.Instructions.Count <= 0 || method.IsSpecialName || this.MethodFromMicrosoftAssemblies(method2)))
                    {
                        flag = this.VisitMethodInCatch(method2, callstack);
                        if (flag)
                        {
                            break;
                        }
                    }
                    else if (current.Value.ToString().Equals("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag && !this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
            {
                this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
            }
            return flag;
        }
        
        private bool VisitMethodInCatch(Method method)
        {
            return VisitMethodInCatch(method, new Stack<string>());
        }
    }
}
