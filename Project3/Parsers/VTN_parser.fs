module VTN_parser
let VTN_file = System.Environment.CurrentDirectory + "/RET.VTN"

let private parseline (date: System.DateTime) (line: string) =
    let rec dates (days: string) (date: System.DateTime) =
        if days.Length > 0 then
            match days.[0] with
            | '0' -> dates (days.Substring 1) (date.AddDays 1.0)
            | '1' -> date :: (dates (days.Substring 1) (date.AddDays 1.0))
            | _ -> failwith "Incorrect VTN format"
        else
            []
    (int (line.Substring (1,5) ), dates (line.Substring (7,98)) date)

let GetVTNRecords (date: System.DateTime) = printfn "Parsing VTN"; System.IO.File.ReadAllLines(VTNFile) |> Array.map (parseline date) |> Map.ofArray