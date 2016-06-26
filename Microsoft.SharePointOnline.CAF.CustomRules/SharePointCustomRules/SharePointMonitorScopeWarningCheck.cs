namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointMonitorScopeWarningCheck : SharePointMonitorScopeCheck
    {
        public SharePointMonitorScopeWarningCheck() : base("SharePointMonitorScopeWarningCheck", "SharePointCustomRules.CustomRules", typeof(SharePointMonitorScopeWarningCheck).Assembly)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            if (member != null)
            {
                Method method = member as Method;
                if ((null != method) && (method.Name.ToString().Equals("OnInit") || method.Name.ToString().Equals("Render")))
                {
                    return base.Check(member);
                }
            }
            return base.Problems;
        }
    }
}
