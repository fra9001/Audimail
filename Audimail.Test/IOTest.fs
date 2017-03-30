module ``IO Tests: ``

    open Xunit
    open FsUnit.Xunit
    open Audimail.IO

    let fooFile = "C:\\Temp\\foo"

    let writeFooFile () =
        System.IO.File.WriteAllText (fooFile, "foo")
    
    let readFooFile () =
        let res =
            System.IO.File.ReadAllText (fooFile)
        System.IO.File.Delete (fooFile)
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
        getFiles "*.*" (!! ".\\")
        |> should not' (be Empty)

    [<Fact>]
    let ``exec executes a program with the supplied arguments`` () =
        exec (Dir "cmd") (Dir "/c echo foo")
        |> should endWith "Return code: 0\n"

    [<Fact>]
    let ``clearFile deletes the content of a file`` () =
        writeFooFile ()

        clearFile (!! fooFile)

        readFooFile ()
        |> should be NullOrEmptyString

    [<Fact>]
    let ``read gets all the content of a file`` () =
        writeFooFile()
        read (!! fooFile)
        |> should equal (readFooFile ())

    [<Fact>]
    let ``append appends a text to a file`` () =
        append (Dir fooFile) "foo"
        readFooFile() |> should equal "foo"

    [<Fact>]
    let ``write writes some test to a file, overwriting`` () =
        writeFooFile ()
        write (!! fooFile) "blah"
        readFooFile () |> should not' (equal "foo")