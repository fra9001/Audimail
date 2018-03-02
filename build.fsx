// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

// Directories
let buildDir, testDir, deployDir  = "./build/", "./test/", "./deploy/"

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
    |> Log "Debug-Output: "
)

Target "Release" (fun _ ->
    appReferences
    -- "/**/*Test.fsproj"
    |> MSBuildRelease buildDir "Build" 
    |> Log "Release-Output: "
)

Target "Test" (fun _ ->
    !! (testDir @@ "*Test.dll")
    |> xUnit2 (fun p -> { p 
                            with HtmlOutputPath = Some (testDir @@ "xunit.html")
                                 Parallel = ParallelMode.All })
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
RunTargetOrDefault "Deploy"
