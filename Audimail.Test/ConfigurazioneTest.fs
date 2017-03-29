module ``Configurazione Tests``

    open Xunit
    open FsUnit.Xunit
    open Audimail

    [<Fact>]
    let ``file should create a filepath with the supplied parameters`` () =
        Dir.file None (Dir "C:\\") "foo"
        |> should equal (Dir "C:\\foo")
        Dir.file (Some "exe") (Dir "C:\\") "foo"
        |> should equal (Dir "C:\\foo.exe")