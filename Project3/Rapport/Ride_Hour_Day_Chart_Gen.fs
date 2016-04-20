module Ride_Hour_Day_Chart_Gen

open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

let GetChart() =
    let data = CsvProvider<"Data/CountRidesPerHourAndDayIn90Days.csv">.GetSample()
    let GetDayOfWeek iDay =
        ["Zo"; "Ma"; "Di"; "Wo"; "Do"; "Vr"; "Za"].Item (iDay-1)
    let chart = Chart.Combine(data.Rows
                                (* sort days to ma di wo do vr za zo *)
                                    |> Seq.sortBy(fun row -> (row.Day_Of_Week+6)%8)
                                    |> Seq.groupBy(fun row -> row.Day_Of_Week )
                                    |> Seq.map (fun (day, rows) -> Chart.Line(rows
                                        (* Sort time to normal format *)
                                        |> Seq.sortBy(fun row -> (row.Hour+24) % 27)
                                        |> Seq.map (fun row -> (sprintf "%i:00" row.Hour), row.CountRides/14), (GetDayOfWeek day)))
                                ).WithLegend(true).WithTitle("ritten per uur per dag", false)
                                .WithYAxis(true, "Aantal ritten")
                                .WithXAxis(true, "tijd")
    chart

let GetForm() = 
    let form = GetChart().ShowChart()
    form.Text <- "ritten per uur per dag"
    form