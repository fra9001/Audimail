open System
open Audimail
open FSharpx

let first = function
    | [||] -> Choice2Of2 "Missing Argument"
    | xs -> Choice1Of2 xs.[0]

let cprintf c fmt =
    Printf.kprintf 
        (fun x ->
            let old = Console.ForegroundColor
            try
                Console.ForegroundColor <- c;
                Console.Write x
            finally
                Console.ForegroundColor <- old
        ) fmt

let mapError = function
    | BadPath d -> sprintf "Bad Path: %s" (IO.get d)
    | FileNotExisting d -> sprintf "File Not Existing: %s" (IO.get d)
    | FileUnreacheable (d, msg) ->
        sprintf "File Unreacheable: %s\nMessage: %s" (IO.get d) msg
    | Error (d, msg) -> sprintf "Error: %s\nMessage: %s" (IO.get d) msg

let report = function
    | Choice1Of2 _ -> 0
    | Choice2Of2 err -> cprintf (ConsoleColor.Red) "%s\n" err; 1

let parse content =
    content
    |> Choice.protect (Config.parse)
    |> Choice.mapSecond (fun ex -> sprintf "Error: %s" ex.Message)

let step f x =
    f x
    |> Choice.mapSecond mapError

open FSharpx.Choice
open Audimail.IO

[<EntryPoint>]
let main argv =
    (!!)
    <!> first argv
    >>= step IO.read'
    >>= parse
    >>= step Run.test
    |> report