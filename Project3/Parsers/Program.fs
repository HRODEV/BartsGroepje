open PAS_parser
open HLT_parser
open VTN_parser
open SQL

#if INTERACTIVE
let PAS_file = __SOURCE_DIRECTORY__ + "\RET.HLT"
let VTN_file = __SOURCE_DIRECTORY__ + "\RET.PAS"
let HLT_file = __SOURCE_DIRECTORY__ + "\RET.VTN"
#else
let PAS_file = System.Environment.CurrentDirectory + "/RET.PAS"
let VTN_file = System.Environment.CurrentDirectory + "/RET.VTN"
let HLT_file = System.Environment.CurrentDirectory + "/RET.HLT"
#endif

let stops = HLT_parser.parse_HLTFile HLT_file
let trips = PAS_parser.parse_PAS PAS_file
let metroStops = HLT_parser.FilterMetroStops stops trips
let metroStations = HLT_parser.ConvertStopsToStations metroStops

printfn "Creating platforms"
SQL.CreatePlatformQuery metroStations
printfn "Creating trips"
SQL.CreateTripQuery trips