namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Globalization;
    using System.Linq;

    public class SharePointRunWithElevatedPrivilegesCheck : BaseIntrospectionRule
    {
        public SharePointRunWithElevatedPrivilegesCheck() : base("SharePointRunWithElevatedPrivilegesCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRules.SharePointRunWithElevatedPrivilegesCheck).Assembly)
        {
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
                    if (current is ClassNode)
                    {
                        MetadataCollection<Member>.Enumerator enumerator2 = current.Members.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Member member = enumerator2.Current;
                            if (member.FullName.Contains("CS$<>9__CachedAnonymousMethodDelegate") && ((Field) member).Type.ToString().Contains("Microsoft.SharePoint.SPSecurity+CodeToRunElevated"))
                            {
                                string[] strArray = member.FullName.Split(new char[] { '_' });
                                int DelegateMethodNumber = int.Parse(strArray[2].Substring(0x1d), NumberStyles.AllowHexSpecifier);
                                DelegateMethodNumber--;
                                Member member2 = (from t in current.Members
                                    where t.FullName.Contains("b__" + Convert.ToString(DelegateMethodNumber, 0x10))
                                    select t).FirstOrDefault<Member>();
                                string[] source = "SCardSvr,SNMPTRAP".Split(new char[] { ',' });
                                Method method = member2 as Method;
                                if (null != method)
                                {
                                    for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                                    {
                                        Instruction instruction = method.Instructions[i];
                                        if (null != instruction.Value)
                                        {
                                            if (instruction.OpCode.ToString().Equals("Call") || instruction.OpCode.ToString().Equals("Callvirt"))
                                            {
                                                if (((instruction.Value.ToString().Equals("System.IO.Directory.Delete") || instruction.Value.ToString().Equals("System.IO.DirectoryInfo.Delete")) || instruction.Value.ToString().Equals("System.IO.File.Delete")) || instruction.Value.ToString().Equals("System.IO.FileSystemInfo.Delete"))
                                                {
                                                    namedResolution = this.GetNamedResolution("FileDeleteOperationCheck", new string[] { instruction.Value.ToString() });
                                                    base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                                    num++;
                                                }
                                                if (instruction.Value.ToString().Equals("Microsoft.Office.Server.UserProfiles.UserProfileManager.RemoveUserProfile"))
                                                {
                                                    namedResolution = this.GetNamedResolution("ProfileDeleteOperationCheck", new string[] { method.Name.ToString() });
                                                    base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                                    num++;
                                                }
                                            }
                                            if (((instruction.OpCode.ToString().Equals("Newobj") && instruction.Value.ToString().Contains("System.ServiceProcess.ServiceController(")) && method.Instructions[i - 1].OpCode.ToString().Equals("Ldstr")) && source.Contains<string>(method.Instructions[i - 1].Value.ToString()))
                                            {
                                                namedResolution = this.GetNamedResolution("ServiceOperationCheck", new string[] { method.Instructions[i - 1].Value.ToString() });
                                                base.Problems.Add(new Problem(namedResolution, Convert.ToString(num)));
                                                num++;
                                            }
                                        }
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
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheck" + exception.Message);
            }
            catch (Exception exception2)
            {
                str = string.Empty;
                Logging.UpdateLog((CustomRulesResource.ErrorOccured + "SharePointMonitorScopeWebpartCheck") + exception2.InnerException.Message + exception2.Message);
            }
            return base.Problems;
        }
    }
}

