open System
open System.Windows.Forms
open FSharp.Charting
open FSharp.Markdown
open FSharp.Markdown.Pdf
open Ride_Hour_Day_Chart_Gen
open Total_Ride_Lines_Chart_Gen

let chart, chartName = Ride_Hour_Day_Chart_Gen.GetChart 
let chart', chartName' = Total_Ride_Lines_Chart_Gen.GetChart

let showChart = chart.ShowChart()
let showChart' = chart'.ShowChart()

Chart.Save ("Data/"+chartName+".png") chart
Chart.Save ("Data/"+chartName'+".png") chart'

let generateRapport() =
    let file = System.IO.File.ReadAllText(@"format.md")
    let file = file.Replace("{|CHART1|}", ("Data/"+chartName+".png")).Replace("{|CHART2|}", ("Data/"+chartName'+".png"))

    Markdown.TransformPdf(file, "Rapport_Barts_Groepje.pdf")

generateRapport()
showChart' |> ignore
Application.Run(showChart')