module ``Run Tests: ``

open Xunit
open FsUnit.Xunit
open Run

[<Theory>]
[<InlineData("foo", "C:\\path\\to\\bar.txt", false)>]
[<InlineData("foo", "C:\\path\\to\\foo.txt", true)>]
[<InlineData("bar", "C:\\path\\to\\foo.txt", false)>]
[<InlineData("bar", "C:\\path\\to\\bar.txt", true)>]
[<InlineData("bar|foo", "C:\\path\\to\\bar.txt", true)>]
[<InlineData("bar|foo", "C:\\path\\to\\foo.txt", true)>]
[<InlineData("bar|foo", "C:\\path\\to\\blah.txt", false)>]
[<InlineData("bar|foo", "C:\\path\\to\\blah.txt", false)>]
[<InlineData(null, "C:\\path\\to\\bar.txt", true)>]
let ``Regexp == Dir = <bool>`` (filter, name, expected : bool) =
    let filter' =
        if isNull filter
            then None
            else Some filter
    filter' == IO.Dir name
    |> should equal expected