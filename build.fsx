#r "paket:
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Paket
nuget Fake.DotNet.MsBuild
nuget Fake.DotNet.Testing.XUnit2 //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.DotNet
open Fake.DotNet.Testing

let buildDir, testDir, deployDir = "./build/", "./test/", "./deploy/"

let appReferences  =
    !! "./**/*.fsproj"

Target.create "Restore" (fun _ ->
    Paket.restore id
)

Target.create "Clean" (fun _ ->
    Shell.cleanDirs [buildDir; deployDir; testDir]
)

Target.create "Debug" (fun _ ->
    appReferences
    |> MSBuild.runDebug id testDir "Build"
    |> ignore
)

Target.create "Release" (fun _ ->
    appReferences
    -- "./**/*Test.fsproj"
    |> MSBuild.runRelease id buildDir "Build" 
    |> ignore
)

Target.create "Test" (fun _ ->
    !! (testDir + "*Test.dll")
    |> XUnit2.run (fun p -> { p with HtmlOutputPath = Some (testDir + "xunit.html")})
)

open Fake.Core.TargetOperators

"Restore"
  ==>  "Clean"
  ==> "Debug"
  ==> "Test"
  ==> "Release"

// start build
Target.runOrDefault "Release"
