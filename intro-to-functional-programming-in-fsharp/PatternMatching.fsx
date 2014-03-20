(*
  Active Patterns to multiple of some number
  
  Parsing regex' the awesome way!
  
  Capturing Integers as patterns  
*)

































let (|MULTIPLE_OF|_|) a b =
    if b % a = 0 then Some() else None

[3..1000] |> Seq.sumBy (fun x ->
    match x with
        | MULTIPLE_OF 3
        | MULTIPLE_OF 5 -> x 
        | _ -> 0)

let (|ParseRegEx|_|) pattern input =
    let m = System.Text.RegularExpressions.Regex.Match(input, pattern)
    if m.Success 
        then Some(List.tail [for x in m.Groups -> x.Value ])
        else None

let (|Integer|_|) input =
    let (b, r) = System.Int32.TryParse input
    if b then Some(r) else None

let parseData input =
    match input with
        | ParseRegEx "(.+): (\d+)" [name; Integer age] ->
            printfn "%s are %i år gammel" name age
        | _ -> printfn "unable to parse numbers"

parseData "Simon Skov Boisen: 29 år"