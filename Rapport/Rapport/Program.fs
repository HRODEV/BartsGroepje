open System
open System.Data
open System.Data.Common
open FSharp.Charting
open System.Windows.Forms

let chart = Chart.Line([ for x in 0 .. 10 -> x, x*x ]).ShowChart()

Application.Run(chart)