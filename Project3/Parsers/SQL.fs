module SQL
open PAS_parser
open HLT_parser
open VTN_parser
open System.IO
open System

let private EscapeApostraphe    (s: string) = s.Replace("'", "''")

let private makeStationQuery    (x: Station)                   = System.String.Format ("INSERT INTO  [retdb].[dbo].[Stations] VALUES ('{0}', {1}, {2});", EscapeApostraphe x.Name, x.RDX, x.RDY)
//let private makePlatformQuery   (x: Station) (y: StationStop)  = System.String.Format("INSERT INTO  [retdb].[dbo].[Platforms] VALUES ('{0}', '{1}', {2}, {3});", y.Code, EscapeApostraphe x.Name, y.RDX, y.RDY)
let private makeTripQuery       (x: trip)                      = System.String.Format("INSERT INTO  [retdb].[dbo].[Rides] VALUES ({0}, {1}, {2}, {3});", x.trip_id, x.line_id, x.departure, x.direction)
let private makeStopQuery       (x: trip) (y: stop)            = System.String.Format("INSERT INTO `stop` VALUES ('{0}', {1}, '{2}', '{3}');", y.id, x.trip_id, y.arrival, y.departure)
let private makeActiveOnQuery   (x: trip) (y: System.DateTime) = System.String.Format("INSERT INTO `stop` VALUES ('{0}', {1}, {2}, {3});", x.trip_id, y.Year, y.Month, y.Day)

(*

    Declare @lastStationID int;

    set @lastStationID = SCOPE_IDENTITY();

    select @lastStationID;
    select @lastStationID;
    select @lastStationID;
*)

let makePlatformQuery' (station:Station) =
    let StationQ = makeStationQuery station
    let getscopeID = "\nset @lastStationID = SCOPE_IDENTITY(); \n"
    let platforms = 
            @"INSERT INTO [retdb].[dbo].[Platforms]
               ([Code]
               ,[X]
               ,[Y]
               ,[StationID])
         VALUES " + (station.Stops |> List.map (fun stop -> sprintf "('%s','%f','%f',@lastStationID)" stop.Code stop.RDX stop.RDY )  |> String.concat ",")

    StationQ + getscopeID + platforms


let private WriteSQLFile (fname: string) (seq: string seq) =
    printfn "Creating SQL file %A: " fname
    let outf = new StreamWriter(fname)
    Seq.iter (fun (x: string) ->
        outf.WriteLine(x)
        outf.Flush()
    ) seq
    ()

let dToString (d:DateTime) =
    //yyyymmdd HH:MM:SS
    sprintf "%4i-%2i-%2i %2i:%2i:00" d.Year d.Month d.Day d.Hour d.Minute

let makeTripQuery' (t: trip) =
    let dates = t.active_days //|> List.filter (fun d -> d < new DateTime(2015, 12, 22))
    let startTime = t.stops.[0].departure
    let getScopeID = "\nset @lastTripID = SCOPE_IDENTITY();"
    let sql =
        (dates |> List.map ( fun d ->
            //need to add connection to line
            let ridequery = (sprintf "INSERT INTO [retdbim].[dbo].[Rides] VALUES ('%s', '%i', '%i');" ((d + startTime)  |> dToString)) t.line_id t.direction
            //time, tripid, platformid
            let stopquery =  (t.stops |> List.map (fun s -> (sprintf @"INSERT INTO [retdbim].[dbo].[RideStops] VALUES ('%s', @lastTripID, (SELECT TOP 1 [id] FROM [retdbim].[dbo].[Platforms] WHERE [code] = '%s'));" ((d + s.arrival) |> dToString) s.id)) |> List.fold (+) "" )
            (ridequery + getScopeID + stopquery)
        ) |> List.fold (+) "")
    "BEGIN TRANSACTION t\n"+ sql+"\nCOMMIT TRANSACTION t"

//let CreateStationQuery (x: seq<Station>)    = x |> Seq.map  (fun x -> makeStationQuery x) |> WriteSQLFile "Stations.sql"
let CreatePlatformQuery (x: seq<Station>)   = seq{ yield "Declare @lastStationID int;"; yield! (x |> Seq.map  (fun x -> makePlatformQuery' x)) }|> WriteSQLFile "Platforms.sql"
let CreateTripQuery (x: trip seq) n         = seq{ yield "Declare @lastTripID int;"; yield! (x |> Seq.map (fun s -> makeTripQuery' s) ) }|> WriteSQLFile ("trips/Trips"+n.ToString()+".sql")