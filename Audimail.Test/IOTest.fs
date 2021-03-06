module ``IO Tests: ``

open FSharpx.Choice
open Xunit
open FsUnit.Xunit
open IO
open System.IO

type TempFile(p:string) =
    let dir = Path.GetDirectoryName(p)
    do
        if not (Directory.Exists (dir)) then
            Directory.CreateDirectory (dir) |> ignore
    member this.Text =
        File.ReadAllText (p)
    member this.Write content =
        File.WriteAllText (p, content)
    interface System.IDisposable with
        member this.Dispose() =
            if File.Exists (p) then
                File.Delete (p)
            if Directory.Exists (dir) then
                Directory.Delete (dir)

let fooFile = "C:\\Temp\\foo"


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
    use file = new TempFile(fooFile)
    file.Write "foo"

    clearFile (!! fooFile)
    |> should be (choice 1)

    file.Text
    |> should be NullOrEmptyString

[<Fact>]
let ``read gets all the content of a file`` () =
    use file = new TempFile(fooFile)
    file.Write "foo"
    choose {
        let! r = read (!! fooFile)
        return r |> should equal (file.Text)
    } |> ignore

[<Fact>]
let ``append appends a text to a file`` () =
    use file = new TempFile(fooFile)

    append (Dir fooFile) "foo"
    |> should be (choice 1)

    file.Text
    |> should equal "foo"

[<Fact>]
let ``write writes some test to a file, overwriting`` () =
    use file = new TempFile(fooFile)
    file.Write "foo"

    write (!! fooFile) "blah"
    |> should be (choice 1)

    file.Text
    |> should not' (equal "foo")