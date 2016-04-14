module HalteFile_parser

open FSharp.Data

type halteCSV = CsvProvider<"haltebestand.csv", ";", IgnoreErrors=true>

let file = new halteCSV()
let rows = file.Rows

let MetroRows = 
    rows |> Seq.filter (fun row -> row.Desc.StartsWith("Metrolijn "))

let metroNamesWithLines =
    MetroRows 
    |> Seq.distinctBy (fun row -> row.Desc + row.Name.Replace(" Ca", "")) 
    |> Seq.map 
        (fun row -> 
            row.Name.Replace(" Ca", "").Replace("Schiedam", "Schiedam Centrum"), 
            if row.Desc.Split(' ').[1].EndsWith(",") then
                (row.Desc.Split(' ').[1].TrimEnd(',').Split('/') |> List.ofArray) @ (row.Desc.Split(' ').[3].TrimEnd(',').Split('/') |> List.ofArray ) |> Array.ofList
            else
                row.Desc.Split(' ').[1].TrimEnd(',').Split('/')
        ) 
    |> Seq.groupBy fst
    |> Seq.map (fun (name, s) -> name, (s |> Seq.map (fun (name, lines) -> lines |> List.ofArray) |> Seq.fold (@) [] |> Set.ofList |> Set.toArray))
    

metroNamesWithLines |> Seq.iter (printfn "%A")

(*
    INSERT INTO [retdb].[dbo].[Lines]
           ([Id],[Name])
     VALUES
           (1, "A"),
           (2, "B"),
           (3, "C"),
           (4, "D"),
           (5, "E");

    INSERT INTO [dbo].[StationLines]
           ([Station_Id]
           ,[Line_Id])
     VALUES
           ((SELECT TOP 1 [Id] FROM [retdb].[dbo].[Stations] WHERE [Name] = 'Zuidplein'),
           1)
*)
let EscapeApostraphe    (s: string) = s.Replace("'", "''")

let getLineId (rawLine: string) =
    let lines = [1..5] |> List.map2 (fun line id -> id,line.ToString()) ['A'..'E']
    lines |> List.find (fun (_, line) -> line = rawLine) |> fst

let insertValuesString = 
    metroNamesWithLines 
    |> Seq.map (
        fun (name, lines)->
            lines |> Array.map 
                (fun line ->
                    sprintf "((SELECT TOP 1 [Id] FROM [retdb].[dbo].[Stations] WHERE [Name] = '%s'), 
                    %i)" (EscapeApostraphe name) (getLineId line)
                ) |> String.concat ","
        ) |> String.concat ","

let insertSQL = 
    sprintf "INSERT INTO [dbo].[StationLines]
           ([Station_Id]
           ,[Line_Id])
     VALUES
           %s;"  insertValuesString

System.Console.Write(insertSQL)