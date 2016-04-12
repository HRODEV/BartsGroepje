// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System
open System.Globalization
open System.Reflection
open FSharp.Data
open FSharp.Data.JsonExtensions
open System.Collections.Generic

open Game

[<EntryPoint>]
let main argv = 
    use g = new TrainSimulation()
    g.Run()
    0 // return an integer exit code
