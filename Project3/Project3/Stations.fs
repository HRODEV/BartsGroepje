module Stations

open System.IO


#if INTERACTIVE
let locationHLTFile = __SOURCE_DIRECTORY__ + "\RET.HLT"
#else
let locationHLTFile = System.Environment.CurrentDirectory + "\RET.HLT"
#endif

type Station = 
    {
        OverStap: bool
        Code: string
        RDX: int
        RDY: int
        Name: string
    }

let ParseLine (line:string) =
    {
        OverStap = line.[0] = '1'
        Code = line.Substring(2, 6).Trim()
        RDX = line.Substring(29,6) |> int
        RDY = line.Substring(36,6) |> int
        Name = line.Substring(43,49).Trim()
    }


let GetAllRecordsFromFile file =
    File.ReadAllLines(file)
    |> Array.map ParseLine
    |> Array.toList

let GetAllRecords () = GetAllRecordsFromFile locationHLTFile
