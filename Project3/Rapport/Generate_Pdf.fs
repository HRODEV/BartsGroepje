module Generate_Pdf
open System
open System.Drawing
open System.Windows.Forms
open FSharp.Charting
open FSharp.Markdown
open FSharp.Markdown.Pdf
open Ride_Hour_Day_Chart_Gen
open Total_Ride_Lines_Chart_Gen
open Line_Has_Stations_Chart_Gen
open Total_Stops_Per_Station_Chart_Gen
open Total_Stops_Per_Station_Last5_Chart_Gen
open Rides_All_Days_Chart_Gen

let Charts = [Ride_Hour_Day_Chart_Gen.GetChart(), "Ride_Hour_Day"; 
              Total_Ride_Lines_Chart_Gen.GetChart(), "Total_Ride_Lines";
              Line_Has_Stations_Chart_Gen.GetChart(), "Line_Has_Stations";
              Total_Stops_Per_Station_Chart_Gen.GetChart(), "Total_Stops_Per_Station_Top5";
              Total_Stops_Per_Station_Last5_Chart_Gen.GetChart(), "Total_Stops_Per_Station_Last5";
              Rides_All_Days_Chart_Gen.getChart(), "Rides_All_Days";
              ]

let ProcessChart (chart : ChartTypes.GenericChart, name : String) =
    let ShowChart = chart.ShowChart()
    ShowChart.Width <- 600
    chart.CopyAsBitmap().Save("Data/"+name+".png")
    ShowChart

let processed = Charts
                |> List.map(fun (chart, name) -> ProcessChart(chart, name))
                |> List.iter(fun form -> form.Close();form.Dispose())

let GeneratePdf() =
    let newFile = (Charts 
                  |> List.fold (fun (acc:string) (_, name) -> acc.Replace((sprintf "{|%s|}" name), (sprintf "Data/%s.png" name))) (System.IO.File.ReadAllText(@"format.md")))
                    .Replace("{|date|}", System.DateTime.Now.ToShortDateString().Replace("-", "\-"))
    let filePath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Rapport_Barts_Groepje.pdf")
    Markdown.TransformPdf(newFile, filePath)
    filePath