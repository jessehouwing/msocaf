namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointVerboseLoggingCheck : BaseIntrospectionRule
    {
        public SharePointVerboseLoggingCheck() : base("SharePointVerboseLoggingCheck", "SharePointCustomRules.CustomRules", typeof(SharePointVerboseLoggingCheck).Assembly)
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
                    for (short i = 0; i < method.Instructions.Count; i = (short) (i + 1))
                    {
                        instruction = method.Instructions[i];
                        if ((instruction.Value != null) && (null != (instruction.Value as Method)))
                        {
                            MetadataCollection<Parameter>.Enumerator enumerator = (instruction.Value as Method).Parameters.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Parameter current = enumerator.Current;
                                if (current.ToString().Contains("Microsoft.SharePoint.Administration.TraceSeverity") && method.Instructions[i - 1].Value.Equals(100))
                                {
                                    resolution = base.GetResolution(new string[] { method.ToString() });
#if ORIGINAL 
                                    base.Problems.Add(new Problem(resolution, Convert.ToString(num)));
#else
                                    base.Problems.Add(new Problem(resolution, current, Convert.ToString(num)));
#endif
                                    num++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logging.UpdateLog(CustomRulesResource.ErrorOccured + "SharePointVerboseLoggingCheck:Check() - " + exception.Message);
            }
            return base.Problems;
        }
    }
}
