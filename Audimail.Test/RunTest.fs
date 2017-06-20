module ``Run Tests: ``

open Xunit
open FsUnit.Xunit
open IO
open Run

let empty = Dir ""

[<Fact>]
let ``!= compares an optional filename and a Dir`` () =
    ["C:\\path\\to\\foo.txt";"C:\\path\\to\\bar.txt"]
    |> List.map Dir
    |> List.filter (fun f -> Some "foo.txt" != f)
    |> should equal [Dir "C:\\path\\to\\bar.txt"]