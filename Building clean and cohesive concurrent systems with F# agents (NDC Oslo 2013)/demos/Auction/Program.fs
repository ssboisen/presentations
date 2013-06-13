open System
type Agent<'T> = MailboxProcessor<'T>

[<Measure>] type usd

type AuctionMessage =
    | Bid of float<usd> * (string * Agent<BidderMessage>)

and BidderMessage =
    | BidIsTooLow
    | YourTheHighestBidder
    | YouWon of float<usd>


let r = new System.Random() 
let rndGen = fun () -> r.Next(500,2000)

let createAuctionAgent startprice auctionEndTime = new Agent<AuctionMessage>(fun inbox ->
          let sw = Diagnostics.Stopwatch.StartNew()
          let msgCount = ref 0.
          let rec loop bidprice winner = async {
              let! msg = inbox.TryReceive(500)
              match msg, winner with 
                  | Some(msg), _ when DateTime.UtcNow < auctionEndTime ->
                      printfn "Received bid"
                      msgCount := !msgCount + 1.
                      match msg, winner with
                      | Bid(_, (id, bidder)), Some(wId, _) when id = wId -> bidder.Post(YourTheHighestBidder); return! loop bidprice winner
                      | Bid(bid, (_, bidder)), _ when bid <= bidprice -> bidder.Post(BidIsTooLow); return! loop bidprice winner
                      | Bid(bid, (id, bidder)), _ -> printfn "%A is new highest bidder at %A" id bid
                                                     bidder.Post(YourTheHighestBidder)
                                                     return! loop bid <| Some((id, bidder))
                  | _, Some((id, winner)) when DateTime.UtcNow > auctionEndTime ->
                      printfn "Messagethroughput: %f msg/sec" (!msgCount / sw.Elapsed.TotalSeconds)
                      printfn "Auction ended with winning bid of %A from %s" bidprice id
                      winner.Post(YouWon(bidprice))
                  | _, None when DateTime.UtcNow > auctionEndTime -> printfn "Auction ended with no bids placed"
                  | _ -> return! loop bidprice winner
          }
          loop startprice None)

let createBidderAgent bidderName (auction : Agent<_>) maxBid bidstep = new Agent<BidderMessage>(fun inbox ->
          let rec loop lastBidPrice = async {
              let nextBid = lastBidPrice + bidstep
              let nextBid = if nextBid < maxBid then nextBid else maxBid
              auction.Post(Bid(nextBid, (bidderName, inbox)))
              let! msg = inbox.Receive()
              match msg with
                  | BidIsTooLow -> return! loop nextBid
                  | YourTheHighestBidder -> do! Async.Sleep(rndGen())
                                            return! loop lastBidPrice
                  | YouWon(_) -> ()
          }
          loop 0.0<usd>)

let auction = createAuctionAgent 0.<usd> <| DateTime.UtcNow.AddSeconds(15.)

[<EntryPoint>]
let main argv = 
    auction.Start()
    
    for _ in 1..100000 do
      let bidPrice = LanguagePrimitives.FloatWithMeasure<usd> (float(r.Next(60,180)))
      let bidder = createBidderAgent (Guid.NewGuid().ToString()) auction 900000.<usd> bidPrice
      bidder.Start()
    
    printfn "All bidders have been started"
    Console.ReadLine()
    0 // return an integer exit code
