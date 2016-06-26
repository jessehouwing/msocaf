namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;
    using System.Collections.Generic;

    public class SharePointItemsCollectionCheck : BaseIntrospectionRule
    {
        public SharePointItemsCollectionCheck() : base("SharePointItemsCollectionCheck", "SharePointCustomRules.CustomRules", typeof(SharePointItemsCollectionCheck).Assembly)
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
                            if (instruction.Value.ToString().Contains("SPList.get_Items") || (instruction.Value.ToString().Contains("SPList.GetItemById") || instruction.Value.ToString().Contains("SPList.GetItems")))
                            {
                                Resolution resolution = base.GetResolution(new string[] { method.ToString(), instruction.Value.ToString() });
                                base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
                                num++;
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointItemsCollectionCheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }
    }
}

