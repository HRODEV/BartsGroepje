module SQL
open PAS_parser
open HLT_parser
open VTN_parser
open System.IO
open System

let private EscapeApostraphe    (s: string) = s.Replace("'", "''")

let private makeStationQuery    (x: Station)                   = System.String.Format ("INSERT INTO `station` VALUES ('{0}', {1}, {2});", EscapeApostraphe x.Name, x.RDX, x.RDY)
let private makePlatformQuery   (x: Station) (y: StationStop)  = System.String.Format("INSERT INTO `perron` VALUES ('{0}', '{1}', {2}, {3});", y.Code, EscapeApostraphe x.Name, y.RDX, y.RDY)
let private makeTripQuery       (x: trip)                      = System.String.Format("INSERT INTO `trip` VALUES ({0}, {1}, {2}, {3});", x.trip_id, x.line_id, x.departure, x.direction)
let private makeStopQuery       (x: trip) (y: stop)            = System.String.Format("INSERT INTO `stop` VALUES ('{0}', {1}, '{2}', '{3}');", y.id, x.trip_id, y.arrival, y.departure)
let private makeActiveOnQuery   (x: trip) (y: System.DateTime) = System.String.Format("INSERT INTO `stop` VALUES ('{0}', {1}, {2}, {3});", x.trip_id, y.Year, y.Month, y.Day)

let private WriteSQLFile (fname: string) (seq: string seq) =
    printfn "Creating SQL file %A: " fname
    let outf = new StreamWriter(fname)
    Seq.iter (fun (x: string) ->
        outf.WriteLine(x)
        outf.Flush()
    ) seq
    ()

let CreateStationQuery (x: seq<Station>)    = x |> Seq.map  (fun x -> makeStationQuery x) |> WriteSQLFile "Stations.sql"
let CreatePlatformQuery (x: seq<Station>)   = x |> Seq.map  (fun x -> List.map (fun y -> makePlatformQuery x y) x.Stops) |> Seq.fold (fun a s -> s @ a) [] |> List.ofSeq |> WriteSQLFile "Platforms.sql"
let CreateTripQuery (x: trip list)          = x |> List.map (fun x -> makeTripQuery x) |> Seq.ofList |> WriteSQLFile "Trips.sql"
let CreateStopQuery (x: trip list)          = x |> List.map (fun x -> List.map( fun y -> makeStopQuery x y) x.stops) |> List.fold (fun a s -> s @ a) [] |> Seq.ofList |> WriteSQLFile "Stops.sql"
let CreateActiveOnQuery (x: trip list)      = x |> List.map (fun x -> List.map( fun y -> makeActiveOnQuery x y) x.active_days) |> List.fold (fun a s -> s @ a) [] |> Seq.ofList |> WriteSQLFile "ActiveOn.sql"
