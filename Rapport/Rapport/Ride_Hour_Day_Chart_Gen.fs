module Ride_Hour_Day_Chart_Gen
open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

(*
//let chart = Chart.FastLine(data.Rows |> Seq.map (fun row -> (sprintf "%i" row.Day_Of_Week), row.CountRides), "", "Ride stops of 90 days").WithLegend(true, "Legenda")

//let chart = [ for row in data.Rows -> row.Day_Of_Week, row.CountRides ]
    //          |> Chart.FastLine

let showChart = chart.ShowChart()

//Chart.Save "Chart/.png" chart
*)

let GetChart =
    let data = CsvProvider<"Data/CountRidesPerHourAndDayIn90Days.csv">.GetSample()

    let GetDayOfWeek iDay = 
        ["zo"; "ma"; "di"; "wo"; "do"; "vr"; "za"] |> List.item (iDay-1)

    let chart = Chart.Combine(data.Rows 
                                |> Seq.sortBy(fun row -> (row.Day_Of_Week+6)%8)
                                |> Seq.groupBy(fun row -> row.Day_Of_Week )
                                |> Seq.map (fun (day, rows) -> Chart.Line(rows |> Seq.sortBy(fun row -> (row.Hour+24) % 27) |> Seq.map (fun row -> (sprintf "%i:00" row.Hour), row.CountRides), (GetDayOfWeek day)))).WithLegend(true)

    (chart, "Ride_Hour_Day")
