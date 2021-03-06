// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using FluentAssertions;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Test.Utility;
using Xunit;

namespace Dotnet.Integration.Test
{
    [Collection("Dotnet Integration Tests")]
    public class DotnetRestoreTests
    {
        private MsbuildIntegrationTestFixture _msbuildFixture;

        public DotnetRestoreTests(MsbuildIntegrationTestFixture fixture)
        {
            _msbuildFixture = fixture;
        }

        [PlatformFact(Platform.Windows)]
        public void DotnetRestore_SolutionRestoreVerifySolutionDirPassedToProjects()
        {
            using (var pathContext = new SimpleTestPathContext())
            {
                _msbuildFixture.CreateDotnetNewProject(pathContext.SolutionRoot, "proj");

                var slnContents = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 15
VisualStudioVersion = 15.0.27330.1
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"") = ""proj"", ""proj\proj.csproj"", ""{216FF388-8C16-4AF4-87A8-9094030692FA}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{216FF388-8C16-4AF4-87A8-9094030692FA}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{216FF388-8C16-4AF4-87A8-9094030692FA}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{216FF388-8C16-4AF4-87A8-9094030692FA}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{216FF388-8C16-4AF4-87A8-9094030692FA}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {9A6704E2-6E77-4FF4-9E54-B789D88829DD}
	EndGlobalSection
EndGlobal";

                var slnPath = Path.Combine(pathContext.SolutionRoot, "proj.sln");
                File.WriteAllText(slnPath, slnContents);

                var projPath = Path.Combine(pathContext.SolutionRoot, "proj", "proj.csproj");
                var doc = XDocument.Parse(File.ReadAllText(projPath));

                doc.Root.Add(new XElement(XName.Get("Target"),
                    new XAttribute(XName.Get("Name"), "ErrorOnSolutionDir"),
                    new XAttribute(XName.Get("BeforeTargets"), "CollectPackageReferences"),
                    new XElement(XName.Get("Error"),
                        new XAttribute(XName.Get("Text"), $"|SOLUTION $(SolutionDir) $(SolutionName)|"))));

                File.Delete(projPath);
                File.WriteAllText(projPath, doc.ToString());

                var result = _msbuildFixture.RunDotnet(pathContext.SolutionRoot, "msbuild proj.sln /t:restore /p:DisableImplicitFrameworkReferences=true");

                result.ExitCode.Should().Be(1, "error text should be displayed");
                result.AllOutput.Should().Contain($"|SOLUTION {PathUtility.EnsureTrailingSlash(pathContext.SolutionRoot)} proj|");
            }
        }
    }
}
