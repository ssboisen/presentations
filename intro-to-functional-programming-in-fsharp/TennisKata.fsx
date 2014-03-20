(*
Domain of Tennis:

- Player
- Player Points
- State of Game:
   - Points
   - Advantage
   - Deuce
   - Game
*)







































let random = System.Random()

type Player = A | B
type PlayerPoints = Zero | Fifteen | Thirty | Fourty

type Score =
    | Points of PlayerPoints * PlayerPoints
    | Advantage of Player
    | Deuce
    | Game of Player

let incScore =
    function 
        | Zero -> Fifteen 
        | Fifteen -> Thirty 
        | Thirty -> Fourty 
        | Fourty -> failwith "what?!"

let normalize = 
    function
        | Points(Fourty, Fourty) -> Deuce 
        | score -> score

let scorePoint score point =
    match score, point with
    | Points(Fourty, _), A -> Game A
    | Points(_, Fourty), B -> Game B
    | Points(a, b), A -> normalize (Points (incScore a, b))
    | Points(a, b), B -> normalize (Points (a, incScore b))
    | Deuce, p -> Advantage p
    | Advantage p1, p2 when p1 = p2 -> Game p1
    | Advantage p1, p2 -> Deuce
    | Game _, _ ->  score

let randomWinner () =
    if random.NextDouble() <= 0.5
        then A
        else B

let scoreGame points =
    Seq.scan scorePoint (Points(Zero, Zero)) points

let gameStream =
    seq { while true do
           yield randomWinner() }

gameStream 
|> scoreGame
|> Seq.pairwise
|> Seq.takeWhile (function Game p, Game _ -> printfn "Player: %A won!" p; false | _ -> true)
|> Seq.map snd
|> List.ofSeq