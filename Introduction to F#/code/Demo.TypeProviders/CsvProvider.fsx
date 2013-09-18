#load "../packages/FSharp.Charting.0.84/FSharp.Charting.fsx"
#r "bin/Debug/FSharp.Data.dll"

open FSharp.Data
open FSharp.Charting

type Titanic = CsvProvider<"titanic.csv", IgnoreErrors=true, PreferOptionals=true>

let titanic = Titanic.Load(__SOURCE_DIRECTORY__ + "/titanic.csv")

let survivability (persons : seq<Titanic.Row>) = 
        persons |> Seq.countBy (fun p -> p.survived) 
                |> Seq.map (snd >> float)
                |> List.ofSeq
                |> function 
                        s :: d :: [] -> Some((s / (s + d)) * 100.0)
                        | _ -> None

let survivePerType selector data = 
    data |> Seq.groupBy selector
         |> Seq.choose (fun (key, persons) -> 
                            let ratio = survivability persons
                            match ratio with
                                | None -> None
                                | Some(r) -> Some((key, r)))

let survived x = if x then "Survived" else "Died"

Chart.Pie (titanic.Data |> Seq.countBy (fun p -> p.survived) |> Seq.map (fun (k, c) -> (survived k, c)))

Chart.Bar (survivePerType (fun p -> p.sex) titanic.Data)

Chart.Bar (survivePerType (fun p -> p.pclass) titanic.Data)

Chart.Bar (survivePerType (fun p -> p.age.Value) (titanic.Data |> Seq.filter (fun p -> p.age.IsSome)))