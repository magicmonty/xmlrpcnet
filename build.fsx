// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Paket
open Fake.AssemblyInfoFile

// Properties
let testDir = "bin/Debug"
let deployDir = "deploy"

let version = "3.0.0.0"

let projects = [
  "src/CookComputing.XmlRpc.csproj"
  "xmlrpcserver/CookComputing.XmlRpcServer.csproj"
  "Test/Test.csproj" ]

// Targets
Target "Clean" (fun _ ->
  !!"**/bin"
  |> DeleteDirs

  !!"**/obj"
  |> DeleteDirs

  DeleteDir deployDir
)

Target "Create AssemblyVersion" (fun _ ->
  CreateCSharpAssemblyInfo "src/AssemblyBuildNumber.cs"
    [Attribute.Version version
     Attribute.InformationalVersion version
     Attribute.FileVersion version]

  CreateCSharpAssemblyInfo "xmlrpcserver/AssemblyBuildNumber.cs"
    [Attribute.Version version
     Attribute.InformationalVersion version
     Attribute.FileVersion version]
)

Target "BuildDebug" (fun _ ->
    projects
    |> MSBuildDebug "" "Build"
    |> Log "DebugBuild-Output: "
)

Target "BuildRelease" (fun _ ->
    projects
    |> MSBuildRelease "" "Build"
    |> Log "ReleaseBuild-Output: "
)

Target "Test" (fun _ ->
    !!(testDir @@ "Test.dll")
      |> NUnit(fun p ->
          { p with DisableShadowCopy = true
                   OutputFile = testDir @@ "TestResults.xml"
                   ToolPath = "packages/NUnit.Runners/tools"})
)

Target "CreatePackage" (fun _ ->
    Pack(fun p ->
        { p with OutputPath = deployDir
                 TemplateFile = "paket.template"
                 WorkingDir = "."
                 Version = version 
                 ToolPath = ".paket/paket.exe" })
)

Target "PushPackage" (fun _ ->
    Push(fun p ->
        { p with WorkingDir = deployDir })
)

// Dependencies
"Clean"
  ==> "Create AssemblyVersion"
  ==> "BuildDebug"
  ==> "Test"
  ==> "BuildRelease"
  ==> "CreatePackage"
  ==> "PushPackage"

// start build
RunTargetOrDefault "BuildRelease"
