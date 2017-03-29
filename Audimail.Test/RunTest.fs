module ``Run Tests: ``

    open Xunit
    open FsUnit.Xunit
    open FSharp.Data
    open Audimail

    [<Fact>]
    let ``configureExe creates a ConfiguredExecutable`` () =
        Run.configureExe [] {Path=Dir "";Extension="foo";Output="foo"}
        |> should be (choice 1)
    
    [<Fact>]
    let ``runExec creates a Execution`` () =
        Run.runExec ({Path=Dir "";Output="foo";Configs=[]})
        |> should be (choice 1)
    
    [<Fact>]
    let ``createPathsFromNames creates a path for every file name in a directory`` () =
        Run.createPathsFromNames (Dir "") {Title="foo";Files=[]}
        |> should not' (be Empty)
    
    [<Fact>]
    let ``htmlfiles parse all the html file in a directory`` () =
        Run.htmlfiles (Dir "") {Title="foo";Files=[]}
        |> should be (choice 1)
    
    [<Fact>]
    let ``htmldocs creates all html documents from file contents`` () =
        Run.htmldocs (Dir "") {Title="foo";Files=[]}
        |> should be (choice 1)
    
    [<Fact>]
    let ``files creates all the HtmlFiles`` () =
        Run.files (Dir "") (Dir "") {Title="foo";Files=[]}
        |> should be (choice 1)
    
    [<Fact>]
    let ``parse gets all the files for every Mail`` () =
        Run.parse (Dir "") {Base=Dir "";Mails=[]}
        |> should be (choice 1)
    
    [<Fact>]
    let ``getFilesForEveryExec gets the files for every Execution`` () =
        let data =
            Run.execution "" []
        Run.getFilesForEveryExec (Dir "") [] data
        |> should be (choice 1)

    [<Fact>]
    let ``execute creates the Result`` () =
        Run.execute (Dir "") [] [] {Path=Dir "";Extension="";Output=""}
        |> should be (choice 1)
    
    [<Fact>]
    let ``executeTest executes all tests`` () =
        Run.executeTest (Dir "") {Name="";Program={Directories=[];Executables=[]};Results=[]}
        |> should be (choice 1)
    
    [<Fact>]
    let ``createPath creates a file path for every HtmlFile`` () =
        let data =
            Run.htmlFile "" (Dir "") (HtmlDocument.New ("", []))
        Run.createPath "" data
        |> should be instanceOfType<Dir>

    [<Fact>]
    let ``writeMail outputs to file the html for the mail`` () =
        let data =
            Run.htmlFile "" (Dir "") (HtmlDocument.New ("", []))
        Run.writeMail "" data
        |> should be (choice 1)