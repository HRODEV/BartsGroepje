// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

// Define your library scripting code here

#load "Scripts/load-project-debug.fsx"

let stops = Stations.GetAllStationStopRecords ()
let ritten = Ritten.ritten ()

let metroStops = Stations.FilterMetroStops stops ritten
let metroStations = Stations.ConnvertStopsToStations metroStops

open System.IO

//let filePath = __SOURCE_DIRECTORY__ + "\points.csv"

//let wr = new System.IO.StreamWriter(filePath)
//"name,x,y\n" + (metroStations |> List.map (fun st -> sprintf "%s,%f,%f\n" st.Name st.RDX st.RDY) |> List.fold (+) "") |> wr.Write
//wr.Close()