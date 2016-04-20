module Total_Stops_Per_Station_Chart_Gen

open FSharp.Data
open FSharp.Charting

let GetChart() =
    let data = CsvProvider<"Data/TotalStopsPerStation.csv">.GetSample()
    let chart = Chart.Column(data.Rows
                            |> Seq.take 5
                            |> Seq.map (fun row -> row.StationName, (row.Stops/14/7)), "Top 5 meeste stops per station", "Top 5 meeste stops per station").With3D()
    let table =
        data.Rows
        |> Seq.take 5
        |> Seq.map (fun row -> sprintf "%s|%i \n" row.StationName (row.Stops/14/7))
        |> Seq.reduce (+)
    chart

let GetForm() = 
    GetChart().ShowChart()