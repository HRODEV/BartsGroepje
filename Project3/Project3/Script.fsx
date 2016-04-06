// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

// Define your library scripting code here

#load "Scripts/load-project-debug.fsx"

let stations = Stations.GetAllRecords ()
let ritten = Ritten.ritten ()

let ritStationCodes = ritten |> List.fold (fun state rit -> rit.haltes @ state ) [] |> List.map (fun halte -> Ritten.GetHalteCode halte)

let metroStations = 
    stations |> List.filter (fun station -> ritStationCodes |> List.exists(fun code -> code = station.Code))


metroStations |> List.length
stations |> List.length