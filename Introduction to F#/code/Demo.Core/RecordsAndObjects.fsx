open System

type Person =
    { Name: string; Job : string; Address : Address }
and Address =
    { Street : string; Number : int; City: string}

let simon = { 
    Name = "Simon"; 
    Job = "Programmer"; 
    Address = {
                Street = "Jens Baggsens Vej";
                Number = 110;
                City = "Aarhus N" }
    }

let moved = { simon with Address = { Street = "Gammelager"; Number = 8; City = "Foldby" } }

printfn "%A" simon
printfn "%A" moved

type MyClass() =
    member val Name = "Simon" with get, set

let myInstance = MyClass()
myInstance.Name <- "Simon Boisen"


type Point(x : float, y : float) =
    let length = 
        let sqr x = x * x
        sqrt <| sqr x + sqr y
    do printfn "Initialized to [%f, %f]" x y
    
    member this.X = x
    member this.Y = y
    member this.Length = length

    new() = new Point(0.0, 0.0)
    
    new(text : string) =        
        if text = null then
            raise <| ArgumentException("text")

        let parts = text.Split([| ',' |]) |> Array.map Double.TryParse

        if parts.Length < 2 || parts |> Array.exists (fst >> not) then
            raise <| ArgumentException("text")

        let (x,y) = parts |> Array.map snd |> Seq.pairwise |> Seq.head
        
        // Calls the primary constructor
        new Point(x, y) 

//Object expressions
let makeResource name = { 
                    new System.Object() with 
                        member this.ToString() = name
                    
                    interface System.IDisposable with
                        member this.Dispose() = printfn "Disposed: %A" this
                }
let useIt =
    use r = makeResource "r1"
    printfn "Doing some of that sweet printing with %A" r
    [5; 6]

//Paremetherized object expression
let Delimiter(delim1 : string, delim2 : string ) = { 
                new System.IFormattable with 
                    member x.ToString(format : string, provider : System.IFormatProvider) =
                      if format = "D" then delim1 + x.ToString() + delim2
                      else x.ToString()
           }