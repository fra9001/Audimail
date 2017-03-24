namespace Audimail.IO

open System
open System.IO
open System.Diagnostics
open Audimail.Configurazione
open FSharpx

type IOError = BadPath of Dir
             | FileNotExisting of Dir
             | FileUnreacheable of Dir * string
             | Error of Dir * string

module IO =
    /// gets the files within a specific directory, filtering with a regexp
    let getFiles filter (Dir dir)=
        Directory.GetFiles (dir, filter)
        |> Array.map (Dir)
        |> Array.toList

    /// executes a program with the supplied arguments
    let exec (Dir program) (Dir config) =
        let mutable proc = ProcessStartInfo(program, config)
        proc.UseShellExecute <- false
        proc.RedirectStandardOutput <- true
        proc.CreateNoWindow <- true
        proc.WorkingDirectory <- Path.GetDirectoryName(program)
        let p = Process.Start(proc)
        let log code = sprintf "\n%s: %s\nReturn code: %d\n" program config code
        // printf "%s" (p.StandardOutput.ReadToEnd())
        p.WaitForExit()
        log p.ExitCode

    /// clear a file
    let clearFile (Dir file) =
        use stream = File.OpenWrite (file)
        use writer = new StreamWriter(stream)
        stream.SetLength (0L)

    /// reads the content of a file
    let read (Dir path) = File.ReadAllText (path)

    /// append a string into a file
    let append (Dir dest) text =
        File.AppendAllText (dest, text)

    /// writes a text to a destination
    let write (Dir dest) text =
        File.WriteAllText (dest, text)
    
    /// creates a path from a directory, and an optional extension
    let createPath (Dir dest) name = function
        | Some ext -> Dir (sprintf "%s%s.%s" dest name ext)
        | None -> Dir (sprintf "%s%s" dest name)

    /// executes a IO function catching the error
    let safeIO f a =
        try
            let res = f a
            Choice1Of2 res
        with
        | :? ArgumentException as ae -> Choice2Of2 (BadPath a)
        | :? FileNotFoundException as ff -> Choice2Of2 (FileNotExisting a)
        | :? IOException as ie -> Choice2Of2 (FileUnreacheable (a, ie.Message))
        | e -> Choice2Of2 (Error (a, e.Message))

    /// executes a IO function with 2 params
    let safeIO2 f a b =
        safeIO (fun a' -> f a' b) a

    // safe versions

    let clearFile', read' = (safeIO clearFile), (safeIO read)

    let append', write', exec' =
        (safeIO2 append), (safeIO2 write), (safeIO2 exec)
    
    let getFiles' e d =
        safeIO (getFiles e) d