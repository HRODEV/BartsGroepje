module Line_Has_Stations_Chart_Gen

open FSharp.Data
open FSharp.Charting

let GetChart() =
    let data = CsvProvider<"Data/stationsOnLine.csv">.GetSample()
    let chart = Chart.Bar(data.Rows |> Seq.map (fun row -> row.Line, row.Count),"aantal stations per lijn").WithYAxis(true,"aantal stations").WithXAxis(true,"lijn").WithTitle("aantal stations per lijn", false)
    chart

let GetForm() = 
    GetChart().ShowChart()