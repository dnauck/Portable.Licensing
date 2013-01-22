// include Fake lib
#r @"Tools/FAKE/FakeLib.dll"
open Fake
open System;

// common path properties
let sourceDir = @"./src/"
let buildDir = @"./Build/"
let buildMetricsDir = @"./BuildMetrics/"
let publishDir = @"./Publish/"
let solutionFile = sourceDir @@ "Portable.Licensing.sln"

// tools path properties
let toolsDir = @"./Tools/"
let nunitPath = toolsDir @@ "NUnit/"

// common assembly info properties
let assemblyVersion = "0.0.0.0"
let assemblyFileVersion = "0.0.0.0"
let assemblyInformationalVersion = "0.0.0-devel"

// Targets
Target "All" DoNothing

Target "Clean" (fun _ ->
    CleanDir buildDir
    CleanDir buildMetricsDir
    CleanDir publishDir
)

Target "CreateAssemblyInfo" (fun _ ->
    AssemblyInfo 
      (fun asm -> 
        {asm with
          CodeLanguage = CSharp;
          AssemblyCompany = "Nauck IT KG";
          AssemblyProduct = "Portable.Licensing";
          AssemblyCopyright = String.Format("Copyright © 2012 - {0} Nauck IT KG", DateTime.Now.Year);
          AssemblyTrademark = String.Empty;
          AssemblyVersion = assemblyVersion;
          AssemblyFileVersion = assemblyFileVersion;
          AssemblyInformationalVersion = assemblyInformationalVersion;
          OutputFileName = sourceDir @@ "CommonAssemblyInfo.cs"
          }
        )
)

Target "Build" (fun _ ->
    build (fun msbuild ->
        {msbuild with
            Targets = ["Build"]
            Properties = ["Configuration","Release"]
        }
    ) solutionFile
)

Target "Test" (fun _ ->
    !! (buildDir @@ @"*Tests.dll") 
      |> NUnit (fun p ->
          {p with
             ToolPath = nunitPath;
             //DisableShadowCopy = true;
             OutputFile = buildMetricsDir @@ @"nunit-result.xml" })
)

// Dependencies
"Clean" 
//  ==> "CreateAssemblyInfo"
    ==> "Build"
    ==> "Test"
    ==> "All"
 
// start build
Run <| getBuildParamOrDefault "target" "All"