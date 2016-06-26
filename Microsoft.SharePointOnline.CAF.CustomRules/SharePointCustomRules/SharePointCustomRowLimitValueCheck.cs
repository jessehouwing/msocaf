namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointCustomRowLimitValueCheck : BaseIntrospectionRule
    {
        private bool m_bValueFound;
        private int m_iValue;
        private string m_sGetValueName;

        public SharePointCustomRowLimitValueCheck() : base("SharePointCustomRowLimitValueCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRowLimitValueCheck).Assembly)
        {
            this.m_iValue = 1;
            this.m_bValueFound = false;
        }

        public override ProblemCollection Check(Member member)
        {
            string str2;
            Method method = member as Method;
            int num = 0;
            Instruction instruction = null;
            int num2 = 0;
            try
            {
                if (null != method)
                {
                    MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Instruction current = enumerator.Current;
                        if ((current.Value != null) && current.Value.ToString().Contains("SPQuery.set_RowLimit"))
                        {
                            instruction = method.Instructions[num - 1];
                            if (null != instruction.Value)
                            {
                                if (instruction.OpCode.ToString().Contains(OpCode.Ldloc.ToString()))
                                {
                                    this.m_sGetValueName = (instruction.Value as Local).Name.Name;
                                    this.VisitMethod(method);
                                }
                                else if (instruction.OpCode.ToString().Contains(OpCode.Ldfld.ToString()))
                                {
                                    this.m_sGetValueName = (instruction.Value as Member).Name.Name;
                                    this.VisitClass(method.DeclaringType as ClassNode);
                                }
                                else if (instruction.OpCode.ToString().Contains(OpCode.Ldsfld.ToString()))
                                {
                                    this.m_sGetValueName = (instruction.Value as Member).Name.Name;
                                    this.VisitModule(method.DeclaringType.DeclaringModule);
                                }
                                else
                                {
                                    this.m_iValue = Convert.ToInt32(instruction.Value.ToString());
                                }
                                this.m_bValueFound = false;
                            }
                        }
                        if ((this.m_iValue < 1) || (this.m_iValue > 0x7d0))
                        {
                            num2++;
                            string name = string.Empty;
                            if (null != (method.Instructions[num - 2].Value as Local))
                            {
                                name = (method.Instructions[num - 2].Value as Local).Name.Name;
                            }
                            else if (null != (method.Instructions[num - 2].Value as Parameter))
                            {
                                name = (method.Instructions[num - 2].Value as Parameter).Name.Name;
                            }
                            else if (null != (method.Instructions[num - 2].Value as Member))
                            {
                                name = (method.Instructions[num - 2].Value as Member).Name.Name;
                            }
                            Resolution resolution = base.GetResolution(new string[] { name, method.Name.Name, method.DeclaringType.Name.Name, Convert.ToString(this.m_iValue) });
                            base.Problems.Add(new Problem(resolution, Convert.ToString(num2)));
                            this.m_iValue = 1;
                        }
                        num++;
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitValueCheck:Check() - " + exception.Message);
            }
            catch (Exception exception2)
            {
                str2 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitValueCheck:Check() - " + exception2.Message);
            }
            return base.Problems;
        }

        public override void VisitAssignmentStatement(AssignmentStatement assignment)
        {
            bool flag = false;
            if (!this.m_bValueFound)
            {
                try
                {
                    if (assignment.Target.NodeType.Equals(NodeType.Local))
                    {
                        Local target = assignment.Target as Local;
                        if ((target != null) && target.Name.Name.Equals(this.m_sGetValueName))
                        {
                            flag = true;
                        }
                    }
                    else if (assignment.Target.NodeType.Equals(NodeType.MemberBinding))
                    {
                        Member boundMember = (assignment.Target as MemberBinding).BoundMember;
                        if ((boundMember != null) && boundMember.Name.Name.Equals(this.m_sGetValueName))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        this.m_iValue = Convert.ToInt32((assignment.Source as Literal).Value);
                        this.m_bValueFound = true;
                    }
                }
                catch (NullReferenceException exception)
                {
                    Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomRowLimitValueCheck:VisitAssignmentStatement() - " + exception.Message);
                }
                base.VisitAssignmentStatement(assignment);
            }
        }
    }
}

