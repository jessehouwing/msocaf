namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Reflection;

    public class SharePointMonitorScopeCheck : BaseIntrospectionRule
    {
        protected bool bSPMonitoredScopeWarning;

        public SharePointMonitorScopeCheck() : base("SharePointMonitorScopeCheck", "SharePointCustomRules.CustomRules", typeof(SharePointMonitorScopeCheck).Assembly)
        {
            this.bSPMonitoredScopeWarning = false;
        }

        public SharePointMonitorScopeCheck(string name, string resourceName, Assembly resourceAssembly) : base(name, resourceName, resourceAssembly)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            try
            {
                if (null != (member as Method))
                {
                    Method method = member as Method;
                    Instruction instruction = null;
                    Resolution resolution = null;
                    int num = 0;
                    short num2 = 0;
                    bool flag = false;
                    bool flag2 = false;
                    for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                    {
                        instruction = method.Instructions[i];
                        if (null != instruction.Value)
                        {
                            if (instruction.Value.ToString().Contains("SharePoint.Administration.SPDeveloperDashboardSettings.set_DisplayLevel") && (method.Instructions[i - 1].Value.Equals(1) || method.Instructions[i - 1].Value.Equals(2)))
                            {
                                num2 = (short) (num2 + 1);
                            }
                            if (instruction.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPPersistedObject.Update"))
                            {
                                num2 = (short) (num2 + 1);
                                flag2 = true;
                            }
                            if (num2.Equals((short) 2) && (instruction.OpCode.Equals(OpCode.Newobj) && instruction.Value.ToString().Contains("Microsoft.SharePoint.Utilities.SPMonitoredScope")))
                            {
                                num2 = 0;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!(flag || !flag2))
                    {
                        resolution = base.GetResolution(new string[] { method.ToString() });
                    }
                    else if (flag && (method.Name.ToString().Equals("OnInit") || method.Name.ToString().Equals("Render")))
                    {
                        resolution = ((SharePointMonitorScopeWarningCheck) this).GetResolution(new string[] { method.ToString() });
                    }
                    if (resolution != null)
                    {
                        base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
                        num++;
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointMonitorScopeCheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }
    }
}

