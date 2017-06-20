open System

let first = function
    | [||] -> Choice2Of2 (exn "Missing Argument")
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

let report = function
    | Choice1Of2 _ -> 0
    | Choice2Of2 (e:exn) -> cprintf (ConsoleColor.Red) "%s\n" e.Message; 1

open FSharpx.Choice
open IO

[<EntryPoint>]
let main argv =
    first argv
    >>= protect (!!)
    >>= read
    >>= protect Config.parse
    >>= Run.test
    |> report