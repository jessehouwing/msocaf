namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;

    public class SharePointListItemUpdateCheck : BaseIntrospectionRule
    {
        public SharePointListItemUpdateCheck() : base("SharePointListItemUpdateCheck", "SharePointCustomRules.CustomRules", typeof(SharePointListItemUpdateCheck).Assembly)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            try
            {
                Method method = member as Method;
                if (null != method)
                {
                    List<Instruction> list = new List<Instruction>();
                    list = new SPLoopDetector().CheckAndGetInstructionsWithinLoopIfExists(method);
                    if (list.Count > 0)
                    {
                        int num = 0;
                        foreach (Instruction instruction in list)
                        {
                            if (instruction.Value.ToString().Contains("SPItem.Update"))
                            {
                                Resolution resolution = base.GetResolution(new string[] { method.ToString(), instruction.Value.ToString() });
#if ORIGINAL
                                 base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
#else
                                base.Problems.Add(new Problem(resolution, instruction, Convert.ToString(num)));
#endif
                                num++;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointListItemUpdateCheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }
    }
}
