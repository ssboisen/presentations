#load "Utils.fsx"
open Utils
open System

let (|MulOf|_|) by n = if n%by=0 then Some MulOf else None

let fizzbuzz = function 
  | MulOf 3 & MulOf 5 -> "FizzBuzz" 
  | MulOf 3 -> "Fizz" 
  | MulOf 5 -> "Buzz" 
  | i -> string i

[ 1..100 ] |> Seq.iter (fizzbuzz >> print)

open System.Text.RegularExpressions

//Regex pattern matcher
let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

//Date parser
let (|Date|_|) input =
    let (s, dt) = DateTime.TryParse(input)
    if s then Some(dt) else None

//Multiple input types
let (|Phone|Email|Unknown|) input =
    match input with
       | Regex @"\(([0-9]{3})\)[-. ]?([0-9]{3})[-. ]?([0-9]{4})" [ area; prefix; suffix ] ->
            Phone(area, prefix, suffix)
       | Regex @"(\w+@[a-zA-Z0-9]+\.[a-zA-Z]{2,4})" [email] ->
            Email(email)
       | _ -> Unknown

let textinput = ref "(012)-345-6789"
//textinput := "ssb@d60.dk"
//textinput := "2013-09-17"

match !textinput with
    | Phone (area, prefix, suffix) ->
        printfn "Area: %s" area
        printfn "Prefix: %s" prefix
        printfn "Suffix: %s" suffix
    | Email (email) -> printfn "Email: %s" email
    | Date date -> printfn "Date: %A" date
    | _ -> printfn "Unable to parse input"