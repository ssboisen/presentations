open System
open System.Threading

type Agent<'T> = MailboxProcessor<'T>

type BorderGatewayProtocol =
    | DoConnect of Agent<BorderGatewayProtocol>
    | ACK
    | Open
    | KeepAlive
    | Update
    | Notification

 let printState state =
  sprintf "Thread #%i in %s state%s" System.Threading.Thread.CurrentThread.ManagedThreadId state Environment.NewLine

let printAgent = Agent.Start(fun inbox -> 
                      let rec loop() = async {
                        let! msg = inbox.Receive()
                        printfn "%s" msg
                        return! loop()
                        }
                      loop())

let bgp (peer : Agent<BorderGatewayProtocol> option) = Agent.Start(fun inbox ->
  let rec idle() = async {
    printAgent.Post(printState "idle")
    match peer with
      | Some peer ->
                peer.Post(DoConnect(inbox));
                return! connect peer
      | None -> let! msg = inbox.Receive()
                match msg with
                  | DoConnect peer -> return! connect peer
                  | _ -> return! idle()
    }
  
  and connect peer = async {
    printAgent.Post(printState "connect")
    peer.Post(ACK)
    let! msg = inbox.Receive()
    match msg with
      | ACK -> return! opensent peer
      | _ -> return! active peer 
    }
  
  and opensent peer = async {
    printAgent.Post(printState "opensent")
    peer.Post(Open)
    let! msg = inbox.Receive()
    match msg with
      | Open -> return! openconfirm peer
      | _ -> return! idle()
    }
  
  and active peer = async {
    return! active peer
    }
  
  and openconfirm peer = async {
    printAgent.Post(printState "openconfirm")
    peer.Post(KeepAlive)
    let! msg = inbox.Receive()
    match msg with
      | KeepAlive -> return! established peer
      | _ as s -> failwithf "Invalid msg %A received" s
    }
  
  and established peer = async {
    printAgent.Post(printState "established")
    let! msg = inbox.Receive()
    match msg with
      | KeepAlive -> return! established peer
      | Update -> return! established peer
      | Notification -> return! idle()
      | _ as s -> failwithf "Invalid msg %A received" s
  }
  idle())

[<EntryPoint>]
let main argv = 
    let p1 = bgp None
    Thread.Sleep(TimeSpan.FromMilliseconds(800.))
    let p2 = bgp (Some(p1))
    System.Console.ReadLine()
    0 // return an integer exit code
