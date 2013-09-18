[<Measure>] type m
[<Measure>] type sec
[<Measure>] type kg

let distance = 1.0<m>    
let time = 2.0<sec>    
let speed = 0.5<m/sec>    
let acceleration = 2.0<m/sec^2>    
let force = 5.0<kg m/sec^2>

speed = distance / time

5.0<sec> * speed

[<Measure>] type N = kg m/sec^2

let force2 = 5.0<N>

force = force2

//conversion
[<Measure>] type degC
[<Measure>] type degF
[<Measure>] type degK

let convertDegCToF c = 
    c * 1.8<degF/degC> + 32.0<degF>

let convertDegCToK (c : float<degC>) =
     LanguagePrimitives.FloatWithMeasure<degK>(c / 1.0<degC> + 273.15)

// test    
let f = convertDegCToF 16.0<degC>

let k = convertDegCToK 16.0<degC>

let difficultCalculationThatUsesSI (deg : float<degK>) =
    deg * 8.9

//difficultCalculationThatUsesSI f
difficultCalculationThatUsesSI k

//Using generic measures with type definitions
type CurrencyRate<[<Measure>]'u, [<Measure>]'v> = 
    { Rate: float<'u/'v>; Date: System.DateTime}

[<Measure>] type EUR
[<Measure>] type USD
[<Measure>] type GBP

let mar1 = System.DateTime(2012,3,1)
let eurToUsdOnMar1 = {Rate= 1.2<USD/EUR>; Date=mar1 }
let eurToGbpOnMar1 = {Rate= 0.8<GBP/EUR>; Date=mar1 }

let tenEur = 10.0<EUR>
let tenEurInUsd = eurToUsdOnMar1.Rate * tenEur 