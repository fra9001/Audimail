module IO

open System
open System.IO
open System.Diagnostics
open FSharpx

// not for actual use
type Dir = Dir of string

/// operator constructor for Dir
let (!!) s =
    if Directory.Exists(s) || File.Exists(s)
        then Dir s
        else failwith (sprintf "Dir %s not existing" s)

/// creates a path from a directory, and an optional extension
let file ext (Dir d) s =
    let name =
        match ext with
        | Some e -> 
            sprintf "%s%s.%s" d s e
        | None ->
            sprintf "%s%s" d s
    Dir name

let simpleFile = file None

/// gets the files within a specific directory, filtering with a searchPattern
let getFiles searchPattern (Dir dir) =
    Directory.GetFiles (dir, searchPattern)
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

/// clears a file
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