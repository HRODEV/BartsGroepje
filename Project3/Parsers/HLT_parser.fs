module HLT_parser
open System.IO

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

let private ParseLine (line:string) =
    {
        OverStap = line.[0] = '1'
        Code = line.Substring(2, 6).Trim()
        RDX = line.Substring(29,6) |> float32
        RDY = line.Substring(36,6) |> float32
        Name = line.Substring(43,49).Replace(" Ca", "").Trim()
    }

let private GetAlStationStoplRecordsFromFile file =
    printfn "Parsing HLT file"
    File.ReadAllLines(file)
    |> Array.map ParseLine
    |> Array.toList

let parse_HLTFile s = GetAlStationStoplRecordsFromFile s

let FilterMetroStops stationStops (rides: PAS_parser.trip list) =
    printfn "Filtering Metro Stations"
    let ritStationCodes = rides |> List.fold (fun state rit -> rit.stops @ state ) [] |> List.map (fun halte -> halte.id)
    stationStops |> List.filter (fun station -> ritStationCodes |> List.exists(fun code -> code = station.Code))

let ConvertStopsToStations (stops: StationStop list) =
    printfn "Creating Stations"
    let groups = stops |> Seq.groupBy (fun station -> station.Name)
    groups 
    |> Seq.map (fun group -> 
        {Name=(group |> fst);
        RDX=((group |> snd) |> Seq.head).RDX;
        RDY=((group |> snd) |> Seq.head).RDY; Stops= (snd group) |> Seq.toList })