open System
open System.Data
open System.Data.Common
open FSharp.Data
open FSharp.Charting
open System.Windows.Forms

// let file = System.IO.File.ReadAllText(@"System.Environment.CurrentDirectory\file.csv")

(*let data = CsvProvider<"/Data/CountStopsPerHourInDays.csv">.GetSample()

data.Rows |> Seq.map (fun row -> row.)
*)

printfn "%A" data

let chart = Chart.Line([ for x in 0 .. 10 -> x, x*x ]).ShowChart()

Application.Run(chart)