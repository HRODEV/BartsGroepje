
open System.IO

let lines = File.ReadAllLines(__SOURCE_DIRECTORY__ + "/Stations.csv")

let idAndLines = lines |> Array.map (fun line -> line.Split(',').[0], line.Split(',').[2..])

let createInsert stationsid lineId =
    sprintf "INSERT INTO [retdbim].[dbo].[StationLines]([Station_Id],[Line_Id]) VALUES (%s,%i);
    " stationsid lineId

let lineToId line =
    match line with
    | "a" -> 1
    | "b" -> 2
    | "c" -> 3
    | "d" -> 4
    | "e" -> 5
    | _ -> failwith "unkow line"

let sql = 
    idAndLines 
    |> Array.map (fun (sid, lines) -> lines |> Array.map(fun line -> createInsert sid (line |> lineToId)) |> List.ofArray)
    |> Array.reduce (@)
    |> List.reduce (+)

printfn "%A" sql
