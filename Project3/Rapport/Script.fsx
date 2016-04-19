#load "Scripts/load-project-debug.fsx"

open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

// let file = System.IO.File.ReadAllText(@"System.Environment.CurrentDirectory\file.csv")

let data = CsvProvider<"Data/CountStopsPerHourInDays.csv">.GetSample()

//data.Rows |> Seq.map (fun row -> row.Hour)


//printfn "%A" data

let chart = Chart.Column(data.Rows |> Seq.map (fun row -> (sprintf "%i:00" row.Hour), row.CountRides)).ShowChart()
//Chart.Line

Application.Run(chart)