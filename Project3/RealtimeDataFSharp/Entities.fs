module Entities
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data
open Coroutines
open Utilities

type Line = A | B | C | D | E

type Station = {
    Name : string
    Arrival : float32
    Next : Option<Station>
    Position : Vector2
}

type TrainStatus = 
    | Waiting of float32
    | Moving of float32
    | Arrived

type Train = {
    Line : Line
    Station : Station
    Position : Vector2
    Status : TrainStatus
} with member x.Update (dt : GameTime) =
        match x.Status with
        | Moving time ->    match x.Station.Next with
                            | Some nextStation ->   let duration = nextStation.Arrival - x.Station.Arrival
                                                    if (time < duration) then
                                                        let newTime = (time + ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f))
                                                        let disX = (nextStation.Position.X - x.Station.Position.X)
                                                        let disY = (nextStation.Position.Y - x.Station.Position.Y)
                                                        let newPosX = if x.Station.Position.X <> nextStation.Position.X then easeInOutQuad2 time x.Station.Position.X disX duration else nextStation.Position.X
                                                        let newPosY = if x.Station.Position.Y <> nextStation.Position.Y then easeInOutQuad2 time x.Station.Position.Y disY duration else nextStation.Position.Y
                                                        {x with Position = new Vector2(newPosX, newPosY); Status = Moving newTime}
                                                    else
                                                        {x with Station = nextStation; Status = Waiting 4.0f }
                            | None -> x
        | Waiting wait ->   if (wait < 0.0f) then
                                match x.Station.Next with
                                | Some nextStation ->   {x with Status = Moving (0.0f)}

                                | None -> {x with Status = Arrived}
                            else
                                {x with Status = Waiting (wait - ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f)) }
        | Arrived -> x

type Metro = {
    Line : Line
    Station : Station
    Position : Vector2
    Status : TrainStatus
    Behaviour : GameTime -> Coroutine<Unit, Metro> 
} with static member Update (dt : GameTime) =
        co{ 
            let! metro = getState
            do! metro.Behaviour dt
        }

type stationData = JsonProvider<"http://145.24.222.212/ret/odata/Stations/?$expand=PlatForms">

type StationDrawable = {
    X:float
    Y:float
}

type GameState = {
    Metro : Metro
    StationList : Station list
    Map : Texture2D
}