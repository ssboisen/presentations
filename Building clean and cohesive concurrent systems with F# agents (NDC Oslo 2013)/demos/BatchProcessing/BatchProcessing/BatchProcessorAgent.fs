module BatchProcessing
open System
open System.Threading
type Agent<'T> = MailboxProcessor<'T>

type BatchProcessorAgent<'T>(batchSize, timeout, ?eventContext:SynchronizationContext) =

  let batchProduced = new Event<'T[]>()
  let agent = Agent<'T>.Start(fun inbox ->
                    let triggerEvent batch = match eventContext with None -> batchProduced.Trigger(batch) | Some ctx -> ctx.Post((fun _ -> batchProduced.Trigger(batch)), null)
                    let rec loop messages dt = async {
                      let t = DateTime.Now
                      let! msg = inbox.TryReceive(max 0 dt)
                      let dt = dt - int (DateTime.Now - t).TotalMilliseconds
                      match msg with
                        | Some(msg) when List.length messages = batchSize - 1 ->
                            msg :: messages |> List.rev |> Array.ofList |> triggerEvent
                            return! loop [] timeout
                        | Some(msg) -> 
                            return! loop (msg :: messages) timeout
                        | None when List.length messages > 0 -> 
                            messages |> List.rev |> Array.ofList |> triggerEvent
                            return! loop [] dt
                        | None -> return! loop messages dt }
                    loop [] timeout)

  [<CLIEvent>]
  member x.BatchProduced = batchProduced.Publish
  
  member x.Enqueue v = agent.Post(v)