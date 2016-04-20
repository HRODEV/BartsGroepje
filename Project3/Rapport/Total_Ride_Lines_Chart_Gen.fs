module Total_Ride_Lines_Chart_Gen

open FSharp.Data
open FSharp.Charting

let GetChart() =
    let data = CsvProvider<"Data/TotalRidesOfLinesPerDirection.csv">.GetSample()
    let chart = Chart.Pie(data.Rows
                |> Seq.groupBy(fun row -> row.LineName)
                |> Seq.map (fun (key, rows) -> (key, rows |> Seq.sumBy(fun row -> (row.Count/14)))), "Aantal ritten per lijn per week", "").WithLegend(true).WithTitle("Aantal ritten per lijn per week", false)
    let total = data.Rows |> Seq.sumBy (fun row -> row.Count) |> float32
    let table =
        data.Rows
        |> Seq.groupBy(fun row -> row.LineName)
        |> Seq.map (fun (key, rows) -> (key, (rows |> Seq.sumBy(fun row -> row.Count) |> float32)/total * 100.f))
        |> Seq.map (fun (line, percent) -> sprintf "%s|%f \n" line percent)
        |> Seq.reduce (+)
    chart

let GetForm() = 
    GetChart().ShowChart()