module Total_Stops_Per_Station_Last5_Chart_Gen

open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

let GetChart() =
    let data = CsvProvider<"Data/TotalStopsPerStation.csv">.GetSample()

    let chart = Chart.Column(data.Rows
                            |> Seq.sortBy(fun row -> row.Stops)
                            |> Seq.take 5
                            |> Seq.map (fun row -> row.StationName, (row.Stops/14/7)), "Top 5 minste stops per station", "Top 5 minste stops per station").With3D()

    let table =
        data.Rows
        |> Seq.take 5
        |> Seq.map (fun row -> sprintf "%s|%i \n" row.StationName row.Stops)
        |> Seq.reduce (+)

    chart.ShowChart()