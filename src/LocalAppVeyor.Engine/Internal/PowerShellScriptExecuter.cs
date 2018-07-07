using System;
using System.Management.Automation;

namespace LocalAppVeyor.Engine.Internal
{
    internal static class PowerShellScriptExecuter
    {
        private static PowerShell powerShell = PowerShell.Create();

        public static bool Execute(
            string workingDirectory,
            string script,
            Action<string> onOutputDataReceived,
            Action<string> onErrorDataReceived)
        {
            var successful = true;

            powerShell.AddScript(script);

            powerShell.Streams.Warning.DataAdding += (sender, args) =>
            {
                onErrorDataReceived(args.ItemAdded.ToString());
            };

            powerShell.Streams.Error.DataAdding += (sender, args) =>
            {
                onErrorDataReceived(args.ItemAdded.ToString());
                successful = false;
            };

            var results = powerShell.Invoke();

            foreach (PSObject result in results)
            {
                onOutputDataReceived(result.ToString());
            }
            
            return successful && !powerShell.HadErrors;
        }
    }
}