module ``IO Tests: ``

    open Xunit
    open FsUnit.Xunit
    open Audimail

    let fooFile = "C:\\Temp\\foo"

    let writeFooFile () =
        System.IO.File.WriteAllText (fooFile, "foo")
    
    let readFooFile () =
        let res =
            System.IO.File.ReadAllText (fooFile)
        System.IO.File.Delete (fooFile)
        res
    
    [<Fact>]
    let ``getFiles gets all files in a directory with the spedified filter`` () =
        IO.getFiles "*.*" (Dir ".\\")
        |> should not' (be Empty)

    [<Fact>]
    let ``exec executes a program with the supplied arguments`` () =
        IO.exec (Dir "cmd") (Dir "/c echo foo")
        |> should endWith "Return code: 0\n"

    [<Fact>]
    let ``clearFile deletes the content of a file`` () =
        writeFooFile ()

        IO.clearFile (Dir fooFile)

        readFooFile ()
        |> should be NullOrEmptyString

    [<Fact>]
    let ``read gets all the content of a file`` () =
        writeFooFile()
        IO.read (Dir fooFile)
        |> should equal (readFooFile ())

    [<Fact>]
    let ``append appends a text to a file`` () =
        IO.append (Dir fooFile) "foo"
        readFooFile() |> should equal "foo"

    [<Fact>]
    let ``write writes some test to a file, overwriting`` () =
        writeFooFile ()
        IO.write (Dir fooFile) "blah"
        readFooFile () |> should not' (equal "foo")