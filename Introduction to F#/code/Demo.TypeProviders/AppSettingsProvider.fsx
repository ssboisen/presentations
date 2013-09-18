#r "bin/Debug/FSharpx.TypeProviders.AppSettings.dll"
open FSharpx

type Settings = AppSettings<"App.config">

printfn "What is awesome: %s" Settings.WhatIsAwesome
printfn "ScaleValue: %i" Settings.OnAsCaleFromOneToTenHowAwesomeIsFsHarp