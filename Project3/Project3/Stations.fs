module Stations

open System.IO


#if INTERACTIVE
let locationHLTFile = __SOURCE_DIRECTORY__ + "\RET.HLT"
#else
let locationHLTFile = System.Environment.CurrentDirectory + "\RET.HLT"
#endif

type StationStop = 
    {
        OverStap: bool
        Code: string
        RDX: float32
        RDY: float32
        Name: string
    }
type Station = 
    {
        Name:string
        RDX:float32
        RDY:float32
        Stops: StationStop list
    }

let ParseLine (line:string) =
    {
        OverStap = line.[0] = '1'
        Code = line.Substring(2, 6).Trim()
        RDX = line.Substring(29,6) |> float32
        RDY = line.Substring(36,6) |> float32
        Name = line.Substring(43,49).Replace(" Ca", "").Trim()
    }


let GetAlStationStoplRecordsFromFile file =
    File.ReadAllLines(file)
    |> Array.map ParseLine
    |> Array.toList

let GetAllStationStopRecords () = GetAlStationStoplRecordsFromFile locationHLTFile

let FilterMetroStops stationStops (rides:Ritten.rit list) =
    let ritStationCodes = rides |> List.fold (fun state rit -> rit.haltes @ state ) [] |> List.map (fun halte -> Ritten.GetHalteCode halte)
    stationStops 
    |> List.filter (fun station -> ritStationCodes |> List.exists(fun code -> code = station.Code))

let ConnvertStopsToStations (stops: StationStop list) =
    let groups = stops |> Seq.groupBy (fun station -> station.Name)
    groups 
    |> Seq.map (fun group -> 
        {Name=(group |> fst);
        RDX=((group |> snd) |> Seq.head).RDX;
        RDY=((group |> snd) |> Seq.head).RDY; Stops= (snd group) |> Seq.toList })

