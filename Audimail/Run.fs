module Run

open FSharpx
open Config

type HtmlFile =
    { Title: string
      Dest: IO.Dir
      Content: FSharp.Data.HtmlDocument }

type Result =
    { Output: string
      Log: string list
      Files: HtmlFile list }

/// constructor for HtmlFile
let htmlFile t d c =
    { Title = t; Dest = d; Content = c }

/// compares the filename and an optional name of file
let (==) file path =
    match file with
    | Some name ->
        System.Text.RegularExpressions.Regex(name).IsMatch(IO.name path)
    | None -> true

let inline (!=) file path = not (file == path)

let getFiles ext =
    Choice.collect (IO.getFiles ext)
    
let execute path =
    Choice.mapM (IO.exec path)

/// read and parse all the html files
let readAndParse dest { Base = dir; Mails = ms } =
    Choice.mapM (fun { Title = t; Files = fs} ->
        fs
        |> Choice.mapM (IO.simpleFile dir >> IO.read)
        |> Choice.map (Html.loadFiles >> Html.mergeFiles >> htmlFile t dest)
    ) ms

open Choice

/// executes the program and saves the output and log
let result dest rs ds (e:Executable) =
        fun ls fs -> { Output = e.Output;Log = ls;Files = fs }
    <!> (List.filter (fun f -> e.Except != f)
        <!> getFiles e.Extension ds
        >>= execute e.Path)
    <*> collect (readAndParse dest) rs

let executeTest dest (t:Test) =
    t.Program.Executables
    |> mapM (result dest t.Results t.Program.Directories)

let writeMails ext =
    iter (fun h ->
        let path = IO.file (Some ext) h.Dest h.Title
        IO.write path (Html.toString h.Content)
    )

let writeThenLog path r =
    writeMails r.Output r.Files
    >>. iter (IO.append path) r.Log

let test { Tests = ts; Log = log; Dest = discoI } =
    IO.clearFile log
    >>. Choice.returnM ts
    >>= collect (executeTest discoI)
    >>= iter (writeThenLog log)