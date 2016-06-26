namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointTimerjobImplementationCheck : BaseIntrospectionRule
    {
        private bool m_bIsJobDeleteDefinitionPresent;
        private bool m_bIsJobExistsDefinitionPresent;
        private bool m_bIsMinuteScheduleUsed;

        public SharePointTimerjobImplementationCheck() : base("SharePointTimerjobImplementationCheck", "SharePointCustomRules.CustomRules", typeof(SharePointTimerjobImplementationCheck).Assembly)
        {
            this.m_bIsJobExistsDefinitionPresent = false;
            this.m_bIsJobDeleteDefinitionPresent = false;
            this.m_bIsMinuteScheduleUsed = false;
        }

        public override ProblemCollection Check(ModuleNode module)
        {
            string str;
            try
            {
                Resolution namedResolution = null;
                int num = 0;
                MetadataCollection<TypeNode>.Enumerator enumerator = module.Types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeNode current = enumerator.Current;
                    if (((current is ClassNode) && (null != current.BaseType)) && current.BaseType.FullName.Equals("Microsoft.SharePoint.Administration.SPJobDefinition"))
                    {
                        if (this.SearchForSPFeatureReceiverClass(module))
                        {
                            if (!this.m_bIsJobExistsDefinitionPresent)
                            {
                                namedResolution = this.GetNamedResolution("JobExistsCheck", new string[] { module.Name });
                                base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                num++;
                            }
                            if (!this.m_bIsJobDeleteDefinitionPresent)
                            {
                                namedResolution = this.GetNamedResolution("JobDeleteCheck", new string[] { module.Name });
                                base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                num++;
                            }
                            if (!this.m_bIsMinuteScheduleUsed)
                            {
                                namedResolution = this.GetNamedResolution("JobMinuteScheduleCheck", new string[] { module.Name });
                                base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                num++;
                            }
                        }
                        else
                        {
                            namedResolution = this.GetNamedResolution("JobFeatureReceiverCheck", new string[] { module.Name });
                            base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                            num++;
                        }
                    }
                    this.m_bIsJobExistsDefinitionPresent = false;
                    this.m_bIsJobDeleteDefinitionPresent = false;
                    this.m_bIsMinuteScheduleUsed = false;
                }
            }
            catch (NullReferenceException exception)
            {
                str = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointTimerjobImplementationCheck:Check() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SharePointTimerjobImplementationCheck:Check() - ") + exception2.InnerException.Message + exception2.Message);
            }
            return base.Problems;
        }

        private bool SearchForSPFeatureReceiverClass(ModuleNode module)
        {
            bool flag = false;
            try
            {
                MetadataCollection<TypeNode>.Enumerator enumerator = module.Types.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeNode current = enumerator.Current;
                    if ((((current != null) && (current is ClassNode)) && (null != current.BaseType)) && current.BaseType.FullName.Equals("Microsoft.SharePoint.SPFeatureReceiver"))
                    {
                        flag = true;
                        this.VisitClass(current as ClassNode);
                        return flag;
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointTimerjobImplementationCheck:SearchForSPFeatureReceiverClass() - " + exception.Message);
            }
            return flag;
        }

        public override void VisitClass(ClassNode classType)
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
                        this.VisitMethod(method);
                    }
                }
            }
            base.VisitClass(classType);
        }

        public override void VisitMethod(Method method)
        {
            short num = 0;
            short num2 = 0;
            try
            {
                Instruction current;
                MetadataCollection<Instruction>.Enumerator enumerator;
                if (method.Name.Name.Equals("FeatureActivated"))
                {
                    enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if (null != current.Value)
                        {
                            if (((current.OpCode.Equals(OpCode.Newobj) && current.Value.ToString().Contains("Microsoft.SharePoint.SPMinuteSchedule")) || current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPJobDefinition.set_Schedule")) || current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPPersistedObject.Update"))
                            {
                                num2 = (short) (num2 + 1);
                            }
                            if (!(!current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPWebApplication.get_JobDefinitions") || this.m_bIsJobExistsDefinitionPresent))
                            {
                                this.m_bIsJobExistsDefinitionPresent = true;
                            }
                            if (!((num2 != 3) || this.m_bIsMinuteScheduleUsed))
                            {
                                this.m_bIsMinuteScheduleUsed = true;
                            }
                            if (this.m_bIsMinuteScheduleUsed && this.m_bIsJobExistsDefinitionPresent)
                            {
                                goto Label_0233;
                            }
                        }
                    }
                }
                else if (method.Name.Name.Equals("FeatureDeactivating"))
                {
                    enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        current = enumerator.Current;
                        if (null != current.Value)
                        {
                            if (current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPWebApplication.get_JobDefinitions") || current.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPPersistedObject.Delete"))
                            {
                                num = (short) (num + 1);
                            }
                            if (num == 2)
                            {
                                this.m_bIsJobDeleteDefinitionPresent = true;
                                goto Label_0233;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointTimerjobImplementationCheck:VisitMethod() - " + exception.Message);
            }
        Label_0233:
            base.VisitMethod(method);
        }
    }
}

