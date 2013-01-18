// include Fake lib
#r @"Tools\FAKE\FakeLib.dll"
open Fake
open System;

// Properties
let sourceDir = @"./src/"
let buildDir = @"./Build/"
let publishDir = @"./Publish/"
let solutionFile = sourceDir @@ "Portable.Licensing.sln"

// common assembly info properties
let assemblyVersion = "0.0.0.0"
let assemblyFileVersion = "0.0.0.0"
let assemblyInformationalVersion = "0.0.0-devel"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
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
          OutputFileName = @"./src/CommonAssemblyInfo.cs"
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

// Dependencies
"Clean" 
  ==> "Build"

//"CreateAssemblyInfo"
//  ==> "Build"
 
// start build
Run "Build"