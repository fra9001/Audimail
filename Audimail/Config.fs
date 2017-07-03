[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Config

open Chiron
open Chiron.Operators
open IO

type Executable = 
    { Path: Dir
      Extension: string
      Output: string
      Except: string option }
    with
    static member FromJson (_:Executable) =
            fun p e o e' ->
                { Path = (!! p); Extension = e; Output = o; Except = e'}
        <!> Json.read "path"
        <*> Json.read "estensione"
        <*> Json.read "output"
        <*> Json.tryRead "eccetto"

type Program =
    { Directories: Dir list
      Executables: Executable list }
    with
    static member FromJson (_:Program) =
            fun d e ->
                { Directories = List.map (!!) d; Executables = e}
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

/// parse the content of the JSON file, unsafe
let parse (content:string) : Config =
    content
    |> (Json.parse >> Json.deserialize)