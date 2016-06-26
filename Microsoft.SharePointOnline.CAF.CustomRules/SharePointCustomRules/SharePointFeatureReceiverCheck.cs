namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using Microsoft.VisualStudio.CodeAnalysis.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class SharePointFeatureReceiverCheck : BaseIntrospectionRule
    {
        private static List<string> fullyQualifiedResolutionStrings = new List<string>();
        private Dictionary<string, Method> methodAlreadyVisited;
        private Dictionary<string, Method> methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly;
        private static Random problemIdGenerator;
        private static object SyncObject = new object();

        public SharePointFeatureReceiverCheck() : base("SharePointFeatureReceiverCheck", "SharePointCustomRules.CustomRules", typeof(SharePointFeatureReceiverCheck).Assembly)
        {
            this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly = new Dictionary<string, Method>();
            this.methodAlreadyVisited = new Dictionary<string, Method>();
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
                    if (((current is ClassNode) && (null != current.BaseType)) && this.InHeritsFeatureReciever(current as ClassNode))
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
                    if (!this.methodAlreadyVisited.ContainsKey(method.FullName))
                    {
                        this.methodAlreadyVisited.Add(method.FullName, method);
                        if (this.VisitMethodInCatch(method))
                        {
                            flag = true;
                        }
                    }
                    return flag;
                }
            }
            return true;
        }

        private int FeatureMethodEndsWithTrace(Method method)
        {
            int num2;
            int num = method.Instructions.Count - 1;
            for (num2 = method.Instructions.Count - 1; num2 >= 0; num2--)
            {
                if (method.Instructions[num2].OpCode.Equals(OpCode.Callvirt))
                {
                    if (method.Instructions[num2].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                    {
                        num = num2;
                    }
                    else
                    {
                        num = method.Instructions.Count - 1;
                    }
                    break;
                }
                if (method.Instructions[num2].OpCode.Equals(OpCode.Call))
                {
                    num = method.Instructions.Count - 1;
                    break;
                }
                if (((method.Instructions[num2].OpCode.Equals(OpCode.Endfinally) || method.Instructions[num2].OpCode.Equals(OpCode.Endfilter)) || (method.Instructions[num2].OpCode.Equals(OpCode._EndHandler) || method.Instructions[num2].OpCode.Equals(OpCode._EndFilter))) || method.Instructions[num2].OpCode.Equals(OpCode._EndTry))
                {
                    num = method.Instructions.Count - 1;
                    break;
                }
            }
            if (num < (method.Instructions.Count - 1))
            {
                for (num2 = num - 1; num2 >= 0; num2--)
                {
                    if (method.Instructions[num2].OpCode.Equals(OpCode.Call) && method.Instructions[num2].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local"))
                    {
                        return num2;
                    }
                }
            }
            return num;
        }

        private TraceResult FeatureMethodStartsWithTrace(Method method, bool methodContainsElevatedCode)
        {
            TraceResult result = new TraceResult();
            bool flag = false;
            for (int i = 0; i <= (method.Instructions.Count - 1); i++)
            {
                if ((!methodContainsElevatedCode || ((i < 0) || (i > 7))) || (((((method.Instructions[i].OpCode != OpCode.Ldnull) && (method.Instructions[i].OpCode != OpCode.Stloc_1)) && ((method.Instructions[i].OpCode != OpCode.Newobj) && (method.Instructions[i].OpCode != OpCode.Stloc_2))) && ((method.Instructions[i].OpCode != OpCode.Ldloc_2) && (method.Instructions[i].OpCode != OpCode.Ldarg_1))) && (method.Instructions[i].OpCode != OpCode.Stfld)))
                {
                    if (null != method.Instructions[i].Value)
                    {
                        if (method.Instructions[i].OpCode.Equals(OpCode.Call))
                        {
                            if (!method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local"))
                            {
                                if (!method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.get_Areas"))
                                {
                                    result.Trace = false;
                                    return result;
                                }
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                        else
                        {
                            if (method.Instructions[i].OpCode.Equals(OpCode.Callvirt))
                            {
                                if (method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                                {
                                    result.Index = i;
                                    result.Trace = true;
                                    return result;
                                }
                                result.Index = 0;
                                result.Trace = false;
                                return result;
                            }
                            if ((!method.Instructions[i].OpCode.Equals(OpCode.Nop) && !method.Instructions[i].OpCode.Equals(OpCode._Locals)) && !flag)
                            {
                                result.Index = 0;
                                result.Trace = false;
                                return result;
                            }
                        }
                    }
                    else if (method.Instructions[i].OpCode.Equals(OpCode._Try) && !flag)
                    {
                        result.Index = 0;
                        result.Trace = false;
                        return result;
                    }
                }
            }
            return result;
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
            bool methodContainsElevatedCode = false;
            methodContainsElevatedCode = this.MethodContainsElevatedPrivilegesCode(method);
            int index = this.FeatureMethodStartsWithTrace(method, methodContainsElevatedCode).Index;
            if (index > 0)
            {
                index++;
            }
            int num4 = this.FeatureMethodEndsWithTrace(method);
            if (num4 != (method.Instructions.Count - 1))
            {
                num4--;
            }
            for (int i = index; i <= num4; i++)
            {
                switch (method.Instructions[i].OpCode)
                {
                    case OpCode.Ldloc_2:
                    case OpCode.Stloc_1:
                    case OpCode.Stloc_2:
                    case OpCode.Ldnull:
                    case OpCode.Ldarg_1:
                    case OpCode.Newobj:
                    case OpCode.Stfld:
                    {
                        if (methodContainsElevatedCode)
                        {
                            if ((i >= 0) && (i <= 7))
                            {
                                flag2 = false;
                            }
                            else if (num2 == 0)
                            {
                                flag2 = true;
                            }
                        }
                        else if (num2 == 0)
                        {
                            flag2 = true;
                        }
                        continue;
                    }
                    case OpCode.Nop:
                    case OpCode.Ret:
                    case OpCode._Locals:
                    case OpCode.Endfinally:
                    {
                        continue;
                    }
                    case OpCode._Try:
                    {
                        if (num2 == 0)
                        {
                            num++;
                        }
                        num2++;
                        continue;
                    }
                    case OpCode._EndTry:
                    case OpCode._EndFilter:
                    case OpCode._EndHandler:
                    {
                        num2--;
                        continue;
                    }
                    case OpCode._Filter:
                    case OpCode._Finally:
                    {
                        num2++;
                        continue;
                    }
                    case OpCode._Catch:
                    {
                        if (num == 1)
                        {
                            flag3 = true;
                        }
                        num2++;
                        continue;
                    }
                }
                flag = true;
                if (num2 == 0)
                {
                    flag2 = true;
                }
            }
            bool flag5 = false;
            if (flag && ((!flag3 || flag2) || (num > 1)))
            {
                flag5 = true;
            }
            return flag5;
        }

        private bool InHeritsFeatureReciever(ClassNode classToBeChecked)
        {
            while (classToBeChecked.BaseType != null)
            {
                if (classToBeChecked.BaseType.FullName.Equals("Microsoft.SharePoint.SPFeatureReceiver"))
                {
                    return true;
                }
                if (classToBeChecked.BaseType is ClassNode)
                {
                    classToBeChecked = classToBeChecked.BaseType as ClassNode;
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        private bool MethodContainsElevatedPrivilegesCode(Method mehtod)
        {
            MetadataCollection<Instruction>.Enumerator enumerator = mehtod.Instructions.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Instruction current = enumerator.Current;
                if ((current.Value != null) && current.Value.ToString().Equals("Microsoft.SharePoint.SPSecurity+CodeToRunElevated(Microsoft.FxCop.Sdk.ParameterCollection)"))
                {
                    return true;
                }
            }
            return false;
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
                                    problem = new Problem(resolution, method.SourceContext) {
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
                                problem = new Problem(resolution, method.SourceContext) {
                                    Certainty = 90,
                                    FixCategory = FixCategories.NonBreaking,
                                    Id = this.GetNextId(),
                                    MessageLevel = MessageLevel.Warning
                                };
                                base.Problems.Add(problem);
                            }
                        }
                        this.VisitFeatureReceiverMethod(method);
                    }
                }
            }
        }

        private bool VisitFeatureReceiverDiagnosis(Method method)
        {
            bool flag3;
            bool flag = true;
            bool flag2 = true;
            try
            {
                MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Instruction current = enumerator.Current;
                    if ((null != current.Value) && (!current.OpCode.Equals(OpCode._Locals) && !current.OpCode.Equals(OpCode.Nop)))
                    {
                        if (current.OpCode.Equals(OpCode.Call))
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsService.get_Local") || current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.get_Areas"))
                            {
                                continue;
                            }
                            flag = false;
                            break;
                        }
                        if (current.OpCode.Equals(OpCode.Callvirt))
                        {
                            if (!current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace"))
                            {
                                flag = false;
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
                            if (method.Instructions[i].Value.ToString().Contains("Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.get_Areas"))
                            {
                                continue;
                            }
                            flag2 = false;
                        }
                        goto Label_0255;
                    }
                    if (method.Instructions[i].OpCode.Equals(OpCode.Call))
                    {
                        flag2 = false;
                        goto Label_0255;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SPDiagnosticsServiceVisitFeatureReceiverMethod" + exception.Message);
            }
        Label_0255:
            flag3 = false;
            if (flag || flag2)
            {
                flag3 = true;
            }
            return flag3;
        }

        public void VisitFeatureReceiverMethod(Method method)
        {
            try
            {
                if (method.Instructions.Any<Instruction>(t => t.OpCode == OpCode._Catch))
                {
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    int startLine = 0;
                    int num2 = -1;
                    int num3 = 0;
                    int num4 = 0;
                    Stack<SourceContext> stack = new Stack<SourceContext>();
                    MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Instruction current = enumerator.Current;
                        num2++;
                        if (current.OpCode == OpCode._Catch)
                        {
                            flag = true;
                            num3++;
                            stack.Push(current.SourceContext);
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
                                SourceContext sourceContext = stack.Pop();
                                if (!flag4)
                                {
                                    str = string.Empty;
                                    flag5 = false;
                                    if (string.IsNullOrEmpty(sourceContext.FileName))
                                    {
                                        str = "'[symbols not found to locate the line number]'";
                                    }
                                    else
                                    {
                                        str = sourceContext.StartLine.ToString();
                                        flag5 = true;
                                    }
                                    resolution = new Resolution("The catch block at line number {0} in method {1} must log to ULS Logs. For ULS logging, please use the Microsoft.SharePoint.Administration.SPDiagnosticsServiceBase.WriteTrace API in SharePoint 2010.", new string[] { str, method.FullName });
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
                                            MessageLevel = MessageLevel.Warning
                                        };
                                        base.Problems.Add(problem);
                                    }
                                }
                                if (!flag3)
                                {
                                    str = string.Empty;
                                    flag5 = false;
                                    if (string.IsNullOrEmpty(sourceContext.FileName))
                                    {
                                        str = "'[symbols not found to locate the line number]'";
                                    }
                                    else
                                    {
                                        str = sourceContext.StartLine.ToString();
                                        flag5 = true;
                                    }
                                    resolution = new Resolution("The catch block at line number {0} in method {1} must throw the exception back to SharePoint.", new string[] { str, method.FullName });
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
                                            MessageLevel = MessageLevel.Warning
                                        };
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

        private bool VisitMethodInCatch(Method method)
        {
            bool flag = false;
            if (method.IsExtern)
            {
                if (method.Name.Name.ToLower().Equals("traceevent"))
                {
                    flag = true;
                }
            }
            else
            {
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
                        if (!((((method2 == null) || (method2.Instructions.Count <= 0)) || (method.IsSpecialName || this.MethodFromMicrosoftAssemblies(method2))) || this.methodAlreadyVisited.ContainsKey(method2.FullName)))
                        {
                            this.methodAlreadyVisited.Add(method2.FullName, method2);
                            flag = this.VisitMethodInCatch(method2);
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
            }
            if (flag && !this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.ContainsKey(this.GetOnlyTheMethodName(method.FullName)))
            {
                this.methodsThatCallSP2010SPDiagnosticsServiceWriteTraceDirectly.Add(this.GetOnlyTheMethodName(method.FullName), method);
            }
            return flag;
        }

        private class TraceResult
        {
            private int index;
            private bool trace;

            public int Index
            {
                get
                {
                    return this.index;
                }
                set
                {
                    this.index = value;
                }
            }

            public bool Trace
            {
                get
                {
                    return this.trace;
                }
                set
                {
                    this.trace = value;
                }
            }
        }
    }
}

