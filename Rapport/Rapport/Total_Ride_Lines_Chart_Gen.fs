module Total_Ride_Lines_Chart_Gen

open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

let GetChart =
    let data = CsvProvider<"Data/TotalRidesOfLinesPerDirection.csv">.GetSample()

    let GetDayOfWeek iDay = 
        ["Heen"; "Terug";] |> List.item (iDay-1)

    let chart = Chart.Combine(data.Rows 
                                |> Seq.sortBy(fun row -> (row.Direction))
                                |> Seq.groupBy(fun row -> row.Direction )
                                |> Seq.map (fun (direction, rows) -> Chart.Column(rows 
                                    |> Seq.sortBy(fun row -> (row.LineName)) 
                                    |> Seq.map (fun row -> (sprintf "%A" row.LineName), row.Count), (GetDayOfWeek direction)))).WithLegend(true)

    (chart, "Total_Ride_Line")
