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

            powerShell.Commands.Clear();
            powerShell.AddScript(script);

            powerShell.Streams.ClearStreams();

            powerShell.Streams.Information.DataAdding += (sender, args) =>
            {
                onOutputDataReceived(args.ItemAdded.ToString());
            };

            powerShell.Streams.Warning.DataAdding += (sender, args) =>
            {
                onErrorDataReceived(args.ItemAdded.ToString());
            };

            powerShell.Streams.Error.DataAdding += (sender, args) =>
            {
                onErrorDataReceived(args.ItemAdded.ToString());
                successful = false;
            };

            var results = powerShell.BeginInvoke();

            results.AsyncWaitHandle.WaitOne();

            powerShell.EndInvoke(results);
            
            return successful && !powerShell.HadErrors;
        }
    }
}