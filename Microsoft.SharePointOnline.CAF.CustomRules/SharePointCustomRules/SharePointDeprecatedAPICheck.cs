namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public class SharePointDeprecatedAPICheck : BaseIntrospectionRule
    {
        private List<SPDeprecatedAPIStore> m_listSPDeprecatedAPIStore;

        public SharePointDeprecatedAPICheck() : base("SharePointDeprecatedAPICheck", "SharePointCustomRules.CustomRules", typeof(SharePointDeprecatedAPICheck).Assembly)
        {
            this.m_listSPDeprecatedAPIStore = new List<SPDeprecatedAPIStore>();
        }

        public override ProblemCollection Check(Member member)
        {
            string str = string.Empty;
            Resolution resolution = null;
            int num = 0;
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
                        foreach (SPDeprecatedAPIStore store in this.m_listSPDeprecatedAPIStore)
                        {
                            if (current.Value is Local)
                            {
                                fullName = (current.Value as Local).Type.FullName;
                                str3 = store.Namespace + "." + store.APIType;
                                if (fullName.Equals(str3) || fullName.Contains("Microsoft.SharePoint.Portal"))
                                {
                                    resolution = base.GetResolution(new string[] { method.ToString(), fullName, store.Message });
                                    base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
                                    num++;
                                    fullName = string.Empty;
                                    str3 = string.Empty;
                                    break;
                                }
                            }
                            else if ((current.Value is Microsoft.FxCop.Sdk.Method) && (null != (current.Value as Microsoft.FxCop.Sdk.Method).DeclaringMember))
                            {
                                str = (current.Value as Microsoft.FxCop.Sdk.Method).DeclaringMember.DeclaringType.FullName;
                                fullName = (current.Value as Microsoft.FxCop.Sdk.Method).DeclaringMember.FullName;
                                if ((str.Equals(store.Namespace) && fullName.Equals(store.APIType)) || fullName.Contains("Microsoft.SharePoint.Portal"))
                                {
                                    resolution = base.GetResolution(new string[] { method.ToString(), fullName, store.Message });
                                    base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
                                    num++;
                                    str = string.Empty;
                                    fullName = string.Empty;
                                    break;
                                }
                            }
                        }
                    }
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
                StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("SharePointCustomRules.SPS2010Deprecated.txt"));
                string str = string.Empty;
                SPDeprecatedAPIStore item = null;
                string str2 = string.Empty;
                string str3 = string.Empty;
                string str4 = string.Empty;
                while (!reader.EndOfStream)
                {
                    item = new SPDeprecatedAPIStore();
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
    }
}

