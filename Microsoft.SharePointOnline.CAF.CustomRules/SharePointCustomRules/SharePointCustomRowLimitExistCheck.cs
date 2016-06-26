namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;

    public class SharePointCustomRowLimitExistCheck : BaseIntrospectionRule
    {
        private List<SPQueryObject> m_listSPQueryObject;

        public SharePointCustomRowLimitExistCheck() : base("SharePointCustomRowLimitExistCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRowLimitExistCheck).Assembly)
        {
            this.m_listSPQueryObject = null;
            this.m_listSPQueryObject = new List<SPQueryObject>();
            Logging.Initialize(CustomRulesResource.SPQueryRowLimitRuleName);
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            int num = 0;
            Resolution namedResolution = null;
            try
            {
                foreach (SPQueryObject obj2 in this.m_listSPQueryObject)
                {
                    if ((!obj2.VerifyRowLimitProperty && !obj2.IsProblemAdded) && obj2.IsStatic)
                    {
                        num++;
                        if (obj2.MethodName.Equals(string.Empty))
                        {
                            namedResolution = this.GetNamedResolution("RowLimitForMemberObject", new string[] { obj2.ObjectName, obj2.ClassName, obj2.NameSpaceName });
                            base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                            obj2.IsProblemAdded = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:Check(ModuleNode) -  ") + exception.Message + exception.InnerException.Message);
            }
            return base.Problems;
        }

        public override ProblemCollection Check(TypeNode type)
        {
            try
            {
                if (type is ClassNode)
                {
                    this.VisitMembers(type.Members);
                    int num = 0;
                    Resolution namedResolution = null;
                    foreach (SPQueryObject obj2 in this.m_listSPQueryObject)
                    {
                        if ((!obj2.VerifyRowLimitProperty && !obj2.IsProblemAdded) && !obj2.IsStatic)
                        {
                            num++;
                            if (obj2.MethodName.Equals(string.Empty))
                            {
                                namedResolution = this.GetNamedResolution("RowLimitForMemberObject", new string[] { obj2.ObjectName, obj2.ClassName, obj2.NameSpaceName });
                            }
                            else
                            {
                                namedResolution = this.GetNamedResolution("RowLimitForLocalObject", new string[] { obj2.ObjectName, obj2.MethodName, obj2.ClassName });
                            }
                            base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                            obj2.IsProblemAdded = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:Check(TypeNode) - ") + exception.Message + exception.InnerException.Message);
            }
            return base.Problems;
        }

        private void CheckIfObjectExistAndSetRowLimitVerifyProperty(string sObjectType, string sObjectName, string sMethodName, string sObjectValue)
        {
            try
            {
                foreach (SPQueryObject obj2 in this.m_listSPQueryObject)
                {
                    if (obj2.ObjectType.Equals(sObjectType) && obj2.ObjectName.Equals(sObjectName))
                    {
                        if (obj2.MethodName.Equals(string.Empty))
                        {
                            obj2.MethodName = sMethodName;
                        }
                        if (obj2.MethodName.Equals(sMethodName))
                        {
                            obj2.VerifyRowLimitProperty = true;
                            obj2.ObjectValue = sObjectValue;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:CheckIfObjectExistAndSetRowLimitVerifyProperty() - " + exception.Message);
            }
        }

        private void FillSPQueryObjectList(Field field)
        {
            string str;
            bool flag = false;
            try
            {
                SPQueryObject current;
                using (List<SPQueryObject>.Enumerator enumerator = this.m_listSPQueryObject.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if (((current.ObjectType.Equals(field.Type.FullName) && current.ObjectName.Equals(field.Name.Name)) && current.MethodName.Equals(string.Empty)) && current.ClassName.Equals(field.DeclaringType.FullName))
                        {
                            flag = true;
                            goto Label_00A9;
                        }
                    }
                }
            Label_00A9:
                if (!flag)
                {
                    current = new SPQueryObject {
                        ObjectType = field.Type.FullName,
                        ObjectName = field.Name.Name,
                        ClassName = field.DeclaringType.FullName,
                        NameSpaceName = field.DeclaringType.Namespace.Name,
                        IsStatic = field.IsStatic
                    };
                    this.m_listSPQueryObject.Add(current);
                }
            }
            catch (NullReferenceException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:FillSPQueryObjectList() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:FillSPQueryObjectList() - " + exception2.Message);
            }
        }

        private void FillSPQueryObjectList(Method method)
        {
            if (method.Locals != null)
            {
                this.FillSPQueryObjectListForLocals(method);
            }
            if (method.Parameters != null)
            {
                this.FillSPQueryObjectListForParameters(method);
            }
        }

        private void FillSPQueryObjectListForLocals(Method method)
        {
            try
            {
                for (short i = 0; i < method.Locals.Count; i = (short) (i + 1))
                {
                    SPQueryObject current;
                    Local local = method.Locals[i];
                    if (!local.Type.FullName.Equals("Microsoft.SharePoint.SPQuery") || RuleUtilities.IsCompilerGenerated(local))
                    {
                        continue;
                    }
                    bool flag = false;
                    using (List<SPQueryObject>.Enumerator enumerator = this.m_listSPQueryObject.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            if (((current.ObjectType.Equals(local.Type.FullName) && current.ObjectName.Equals(local.Name.Name)) && current.ClassName.Equals(method.DeclaringType.FullName)) && current.MethodName.Equals(method.Name.Name))
                            {
                                flag = true;
                                goto Label_00F6;
                            }
                        }
                    }
                Label_00F6:
                    if (!flag)
                    {
                        current = new SPQueryObject {
                            ObjectType = local.Type.FullName,
                            ObjectName = local.Name.Name,
                            MethodName = method.Name.Name,
                            ClassName = method.DeclaringType.FullName
                        };
                        this.m_listSPQueryObject.Add(current);
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:FillSPQueryObjectListForLocals() - " + exception.Message);
            }
        }

        private void FillSPQueryObjectListForParameters(Method method)
        {
            try
            {
                for (short i = 0; i < method.Parameters.Count; i = (short) (i + 1))
                {
                    SPQueryObject current;
                    Parameter parameter = method.Parameters[i];
                    if (!parameter.Type.FullName.Equals("Microsoft.SharePoint.SPQuery"))
                    {
                        continue;
                    }
                    bool flag = false;
                    using (List<SPQueryObject>.Enumerator enumerator = this.m_listSPQueryObject.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            current = enumerator.Current;
                            if (((current.ObjectType.Equals(parameter.Type.FullName) && current.ObjectName.Equals(parameter.Name.Name)) && current.ClassName.Equals(method.DeclaringType.FullName)) && current.MethodName.Equals(method.Name.Name))
                            {
                                flag = true;
                                goto Label_00E6;
                            }
                        }
                    }
                Label_00E6:
                    if (!flag)
                    {
                        current = new SPQueryObject {
                            ObjectType = parameter.Type.FullName,
                            ObjectName = parameter.Name.Name,
                            MethodName = method.Name.Name,
                            ClassName = method.DeclaringType.FullName
                        };
                        this.m_listSPQueryObject.Add(current);
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:FillSPQueryObjectListForParameters() - " + exception.Message);
            }
        }

        private void ParseMethod(Method method)
        {
            string str;
            try
            {
                for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                {
                    if (method.Instructions.Count > (i + 1))
                    {
                        Instruction instruction = method.Instructions[i + 1];
                        Instruction instruction2 = method.Instructions[i];
                        if ((((instruction != null) && (null != instruction.Value)) && instruction.Value.ToString().Contains("SPQuery.set_RowLimit")) && (i > 0))
                        {
                            Instruction instruction3 = method.Instructions[i - 1];
                            if (null != instruction3)
                            {
                                if (instruction3.OpCode.ToString().Contains(OpCode.Ldarg.ToString()))
                                {
                                    instruction3 = method.Instructions[i - 2];
                                }
                                if ((instruction3.Value is Local) && ((instruction3.Value as Local).NodeType == NodeType.Local))
                                {
                                    Local local = instruction3.Value as Local;
                                    if (null != local)
                                    {
                                        this.CheckIfObjectExistAndSetRowLimitVerifyProperty(local.Type.FullName, local.Name.ToString(), method.Name.Name, instruction2.Value.ToString());
                                    }
                                }
                                else if ((instruction3.OpCode == OpCode.Ldfld) || (instruction3.OpCode == OpCode.Ldsfld))
                                {
                                    Field field = instruction3.Value as Field;
                                    if (null != field)
                                    {
                                        this.CheckIfObjectExistAndSetRowLimitVerifyProperty(field.Type.FullName, field.Name.ToString(), method.Name.Name, instruction2.Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:ParseMethod() - " + exception.Message);
            }
            catch (IndexOutOfRangeException exception2)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:ParseMethod() - " + exception2.Message);
            }
            catch (Exception exception3)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:ParseMethod() - " + exception3.Message);
            }
        }

        public override void VisitMembers(MemberCollection members)
        {
            try
            {
                MetadataCollection<Member>.Enumerator enumerator = members.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Member current = enumerator.Current;
                    Field field = current as Field;
                    if (null != field)
                    {
                        if (field.Type.FullName.Equals("Microsoft.SharePoint.SPQuery"))
                        {
                            this.FillSPQueryObjectList(field);
                        }
                    }
                    else
                    {
                        Method method = current as Method;
                        if (null != method)
                        {
                            this.FillSPQueryObjectList(method);
                            this.ParseMethod(method);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitExistCheck:VisitMembers() - " + exception.Message);
            }
            base.VisitMembers(members);
        }
    }
}

