namespace Audimail

open Chiron
open Chiron.Operators
open FSharpx

type Dir = Dir of string

type Executable = 
    {  Path: Dir
       Extension: string
       Output: string }
    with
    static member FromJson (_:Executable) =
            fun p e o ->
                { Path = (Dir p); Extension = e; Output = o}
        <!> Json.read "path"
        <*> Json.read "estensione"
        <*> Json.read "output"

type Program =
    { Directories: Dir list
      Executables: Executable list }
    with
    static member FromJson (_:Program) =
            fun d e ->
                { Directories = List.map (Dir) d; Executables = e}
        <!> Json.read "directory"
        <*> Json.read "eseguibili"

type Mail =
    { Title: string
      Files: string list }
    with
    static member FromJson (_:Mail) =
            fun t f ->
               { Title = t; Files = f }
        <!> Json.read "titolo" 
        <*> Json.read "files"

type Output =
    { Base: Dir
      Mails: Mail list }
    with
    static member FromJson (_:Output) =
            fun b m ->
               { Base = (Dir b); Mails = m}
        <!> Json.read "base" 
        <*> Json.read "mail"

type Test =
    { Name: string
      Program: Program
      Results: Output list }
    with
    static member FromJson (_:Test) =
            fun t p c ->
               { Name = t; Program = p; Results = c}
        <!> Json.read "nome"
        <*> Json.read "programma"
        <*> Json.read "risultati"

type Config =
    { Tests: Test list
      Log: Dir
      Dest: Dir }
    with
    static member FromJson (_:Config) = json { //lo lascio solo come doc
        let! t = Json.read "test"
        let! l = Json.read "log"
        let! d = Json.read "discoI"
        return { Tests = t; Log = Dir (sprintf "%s%s" d l); Dest = (Dir d)}
    }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Config =
    /// parse the content of the JSON file, unsafe
    let parse (content:string) : Config =
        content
        |> (Json.parse >> Json.deserialize)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Dir =
    
    /// creates a path from a directory, and an optional extension
    let file ext (Dir d) s =
        match ext with
        | Some e -> 
            sprintf "%s%s.%s" d s e
            |> Dir
        | None ->
            sprintf "%s%s" d s
            |> Dir
    
    let simpleFile d s = file None d s