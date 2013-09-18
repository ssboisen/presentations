let printfni indent format =
    printf "%s"(System.String('\t', indent))
    printfn format

let print s = printfn "%s" s

let createFormat input = Printf.TextWriterFormat<_>(input)

let fmttpl (t, p) =
    (createFormat t, p)

let fmttrpl (t, p, p2) =
    (createFormat t, p, p2)   
