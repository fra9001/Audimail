namespace Audimail

open FSharp.Data

module Html =
    /// gets the body from the document
    let body doc = 
        HtmlDocument.body doc
        |> HtmlNode.elements

    /// combines two documents appending the bodies left -> right
    let merge left right =
        HtmlDocument.New(seq {
            yield HtmlNode.NewElement("html", seq {
                yield HtmlNode.NewElement("body",
                    [left;right]
                    |> List.collect body
                )
            })
        })

    /// escapes some strings
    let toString' x =
        // occhio che questi spazi non sono uguali
        x.ToString().Replace(" ", "&nbsp;").Replace("<title />", "")

    /// loads a document from a URI
    let load file = HtmlDocument.Parse file

    /// loads a list of documents
    let loadFiles = List.map load

    /// merges a list of documents into one
    let mergeFiles = List.reduce merge

    /// loads and merges a list of documents
    let loadAndMerge fs =
        fs
        |> loadFiles
        |> mergeFiles