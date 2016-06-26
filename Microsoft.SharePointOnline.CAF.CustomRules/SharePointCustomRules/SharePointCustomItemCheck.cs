namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointCustomItemCheck : BaseIntrospectionRule
    {
        public SharePointCustomItemCheck() : base("SharePointCustomItemCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomItemCheck).Assembly)
        {
            Logging.Initialize(CustomRulesResource.SPListItemRuleName);
        }

        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (null != method)
            {
                string str;
                try
                {
                    for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                    {
                        Instruction instruction = method.Instructions[i];
                        if ((null != instruction.Value) && instruction.Value.ToString().Contains("SPList.get_Items"))
                        {
                            Resolution resolution = base.GetResolution(new string[] { method.ToString() });
#if (ORIGINAL)
                            base.Problems.Add(new Problem(resolution));
#else
                            base.Problems.Add(new Problem(resolution, instruction));
#endif
                        }
                    }
                }
                catch (IndexOutOfRangeException exception)
                {
                    str = string.Empty;
                    string str2 = "SharePointCustomItemCheck:Check() - ";
                    Logging.UpdateLog(CustomRulesResource.ErrorOccured + str2 + exception.Message);
                }
                catch (Exception exception2)
                {
                    str = string.Empty;
                    Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomItemCheck:Check() - " + exception2.Message);
                }
            }
            return base.Problems;
        }
    }
}
