// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

// Directories
let buildDir  = "./build/"
let testDir = "./test/"
let deployDir = "./deploy/"

// Filesets
let appReferences  =
    !! "/**/*.fsproj"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir; testDir]
)

Target "Debug" (fun _ ->
    appReferences
    |> MSBuildDebug testDir "Build"
    |> Log "TestBuild-Output: "
)

Target "Release" (fun _ ->
    appReferences
    -- "/**/*Test.fsproj"
    |> MSBuildRelease buildDir "Build" 
    |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir @@ "*Test.dll")
    |> xUnit2 (fun p -> { p with HtmlOutputPath = Some (testDir @@ "xunit.html") })
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir @@ "Audimail.zip")
)

// Build order
"Clean"
  ==> "Debug"
  ==> "Test"
  ==> "Release"
  ==> "Deploy"

// start build
RunTargetOrDefault "Test"