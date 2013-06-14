// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System;
open Microsoft.FSharp.Reflection
open System.Text.RegularExpressions

type Agent<'Msg> = MailboxProcessor<'Msg>

let (|ParseRegex|_|) regex str =
  let m = Regex(regex).Match(str)
  if m.Success
  then Some (List.tail [ for x in m.Groups -> x.Value ])
  else None

type Command =
  | Message of string
  | Stop
  with static member fromString = fun str ->
                                    match str with
                                    | ParseRegex "Message (.+)$" [s] ->  Some(Message(s))
                                    | ParseRegex "Stop" [] -> Some(Stop)
                                    | _ -> None

[<EntryPoint>]
let main argv =
  let agent = Agent.Start(fun inbox ->
                                  let rec loop() = async {
                                                      let! msg = inbox.Receive()
                                                      match msg with
                                                      | Message(str) -> printfn "received message from mailbox: '%s'" str
                                                      | _ -> ()
                                                      return! loop()

                                                     }
                                  loop()
                                  )

  let consoleReader = async {
                      let rec loop() =
                          let input = Console.In.ReadLine()
                          if input = null then
                              loop()
                          else
                            let option = Command.fromString input
                            match option with
                                | Some(msg) -> match msg with
                                                | Message(str) ->  agent.Post msg
                                                                   loop()
                                                | Stop -> ()
                                | None -> printfn "Unknown command '%s'" input
                                          loop()

                      loop()
                    }

  consoleReader |> Async.RunSynchronously
  0 // return an integer exit code
