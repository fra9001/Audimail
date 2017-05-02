module Run

open FSharp.Data
open FSharpx
open Config
open IO
open Choice

type HtmlFile =
    { Title: string
      Dest: Dir
      Content: HtmlDocument }

type ConfiguredExecutable =
    { Executable: Executable
      Configs: Dir list }

type Execution =
    { Output: string
      Log: string list }

type Result =
    { Execution: Execution
      Files: HtmlFile list }

open FSharpx.Choice

let htmlFile t d c =
    { Title = t; Dest = d; Content = c }

let configureExe dirs e =
        fun cs -> { Executable = e; Configs = cs }
    <!> Choice.collect (Choice.protect (IO.getFiles e.Extension)) dirs
    
let runExec (c:ConfiguredExecutable) =
        fun ls -> { Output = c.Executable.Output; Log = ls }
    <!> Choice.mapM (Choice.protect (IO.exec c.Executable.Path)) c.Configs

open FSharpx.Reader

let createPathsFromNames dir =
    List.map (IO.simpleFile dir)
    <!> Reader.asks (fun (m:Mail) -> m.Files)

let htmlfiles dir =
    Choice.mapM (Choice.protect IO.read)
    <!> createPathsFromNames dir

let htmldocs dir =
    Choice.map Html.loadAndMerge
    <!> htmlfiles dir

let files dest dir =
    reader {
        let! (m:Mail) = Reader.ask
        let doc = htmldocs dir m
        return Choice.map (htmlFile m.Title dest) doc
    }

let parse dest { Base = dir; Mails = ms } =
    ms |> Choice.mapM (files dest dir)

open FSharpx.Choice

let getFilesForEveryExec dest rs e =
        fun fs -> { Execution = e; Files = fs }
    <!> (rs |> Choice.collect (parse dest))

let execute dest rs ds (e:Executable) =
    configureExe ds e
    >>= runExec
    >>= getFilesForEveryExec dest rs

open FSharpx.Reader

let executeTest dest (t:Test) =
    t.Program.Executables
    |> Choice.mapM (execute dest t.Results t.Program.Directories)

let createPath ext =
    IO.file (Some ext)
    <!> Reader.asks (fun (h:HtmlFile) -> h.Dest)
    <*> Reader.asks (fun h -> h.Title)

let writeMail ext =
        fun p c -> Choice.protect (IO.write p) c
    <!> createPath ext
    <*> (Html.toString' <!> Reader.asks (fun f -> f.Content))

let writeMails =
    reader {
        let! (r:Result) = Reader.ask
        return r.Files |> Choice.iter (writeMail r.Execution.Output)
    }

let log path =
    Choice.iter (Choice.protect (IO.append path))
    <!> Reader.asks (fun (r:Result) -> r.Execution.Log)

let writeThenLog path =
    writeMails *> log path

open FSharpx.Choice

let test { Tests = ts; Log = log; Dest = discoI } =
    Choice.protect IO.clearFile log
    >>. Choice.returnM ts
    >>= Choice.collect (executeTest discoI)
    >>= Choice.iter (writeThenLog log)