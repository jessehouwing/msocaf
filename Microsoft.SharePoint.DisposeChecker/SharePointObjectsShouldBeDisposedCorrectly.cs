using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.DisposeChecker.Wrapper;
using Microsoft.FxCop.Sdk;
using Microsoft.VisualStudio.CodeAnalysis.Extensibility;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.SharePoint.DisposeChecker
{
    public class SharePointObjectsShouldBeDisposedCorrectly : SharepointRuleBase
    {
        DisposeCheckerWrapper disposeChecker = new DisposeCheckerWrapper();

        public SharePointObjectsShouldBeDisposedCorrectly() : base("SharePointObjectsShouldBeDisposedCorrectly")
        {
        }

        public override ProblemCollection Check(Member member)
        {
            var results = disposeChecker.CheckForDispose(member.DeclaringType.DeclaringModule.Location);
            ProblemArrayToCollection(results.Where(r => 
                r.Method == member.FullName 
                || 
                    (member.NodeType == NodeType.Field 
                    && member.IsStatic 
                    && r.Assignment.StartsWith(member.FullName + " "))
                || 
                    (member.NodeType == NodeType.Field 
                    && !member.IsStatic
                    && r.Assignment.StartsWith("this." + member.Name + " "))
                ), member);

            return this.Problems;
        }

        public void ProblemArrayToCollection(IEnumerable<Disposition.Problem> problems, Member member)
        {
            foreach (var item in problems)
            {
                string itemId = "SPD" + item.ID.Substring(item.ID.LastIndexOf('_') + 1).PadLeft(4, '0'); 
                this.Problems.Add(
                    new Microsoft.FxCop.Sdk.Problem(new Resolution("SharePointObjectsShouldBeDisposedCorrectly", "{0} @ '{1}'", item.Notes, item.Assignment),  member)
                    {
                        SourceFile = item.Source,
                        SourceLine = item.Line,
                        FixCategory = FixCategories.DependsOnFix,
                        Certainty = 100,
                        MessageLevel = MessageLevel.Warning
                    }
                );
            }
        }
    }
}
