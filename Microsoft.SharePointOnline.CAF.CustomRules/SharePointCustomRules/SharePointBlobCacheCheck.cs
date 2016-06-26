namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointBlobCacheCheck : BaseIntrospectionRule
    {
        public SharePointBlobCacheCheck() : base("SharePointBlobCacheCheck", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRules.SharePointBlobCacheCheck).Assembly)
        {
        }

        public override ProblemCollection Check(Member member)
        {
            Method method = member as Method;
            if (null != method)
            {
                try
                {
                    for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                    {
                        Instruction instruction = method.Instructions[i];
                        if ((null != instruction.Value) && (instruction.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPWebConfigModification(") || instruction.Value.ToString().Contains("Microsoft.SharePoint.Administration.SPWebConfigModification.set_Path")))
                        {
                            for (int j = i - 1; j > 0; j--)
                            {
                                if (!method.Instructions[j].OpCode.ToString().Contains("Ldstr"))
                                {
                                    break;
                                }
                                if (method.Instructions[j].Value.ToString().Contains("BlobCache"))
                                {
                                    Resolution resolution = base.GetResolution(new string[] { method.ToString() });
                                    base.Problems.Add(new Problem(resolution));
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointCustomItemCheck:Check() - " + exception.Message);
                }
            }
            return base.Problems;
        }
    }
}

