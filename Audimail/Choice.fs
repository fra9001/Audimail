module Choice

open FSharpx.Choice
/// collects the result of a choice over a list
let collect f xs =
    List.concat <!> mapM f xs

/// ignores the result of a choice over a list
let iter f xs =
    ignore <!> mapM f xs