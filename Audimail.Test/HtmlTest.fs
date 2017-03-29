module ``Html Tests: ``

    open Xunit
    open FsUnit.Xunit
    open Audimail
    open FSharp.Data

    let body n =
        let content =
            List.replicate n (HtmlNode.NewElement("span", [HtmlNode.NewText ("foo")]))
        HtmlNode.NewElement ("body", content)

    let doc n =
        HtmlDocument.New([HtmlNode.NewElement ("html", [body n])])

    [<Fact>]
    let ``body gets the body of a supplied htmldocument`` () =
        Html.body (doc 1)
        |> should equal (HtmlNode.elements (body 1))

    [<Fact>]
    let ``merge unites two htmldocuments bodies into one`` () =
        Html.merge (doc 1) (doc 1)
        |> should equal (doc 2)

    [<Fact>]
    let ``toString' remove unwanted unicode spaces and bad html tags`` () =
        Html.toString' (HtmlNode.NewElement("title"))
        |> should equal ""
        Html.toString' (HtmlNode.NewElement("span", [HtmlNode.NewText(" ")]))
        |> should equal "<span>&nbsp;</span>"

    [<Fact>]
    let ``load loads a file content into a html document`` () =
        Html.load ((doc 1).ToString())
        |> should equal (doc 1)