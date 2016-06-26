using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FxCop.Sdk;

namespace Microsoft.SharePoint.DisposeChecker
{
    public abstract class SharepointRuleBase : BaseIntrospectionRule
    {
        protected SharepointRuleBase(string name)
            : base(name, "Microsoft.SharePoint.DisposeChecker.Rules", typeof(SharepointRuleBase).Assembly)
        {
        }
    }
}
