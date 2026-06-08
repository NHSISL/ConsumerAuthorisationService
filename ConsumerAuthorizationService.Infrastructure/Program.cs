// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using ConsumerAuthorizationService.Infrastructure.Services;

namespace ConsumerAuthorizationService.Infrastructure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var scriptGenerationService = new ScriptGenerationService();

            scriptGenerationService.GenerateBuildScript(
                branchName: "main",
                projectName: "ConsumerAuthorizationService.Core",
                dotNetVersion: "10.0.x");

            scriptGenerationService.GeneratePrLintScript(branchName: "main");
        }
    }
}
