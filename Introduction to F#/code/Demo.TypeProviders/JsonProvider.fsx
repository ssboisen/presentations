#load "../packages/FSharp.Charting.0.84/FSharp.Charting.fsx"
#r "bin/Debug/FSharp.Data.dll"

open FSharp.Data
open FSharp.Charting

type GitCommits = JsonProvider<"github.json">
let githubUri = System.Uri("https://api.github.com/repos/d60/DanskeCommodities/commits?per_page=100")

let run() =
    use httpClient = new System.Net.WebClient()
    httpClient.Headers.Add(System.Net.HttpRequestHeader.Authorization, "Basic c3Nib2lzZW46Ym9pc2tvdjE=");
    
        async {
            let! data = Async.AwaitTask(httpClient.OpenReadTaskAsync(githubUri))
            let commits = GitCommits.Load data

            printfn "Total commits: %i" (Array.length commits)
            
            commits |> Seq.countBy (fun c -> c.Commit.Author.Name)
                    |> Seq.sortBy (snd >> (~-))
                    |> Seq.iter (fun (l, c) -> printfn "%s: %i commits" l c)
    } |> Async.RunSynchronously

run()