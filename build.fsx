// include Fake lib
#I @"Tools/FAKE/"
#r @"FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
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
let xpkgExecutable = toolsDir @@ "xpkg" @@ "xpkg.exe"

// common assembly info properties
let assemblyVersion = getBuildParamOrDefault "assemblyVersion" "1.0.0.0"
let assemblyFileVersion = getBuildParamOrDefault "assemblyFileVersion" "1.1.0.0"
let assemblyInformationalVersion = getBuildParamOrDefault "assemblyInformationalVersion" "1.1.0"

// Targets
Target "All" DoNothing

Target "Clean" (fun _ ->
    CleanDirs [
        buildDir 
        buildMetricsDir
        distributionDir
        publishDir ]
)

Target "CreateAssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo (sourceDir @@ "CommonAssemblyInfo.cs")
        [
        Attribute.Company "Nauck IT KG"
        Attribute.Product "Portable.Licensing"
        Attribute.Copyright (sprintf "Copyright © 2012 - %A Nauck IT KG" DateTime.Now.Year)
        Attribute.Version assemblyVersion
        Attribute.FileVersion assemblyFileVersion
        Attribute.InformationalVersion assemblyInformationalVersion
        ]
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
    CopyFile docsDir "LICENSE.md"

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

Target "PackageXamarinDistribution" (fun _ ->

    xpkgPack (fun p ->
        {p with
            ToolPath = xpkgExecutable;
            Package = "Portable.Licensing";
            Version = assemblyFileVersion;
            OutputPath = publishDir
            Project = "Portable.Licensing"
            Summary = "Portable.Licensing is a cross platform software licensing framework which allows you to implement licensing into your application or library"
            Publisher = "Nauck IT KG"
            Website = "http://dev.nauck-it.de/projects/portable-licensing"
            Details = "./Xamarin/Details.md"
            License = "License.md"
            GettingStarted = "./Xamarin/GettingStarted.md"
            Icons = ["./Xamarin/Portable.Licensing_512x512.png"; "./Xamarin/Portable.Licensing_128x128.png"]
            Libraries = ["mobile", "./Distribution/lib/portable-net40+sl4+wp7+win8/Portable.Licensing.dll"]
            Samples = ["Android Sample. A simple sample that validates a trial license.", "./Samples/Android.Sample/Android.Sample.sln";
                        "iOS Sample. A simple sample that validates a trial license.", "./Samples/iOS.Sample/iOS.Sample.sln"]
        }
    )

    xpkgValidate (fun p ->
        {p with
            ToolPath = xpkgExecutable;
            Package = "Portable.Licensing";
            Version = assemblyFileVersion;
            OutputPath = publishDir
            Project = "Portable.Licensing"
        }
    )
)

// Dependencies
"Clean" 
    ==> "CreateAssemblyInfo"
    ==> "Build"
    ==> "Test"
    ==> "MergeAssemblies"
    ==> "PreparePackaging"
    ==> "PackgaeZipDistribution"
    ==> "PackageNuGetDistribution"
    ==> "PackageXamarinDistribution"
    ==> "All"
 
// start build
Run <| getBuildParamOrDefault "target" "All"