open System
open System.Drawing
open System.Windows.Forms
open FSharp.Charting
open FSharp.Markdown
open FSharp.Markdown.Pdf
open Ride_Hour_Day_Chart_Gen
open Total_Ride_Lines_Chart_Gen
open Line_Has_Stations_Chart_Gen

let Charts = [Ride_Hour_Day_Chart_Gen.GetChart; 
              Total_Ride_Lines_Chart_Gen.GetChart; 
              Line_Has_Stations_Chart_Gen.GetChart]

let ProcessCharts (chart : ChartTypes.GenericChart, name : String) =
    let ShowChart = chart.ShowChart()
    ShowChart.Width <- 600
    chart.CopyAsBitmap().Save("Data/"+name+".png")
    ShowChart

let processed = Charts
                |> List.map(fun (chart, name) -> ProcessCharts(chart, name))

let generateRapport() =
    let newFile = Charts 
                  |> List.fold (fun (acc:string) (_, name) -> acc.Replace((sprintf "{|%s|}" name), (sprintf "Data/%s.png" name))) (System.IO.File.ReadAllText(@"format.md"))
    Markdown.TransformPdf(newFile, "Rapport_Barts_Groepje.pdf")

generateRapport()
Application.Run(processed.Head)