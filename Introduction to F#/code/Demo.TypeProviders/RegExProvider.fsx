#r "bin/Debug/FSharpx.TypeProviders.Regex.dll"
open FSharpx

type PhoneRegex = Regex< @"(?<AreaCode>^\d{3})-(?<PhoneNumber>\d{3}-\d{4}$)">
let reg = PhoneRegex()
let result = PhoneRegex.IsMatch("425-555-2345")
if result then
    let r = PhoneRegex().Match("425-555-2345")
    
    printfn """AreaCode: %s
PhoneNumber: %s""" r.AreaCode.Value r.PhoneNumber.Value