module ``IO Tests: ``

open FSharpx.Choice
open System.IO
open Xunit
open FsUnit.Xunit
open IO

let fooFile = "C:\\Temp\\foo"

let writeFooFile () =
    File.WriteAllText (fooFile, "foo")

let readFooFile () =
    let res =
        File.ReadAllText (fooFile)
    File.Delete (fooFile)
    res

[<Fact>]
let ``(!!) checks the path`` () =
    (!! ".\\") |> should be instanceOfType<Dir>
    shouldFail (fun _ -> (!! ".\\foo") |> ignore)

[<Fact>]
let ``file should create a filepath with the supplied parameters`` () =
    file None (Dir "C:\\") "foo"
    |> should equal (Dir "C:\\foo")
    file (Some "exe") (Dir "C:\\") "foo"
    |> should equal (Dir "C:\\foo.exe")

[<Fact>]
let ``getFiles gets all files in a directory with the spedified filter`` () =
    choose {
        let! r = getFiles "*.*" (!! ".\\")
        return r |> should not' (be Empty)
    } |> ignore

[<Fact>]
let ``exec executes a program with the supplied arguments`` () =
    choose {
        let! r = exec (Dir "cmd") (Dir "/c echo foo")
        return r |> should endWith "Return code: 0\n"
    } |> ignore

[<Fact>]
let ``clearFile deletes the content of a file`` () =
    writeFooFile ()

    clearFile (!! fooFile)
    |> should be (choice 1)

    readFooFile ()
    |> should be NullOrEmptyString

[<Fact>]
let ``read gets all the content of a file`` () =
    writeFooFile()
    choose {
        let! r = read (!! fooFile)
        return r |> should equal (readFooFile ())
    } |> ignore

[<Fact>]
let ``append appends a text to a file`` () =
    append (Dir fooFile) "foo"
    |> should be (choice 1)
    readFooFile() |> should equal "foo"

[<Fact>]
let ``write writes some test to a file, overwriting`` () =
    writeFooFile ()
    write (!! fooFile) "blah"
    |> should be (choice 1)
    readFooFile () |> should not' (equal "foo")