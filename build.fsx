// include Fake lib
#r @"Tools/FAKE/FakeLib.dll"
open Fake
open System

// build properties
let frameworkProfile = @"portable-net40+sl4+wp7+win8"
let buildTarget = getBuildParamOrDefault "buildTarget" "Build"
let buildConfiguration = getBuildParamOrDefault "buildConfiguration" "Release"

// common path properties
let sourceDir = currentDirectory @@ "src"
let buildDir = currentDirectory @@ "Build"
let buildMergedDir = buildDir @@ "merged"
let buildMetricsDir = currentDirectory @@ "BuildMetrics"
let distributionDir = currentDirectory @@ "Distribution"
let publishDir = currentDirectory @@ "Publish"
let solutionFile = sourceDir @@ "Portable.Licensing.sln"

// tools path properties
let toolsDir = currentDirectory @@ "Tools"
let nunitPath = toolsDir @@ "NUnit"
let nugetExecutable = toolsDir @@ "NuGet" @@ "NuGet.exe"
let mergerExecutable = toolsDir @@ "ILRepack" @@ "ILRepack.exe"

// common assembly info properties
let assemblyVersion = getBuildParamOrDefault "assemblyVersion" "0.0.0.0"
let assemblyFileVersion = getBuildParamOrDefault "assemblyFileVersion" "0.0.0.0"
let assemblyInformationalVersion = getBuildParamOrDefault "assemblyInformationalVersion" "0.0.0-devel"

// Targets
Target "All" DoNothing

Target "Clean" (fun _ ->
    let directoriesToClean =
        !+ buildDir
        ++ buildMetricsDir
        ++ distributionDir
        ++ publishDir |> Scan

    CleanDirs directoriesToClean
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
            Targets = [buildTarget]
            Properties = ["Configuration", buildConfiguration]
        }
    ) solutionFile
)

Target "Test" (fun _ ->
    !! (buildDir @@ "*Tests.dll") 
      |> NUnit (fun p ->
          {p with
             ToolPath = nunitPath;
             //DisableShadowCopy = true;
             OutputFile = buildMetricsDir @@ "nunit-result.xml" })
)

Target "MergeAssemblies" (fun _ ->
    CreateDir buildMergedDir

    let result =
        ExecProcess (fun info ->
            info.FileName <- mergerExecutable
            info.WorkingDirectory <- buildDir
            info.Arguments <- sprintf "-target:library -internalize -verbose -lib:%s /out:%s %s %s" buildDir (buildMergedDir @@ "Portable.Licensing.dll") "Portable.Licensing.dll" "BouncyCastle.Crypto.dll"
            ) (TimeSpan.FromMinutes 5.)

    if result <> 0 then failwithf "Error during ILRepack execution."

    let filesToDelete =
        !+ (buildDir @@ "Portable.Licensing.*")
        ++ (buildDir @@ "BouncyCastle.Crypto.*")
        -- (buildDir @@ "Portable.Licensing.XML") |> Scan

    DeleteFiles filesToDelete

    let filesToCopy =
        !+ (buildMergedDir @@ "Portable.Licensing.*") |> Scan

    Copy buildDir filesToCopy
)

Target "PreparePackaging" (fun _ ->
    // create the directory structure for the deployment zip and NuGet package
    // and copy in the files to deploy

    let docsDir = distributionDir @@ "Documentation"
    CreateDir docsDir

    CopyFile docsDir "Readme.md"
    CopyFile docsDir "LICENSE.txt"

    let libsDir = distributionDir @@ "lib" @@ frameworkProfile
    CreateDir libsDir

    let librariesToDeploy =
        !+ (buildDir @@ "Portable.Licensing.dll")
        ++ (buildDir @@ "Portable.Licensing.XML")
        -- (buildDir @@ "Portable.Licensing.pdb") |> Scan

    Copy libsDir librariesToDeploy
)

Target "PackgaeZipDistribution" (fun _ ->
    Zip distributionDir (publishDir @@ String.Format(@"Portable.Licensing-{0}.zip", assemblyInformationalVersion)) !! (distributionDir @@ "**")
)

Target "PackageNuGetDistribution" (fun _ ->

    // rename Readme.md to Readme.txt in the NuGet package, so that the file 
    // could be shown automaticly by VS during package installation
    CopyFile distributionDir "Readme.md"
    Rename (distributionDir @@ "Readme.txt") (distributionDir @@ "Readme.md")

    let result =
        ExecProcess (fun info ->
            info.FileName <- nugetExecutable
            info.WorkingDirectory <- "./"
            info.Arguments <- sprintf "pack Portable.Licensing.nuspec -Version %s -OutputDirectory %s" assemblyInformationalVersion publishDir
            ) (TimeSpan.FromMinutes 5.)

    if result <> 0 then failwithf "Error during NuGet creation."

    // delete the renamed file
    DeleteFile (distributionDir @@ "Readme.txt")
)

// Dependencies
"Clean" 
//  ==> "CreateAssemblyInfo"
    ==> "Build"
    ==> "Test"
    ==> "MergeAssemblies"
    ==> "PreparePackaging"
    ==> "PackgaeZipDistribution"
    ==> "PackageNuGetDistribution"
    ==> "All"
 
// start build
Run <| getBuildParamOrDefault "target" "All"