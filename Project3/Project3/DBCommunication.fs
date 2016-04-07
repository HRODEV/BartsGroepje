module DBCommunication
open System;
open System.Data;
open System.Data.Linq
open System.Data.Sql
open FSharp.Data
open FSharp.Data.Sql

let SendQuery (connectionStr : string, query : string) = 
    let connection = new MySql.Data.MySqlClient.MySqlConnection(connectionStr)
    do connection.Open()
    let command = new MySql.Data.MySqlClient.MySqlCommand(query, connection)
    command.ExecuteNonQuery() |> ignore
    printfn "Command succesfully sent!"
    connection.Close()

let [<Literal>] private resolutionPath = __SOURCE_DIRECTORY__ + "packages/MySql.Data.6.9.8/lib/net45/MySql.Data.dll"
let [<Literal>] private connectionString = "server=localhost;user=root;database=test;password=root;"

type private sql = SqlDataProvider< 
      ConnectionString = connectionString,
      DatabaseVendor = Common.DatabaseProviderTypes.MYSQL,
      ResolutionPath = resolutionPath,
      IndividualsAmount = 1000,
      UseOptionTypes = true >

let RetrieveContext =
    sql.GetDataContext()