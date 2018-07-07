using System;
using System.Linq;

namespace LocalAppVeyor.Engine.Internal.Steps
{
    internal class InitStandardEnvironmentVariablesStep : IInternalEngineStep
    {
        public bool Execute(ExecutionContext executionContext)
        {
            executionContext.Outputter.Write("Initializing environment variables...");

            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_NUMBER", "0", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", executionContext.BuildConfiguration.Version, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_FOLDER", executionContext.CloneDirectory, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("CI", "False", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("APPVEYOR", "False", EnvironmentVariableTarget.User);

            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_NUMBER", "0", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_VERSION", executionContext.BuildConfiguration.Version, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPVEYOR_BUILD_FOLDER", executionContext.CloneDirectory, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("CI", "False", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("APPVEYOR", "False", EnvironmentVariableTarget.Process);

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Configuration))
            {
                Environment.SetEnvironmentVariable("CONFIGURATION", executionContext.CurrentJob.Configuration, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("CONFIGURATION", executionContext.CurrentJob.Configuration, EnvironmentVariableTarget.Process);
            }

            if (!string.IsNullOrEmpty(executionContext.CurrentJob.Platform))
            {
                Environment.SetEnvironmentVariable("PLATFORM", executionContext.CurrentJob.Platform, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable("PLATFORM", executionContext.CurrentJob.Platform, EnvironmentVariableTarget.Process);
            }

            foreach (
                var variable
                in executionContext.BuildConfiguration.EnvironmentVariables.CommonVariables.Concat(executionContext.CurrentJob.Variables))
            {
                Environment.SetEnvironmentVariable(variable.Name, variable.Value, EnvironmentVariableTarget.User);
                Environment.SetEnvironmentVariable(variable.Name, variable.Value, EnvironmentVariableTarget.Process);
                }

            executionContext.Outputter.Write("Environment variables initialized.");

            return true;
        }
    }
}
