module Rides_All_Days_Chart_Gen

open FSharp.Data
open FSharp.Charting

let getChart () =
    let data = CsvProvider<"Data/RidesAllDays.csv">.GetSample()
    let prepared = 
        data.Rows |> Seq.map (fun row -> row.Date, row.Count)
    let chart = Chart.Line (prepared, "Aantal ritten per dag", "aantal ritten per dag")
    chart

let GetForm() = 
    getChart().ShowChart()
