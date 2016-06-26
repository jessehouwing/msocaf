﻿namespace SharePointCustomRules
{
    using Microsoft.FxCop.Sdk;
    using System;

    public class SharePointHardCodedControlTemplatesPath : BaseIntrospectionRule
    {
        public SharePointHardCodedControlTemplatesPath() : base("SharePointHardCodedControlTemplatesPath", "SharePointCustomRules.CustomRules", typeof(SharePointCustomRules.SharePointHardCodedControlTemplatesPath).Assembly)
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
                        if (((null != instruction.Value) && method.Instructions[i].OpCode.ToString().Contains("Ldstr")) && method.Instructions[i].Value.ToString().ToUpper().Contains("_CONTROLTEMPLATES".ToUpper()))
                        {
                            Resolution resolution = base.GetResolution(new string[] { method.ToString() });
                            base.Problems.Add(new Problem(resolution));
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logging.UpdateLog("Error occured in function : " + "SharePointHardCodedControlTemplatesPath:Check() - " + exception.Message);
                }
            }
            return base.Problems;
        }
    }
}

