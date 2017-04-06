module ``Run Tests: ``

open Xunit
open FsUnit.Xunit
open FSharp.Data
open IO
open Run

let empty = Dir ""

[<Fact>]
let ``configureExe creates a ConfiguredExecutable`` () =
    configureExe [] {Path=empty;Extension="foo";Output="foo"}
    |> should be (choice 1)

[<Fact>]
let ``runExec creates a Execution`` () =
    runExec ({Path=empty;Output="foo";Configs=[]})
    |> should be (choice 1)

[<Fact>]
let ``createPathsFromNames creates a path for every file name in a directory`` () =
    createPathsFromNames empty {Title="foo";Files=[]}
    |> should be instanceOfType<Dir list>

[<Fact>]
let ``htmlfiles parse all the html file in a directory`` () =
    htmlfiles empty {Title="foo";Files=[]}
    |> should be (choice 1)

[<Fact>]
let ``htmldocs creates all html documents from file contents`` () =
    htmldocs empty {Title="foo";Files=[]}
    |> should be (choice 1)

[<Fact>]
let ``files creates all the HtmlFiles`` () =
    files empty empty {Title="foo";Files=[]}
    |> should be (choice 1)

[<Fact>]
let ``parse gets all the files for every Mail`` () =
    parse empty {Base=empty;Mails=[]}
    |> should be (choice 1)

[<Fact>]
let ``getFilesForEveryExec gets the files for every Execution`` () =
    let data = execution "" []
    getFilesForEveryExec empty [] data
    |> should be (choice 1)

[<Fact>]
let ``execute creates the Result`` () =
    execute empty [] [] {Path=empty;Extension="";Output=""}
    |> should be (choice 1)

[<Fact>]
let ``executeTest executes all tests`` () =
    executeTest empty {Name="";Program={Directories=[];Executables=[]};Results=[]}
    |> should be (choice 1)

[<Fact>]
let ``createPath creates a file path for every HtmlFile`` () =
    let data =
        htmlFile "" empty (HtmlDocument.New ("", []))
    createPath "" data
    |> should be instanceOfType<Dir>

[<Fact>]
let ``writeMail outputs to file the html for the mail`` () =
    let data =
        htmlFile "" empty (HtmlDocument.New ([]))
    writeMail "" data
    |> should be (choice 2) //TODO: test vero