using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Disposition;
using System.IO;
using Microsoft.FxCop.Sdk;
using Microsoft.VisualStudio.CodeAnalysis.Extensibility;

namespace Microsoft.SharePoint.DisposeChecker.Wrapper
{
    public class DisposeCheckerWrapper
    {
        public static Dictionary<string, Disposition.Problem[]> cache = new Dictionary<string, Disposition.Problem[]>();

        Type _disposeCheckerType;
        object _disposeChecker;

        public DisposeCheckerWrapper()
        {
            var source = typeof(Disposition.Problem).Assembly;
            _disposeCheckerType = source.GetType("Disposition.DisposeChecker");
            _disposeChecker = Activator.CreateInstance(_disposeCheckerType, true);
        }

        public Disposition.Problem[] CheckForDispose(string path)
        {
            return CheckForDispose(path, false, false, true, false, false, false);
        }

        public Disposition.Problem[] CheckForDispose(string path, bool debug, bool v1, bool showUndocumented, bool onlyUndocumented, bool onlyDisposed, bool onlyNotDisposed)
        {
            if (!cache.ContainsKey(path))
            {
                lock (cache)
                {
                    if (!cache.ContainsKey(path))
                    {
                        Disposition.Problem[] outcome = (Disposition.Problem[])_disposeCheckerType.GetMethod("CheckForDispose", BindingFlags.Instance | BindingFlags.Public).Invoke(_disposeChecker, new object[]
                        {
                            path, debug, v1, showUndocumented, onlyUndocumented, onlyDisposed, onlyNotDisposed
                        });

                        cache.Add(path, outcome);
                    }
                }
            }
            
            return cache[path];
        }

        public string[] GetMethodsIgnored()
        {
            return (string[]) _disposeCheckerType.GetMethod("GetMethodsIgnored", BindingFlags.Instance | BindingFlags.Public).Invoke(_disposeChecker, new object[]{});
        }

        public string[] GetModulesChecked()
        {
            return (string[]) _disposeCheckerType.GetMethod("GetModulesChecked", BindingFlags.Instance | BindingFlags.Public).Invoke(_disposeChecker, new object[]{});
        }

        public string[] GetModulesIgnored()
        {
            return (string[]) _disposeCheckerType.GetMethod("GetModulesIgnored", BindingFlags.Instance | BindingFlags.Public).Invoke(_disposeChecker, new object[]{});
        }
    }
}
