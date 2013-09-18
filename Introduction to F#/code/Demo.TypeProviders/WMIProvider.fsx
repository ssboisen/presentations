#r "bin/Debug/FSharpx.TypeProviders.Management.dll"
#r "bin/Debug/System.Management.dll"
open System
open System.Management
open FSharpx.TypeProviders.Management

type Local = WmiProvider<"localhost">
let data = Local.GetDataContext()

for d in data.Win32_PerfRawData_NETFramework_NETCLRExceptions do
   printfn "%s %A" d.Name d.NumberofExcepsThrown
