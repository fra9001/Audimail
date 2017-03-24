namespace Audimail.Test

open Audimail.Configurazione
open Audimail.IO
open Audimail.Html
open FSharp.Data
open FSharpx

type HtmlFile =
    { Title: string
      Dest: Dir
      Content: HtmlDocument }

type ConfiguredExecutable =
    { Path: Dir
      Output: string
      Configs: Dir list }

type Execution =
    { Output: string
      Log: string list }

type Result =
    { Output: string
      Log: string list
      Files: HtmlFile list }

module Choice =
    open FSharpx.Choice
    /// collects the result of a choice over a list
    let collect f xs =
        List.concat <!> Choice.mapM f xs
    
    /// ignores the result of a choice over a list
    let iter f xs =
        ignore <!> Choice.mapM f xs

module TestRun =
    open FSharpx.Choice
    let configuredExecutable p o cs =
        { Path = p; Output = o; Configs = cs }
    
    let execution o ls : Execution =
        { Output = o; Log = ls }
    
    let htmlFile t d c =
        { Title = t; Dest = d; Content = c }
    
    let result o ls fs =
        { Output = o; Log = ls; Files = fs }

    let configureExe dirs (e:Executable) =
        configuredExecutable e.Path e.Output
        <!> Choice.collect (IO.getFiles' e.Extension) dirs
        
    let runExec (c:ConfiguredExecutable) =
        execution c.Output
        <!> Choice.mapM (IO.exec' c.Path) c.Configs
    
    open FSharpx.Reader
    
    let createPathsFromNames dir =
        List.map (fun n -> IO.createPath dir n None)
        <!> Reader.asks (fun (m:Mail) -> m.Files)
    
    let htmlfiles dir =
        Choice.mapM IO.read'
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

    let getFilesForEveryExec dest rs (e:Execution) =
        result e.Output e.Log
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
        reader {
            let! (h:HtmlFile) = Reader.ask
            return IO.createPath h.Dest h.Title (Some ext)
        }

    let writeMail ext =
        IO.write'
        <!> createPath ext
        <*> (Html.toString' <!> Reader.asks (fun f -> f.Content))
    
    let writeMails =
        reader {
            let! (r:Result) = Reader.ask
            return r.Files |> Choice.iter (writeMail r.Output)
        }
    
    let log path =
        reader {
            let! (r:Result) = Reader.ask
            return r.Log |> Choice.iter (fun l -> IO.append' path l)
        }
    
    let writeThenLog path =
        writeMails *> log path
    
    open FSharpx.Choice
    
    let test { Tests = ts; Log = log; Dest = discoI } =
        IO.clearFile' log
        >>. Choice.returnM ts
        >>= Choice.collect (executeTest discoI)
        >>= Choice.iter (writeThenLog log)