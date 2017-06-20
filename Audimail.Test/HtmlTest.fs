module ``Html Tests: ``

open Xunit
open FsUnit.Xunit
open Html
open FSharp.Data

let body' n =
    let content =
        List.replicate n (HtmlNode.NewElement("span", [HtmlNode.NewText ("foo")]))
    HtmlNode.NewElement ("body", content)

let doc n =
    HtmlDocument.New([HtmlNode.NewElement ("html", [body' n])])

[<Fact>]
let ``body gets the body of a supplied htmldocument`` () =
    body (doc 1)
    |> should equal (HtmlNode.elements (body' 1))

[<Fact>]
let ``merge unites two htmldocuments bodies into one`` () =
    merge (doc 1) (doc 1)
    |> should equal (doc 2)

[<Fact>]
let ``toString' remove unwanted unicode spaces and bad html tags`` () =
    toString (HtmlNode.NewElement("title"))
    |> should equal ""
    toString (HtmlNode.NewElement("span", [HtmlNode.NewText(" ")]))
    |> should equal "<span>&nbsp;</span>"

[<Fact>]
let ``load loads a file content into a html document`` () =
    load ((doc 1).ToString())
    |> should equal (doc 1)

[<Fact>]
let ``mergeFiles work on a empty list too`` () =
    mergeFiles []
    |> should be instanceOfType<HtmlDocument>