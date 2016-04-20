module PAS_parser
open System
open System.IO

let PAS_file = System.Environment.CurrentDirectory + "/RET.PAS"

#region // type definitions
type stop =
    {
        id          :   string
        arrival     :   TimeSpan
        departure   :   TimeSpan
    }

type trip =
    {
        transport_type  :   string
        line_id         :   int
        direction       :   int
        trip_id         :   int
        stops           :   stop list
        active_days     :   System.DateTime list
        departure       :   TimeSpan
    }
    with
    static member Zero =
        {
            transport_type  =   ""
            line_id         =   -1;
            direction       =   -1;
            trip_id         =   -1;
            stops           =   [];
            active_days     =   [];
            departure       =   new TimeSpan()
        }
#endregion

#region // Parses start date before the rest, and uses the VTN to get all the dates.
let private parseStartDate (s: string) =
    System.DateTime(
        s.Substring(5,4) |> int,
        s.Substring(3,2) |> int,
        s.Substring(1,2) |> int
    )
let private scheduleDates = System.IO.File.ReadAllLines PAS_file |> fun s -> parseStartDate s.[1] |> fun bd -> VTN_parser.GetVTNRecords bd
#endregion

#region// All the individual parsing functions for each line
let private parseStop (s: string) =
    match s.[0] with
    | '+'               -> { id = s.Substring(1,6);
                             arrival = TimeSpan(s.Substring (9,2) |> int, s.Substring (11,2) |> int, 0);
                             departure = TimeSpan(s.Substring (14,2) |> int, s.Substring (16,2) |> int, 0);
                           }
    | '.' | '<' | '>'   -> { id = s.Substring(1,6);
                             arrival = TimeSpan(s.Substring (9,2) |> int, s.Substring (11,2) |> int, 0);
                             departure = TimeSpan(s.Substring (9,2) |> int, s.Substring (11,2) |> int, 0);
                           }
    |  _                -> failwith "Incorrect format thrown into ParseStop"

let private parseTripInfo (s: string) (t: trip) =
    { t with
        transport_type  =   s.Substring(1,1);
        line_id         =   (s.Substring(10,3) |> int) - 5;
        direction       =   s.Substring(14,1) |> int;
        trip_id         =   s.Substring(16,5) |> int;
    }

let private parseActiveDays (s: string) = s.Substring (1) |> int |> fun s -> scheduleDates.Item s
#endregion

#region// Main parsing functions

let private castLine (s: string) (t: trip) =
    match s.[0] with
    | '!'                       -> t
    | '&'                       -> t
    | '#'                       -> parseTripInfo s t
    | '>' | '.' | '+'           -> { t with stops = parseStop s :: t.stops }
    | '<'                       -> { t with stops = List.rev(parseStop s :: t.stops) }
    | '-'                       -> { t with active_days = parseActiveDays s }
    |  _  -> failwith ("Passed wrong line to CastLine: " + s)

let rec private splitTrip acc (reader: StreamReader) =
    let rec internalLoop s acc =
        if reader.Peek() <> -1 then
            match Convert.ToChar(reader.Peek()) with
            | '#' -> List.rev (s :: acc)
            | _ -> internalLoop (reader.ReadLine()) (s :: acc)
        else
            List.rev (s :: acc)

    if reader.Peek() <> -1 then
        splitTrip ((internalLoop (reader.ReadLine()) []) :: acc) reader
    else
        acc

let parse_PAS (s:string) =
    printfn "Parsing trips."
    match splitTrip [] (new StreamReader(s)) with
    | h :: t    -> t
                   |> List.map(fun x -> List.fold (fun a s -> castLine s a) trip.Zero x)
                   |> List.filter (fun x -> x.transport_type = "M")
                   |> List.map (fun x -> {x with departure = x.stops.Head.departure })
    | _         -> []

#endregion