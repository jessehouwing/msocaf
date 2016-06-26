namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using Microsoft.VisualStudio.CodeAnalysis.Extensibility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public class SharePointClaimsCheck : BaseIntrospectionRule
    {
        private List<SPClaimsTypesToBeChecked> m_listSPDeprecatedAPIStore;
        private static Random problemIdGenerator;
        private static object SyncObject = new object();

        public SharePointClaimsCheck() : base("SharePointClaimsCheck", "SharePointCustomRules.CustomRules", typeof(SharePointClaimsCheck).Assembly)
        {
            this.m_listSPDeprecatedAPIStore = new List<SPClaimsTypesToBeChecked>();
            problemIdGenerator = new Random(1);
        }

        public override ProblemCollection Check(Member member)
        {
            string str = string.Empty;
            Resolution resolution = null;
            string fullName = string.Empty;
            string str3 = string.Empty;
            try
            {
                if (this.m_listSPDeprecatedAPIStore.Count.Equals(0))
                {
                    this.FillSPDeprecatedAPIStore();
                }
                if (member is Microsoft.FxCop.Sdk.Method)
                {
                    Microsoft.FxCop.Sdk.Method method = member as Microsoft.FxCop.Sdk.Method;
                    MetadataCollection<Instruction>.Enumerator enumerator = method.Instructions.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Instruction current = enumerator.Current;
                        foreach (SPClaimsTypesToBeChecked checked2 in this.m_listSPDeprecatedAPIStore)
                        {
                            Problem problem;
                            if (current.Value is Local)
                            {
                                Local local = current.Value as Local;
                                fullName = (current.Value as Local).Type.FullName;
                                str3 = checked2.Namespace + "." + checked2.APIType;
                                if (fullName.Equals(str3))
                                {
                                    resolution = new Resolution("This rule is applicable only to SharePoint 2010 customizations. {0}", new string[] { checked2.Message });
                                    problem = new Problem(resolution, current.SourceContext) {
                                        Certainty = 90,
                                        FixCategory = FixCategories.NonBreaking,
                                        Id = this.GetNextId(),
                                        MessageLevel = MessageLevel.Warning
                                    };
                                    base.Problems.Add(problem);
                                    fullName = string.Empty;
                                    str3 = string.Empty;
                                    break;
                                }
                            }
                            else
                            {
                                if (null == (current.Value as Microsoft.FxCop.Sdk.Method))
                                {
                                    break;
                                }
                                Microsoft.FxCop.Sdk.Method method2 = current.Value as Microsoft.FxCop.Sdk.Method;
                                fullName = this.GetOnlyTheMethodName(method2.FullName);
                                str3 = checked2.Namespace + "." + checked2.APIType;
                                if (fullName.Equals(str3))
                                {
                                    resolution = new Resolution("This rule is applicable only to SharePoint 2010 customizations. {0}", new string[] { checked2.Message });
                                    problem = new Problem(resolution, current.SourceContext) {
                                        Certainty = 90,
                                        FixCategory = FixCategories.NonBreaking,
                                        Id = this.GetNextId(),
                                        MessageLevel = MessageLevel.Warning
                                    };
                                    base.Problems.Add(problem);
                                    str = string.Empty;
                                    fullName = string.Empty;
                                    break;
                                }
                                MetadataCollection<Parameter>.Enumerator enumerator3 = method2.Parameters.GetEnumerator();
                                while (enumerator3.MoveNext())
                                {
                                    Parameter parameter = enumerator3.Current;
                                    fullName = parameter.Type.FullName;
                                    str3 = checked2.Namespace + "." + checked2.APIType;
                                    if (fullName.Equals(str3))
                                    {
                                        resolution = new Resolution("This rule is applicable only to SharePoint 2010 customizations. {0}", new string[] { checked2.Message });
                                        problem = new Problem(resolution, current.SourceContext) {
                                            Certainty = 90,
                                            FixCategory = FixCategories.NonBreaking,
                                            Id = this.GetNextId(),
                                            MessageLevel = MessageLevel.Warning
                                        };
                                        base.Problems.Add(problem);
                                        str = string.Empty;
                                        fullName = string.Empty;
                                    }
                                }
                            }
                        }
                    }
                    this.VisitStatements(method.Body.Statements);
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointDeprecatedAPICheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }

        private void FillSPDeprecatedAPIStore()
        {
            string str5;
            try
            {
                StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointCustomRules.SPS2010ClaimsTypes.txt"));
                string str = string.Empty;
                SPClaimsTypesToBeChecked item = null;
                string str2 = string.Empty;
                string str3 = string.Empty;
                string str4 = string.Empty;
                while (!reader.EndOfStream)
                {
                    item = new SPClaimsTypesToBeChecked();
                    str = reader.ReadLine();
                    str2 = str.Substring(0, str.IndexOf("Type"));
                    item.Namespace = str2.Substring(str2.IndexOf(':') + 2, (str2.IndexOf(',') - str2.IndexOf(':')) - 2);
                    str3 = str.Substring(str.IndexOf("Type"), str.IndexOf("Message") - str.IndexOf("Type"));
                    item.APIType = str3.Substring(str3.IndexOf(':') + 2, (str3.IndexOf(',') - str3.IndexOf(':')) - 2);
                    str4 = str.Substring(str.IndexOf("Message"), str.Length - str.IndexOf("Message"));
                    item.Message = str4.Substring(str4.IndexOf(':') + 2, (str4.Length - str4.IndexOf(':')) - 2);
                    this.m_listSPDeprecatedAPIStore.Add(item);
                }
                reader.Close();
            }
            catch (IOException exception)
            {
                str5 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointDeprecatedAPICheck:FillSPDeprecatedAPIStore() - " + exception.Message);
            }
            catch (XmlException exception2)
            {
                str5 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointDeprecatedAPICheck:FillSPDeprecatedAPIStore() - " + exception2.Message);
            }
            catch (Exception exception3)
            {
                str5 = string.Empty;
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointDeprecatedAPICheck:FillSPDeprecatedAPIStore() - " + exception3.Message);
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

        public override void VisitAssignmentStatement(AssignmentStatement assignment)
        {
            try
            {
                string str = string.Empty;
                Resolution resolution = null;
                string str2 = string.Empty;
                string str3 = string.Empty;
                foreach (SPClaimsTypesToBeChecked checked2 in this.m_listSPDeprecatedAPIStore)
                {
                    Problem problem;
                    str3 = checked2.Namespace + "." + checked2.APIType;
                    if (assignment.Target.NodeType.Equals(NodeType.Local))
                    {
                        Local target = assignment.Target as Local;
                        if ((target != null) && target.Type.Equals(str3))
                        {
                            resolution = new Resolution("This rule is applicable only to SharePoint 2010 customizations. {0}", new string[] { checked2.Message });
                            problem = new Problem(resolution, assignment.SourceContext) {
                                Certainty = 90,
                                FixCategory = FixCategories.NonBreaking,
                                Id = this.GetNextId(),
                                MessageLevel = MessageLevel.Warning
                            };
                            base.Problems.Add(problem);
                            str = string.Empty;
                            str2 = string.Empty;
                        }
                    }
                    else if (assignment.Target.NodeType.Equals(NodeType.MemberBinding))
                    {
                        Member boundMember = (assignment.Target as MemberBinding).BoundMember;
                        if ((boundMember != null) && boundMember.FullName.Equals(str3))
                        {
                            resolution = new Resolution("This rule is applicable only to SharePoint 2010 customizations. {0}", new string[] { checked2.Message });
                            problem = new Problem(resolution, assignment.SourceContext) {
                                Certainty = 90,
                                FixCategory = FixCategories.NonBreaking,
                                Id = this.GetNextId(),
                                MessageLevel = MessageLevel.Warning
                            };
                            base.Problems.Add(problem);
                            str = string.Empty;
                            str2 = string.Empty;
                        }
                    }
                }
            }
            catch
            {
            }
            base.VisitAssignmentStatement(assignment);
        }
    }
}

