module Total_Ride_Lines_Chart_Gen

open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

let GetChart() =
    let data = CsvProvider<"Data/TotalRidesOfLinesPerDirection.csv">.GetSample()

(*     
    let GetDirection iDay = 
        ["Heen"; "Terug";] |> List.item (iDay-1)

   let chart = Chart.Combine(data.Rows 
                                |> Seq.sortBy(fun row -> (row.Direction))
                                |> Seq.groupBy(fun row -> row.Direction)
                                |> Seq.map (fun (direction, rows) -> Chart.Column(rows 
                                    |> Seq.sortBy(fun row -> (row.LineName)) 
                                    |> Seq.map (fun row -> row.LineName, row.Count), (GetDirection direction)))).WithLegend(true)
*)
    let chart = Chart.Pie(data.Rows
                |> Seq.groupBy(fun row -> row.LineName) 
                |> Seq.map (fun (key, rows) -> (key, rows |> Seq.sumBy(fun row -> row.Count))), "Aantal ritten per lijn", "").WithLegend(true).WithTitle("Aantal ritten per lijn", false)
                
    

    let total = data.Rows |> Seq.sumBy (fun row -> row.Count) |> float32

    let table = 
        data.Rows
        |> Seq.groupBy(fun row -> row.LineName) 
        |> Seq.map (fun (key, rows) -> (key, (rows |> Seq.sumBy(fun row -> row.Count) |> float32)/total * 100.f))
        |> Seq.map (fun (line, percent) -> sprintf "%s|%f \n" line percent)
        |> Seq.reduce (+)

    chart.ShowChart()
