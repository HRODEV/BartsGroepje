open System
open System.Windows.Forms
open FSharp.Charting
open Ride_Hour_Day_Chart_Gen

let chart, chartName = Ride_Hour_Day_Chart_Gen.GetChart

let showChart = chart.ShowChart()

Chart.Save ("Data/"+chartName+".png") chart

Application.Run(showChart)