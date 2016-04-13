module Entities
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data
open Coroutines
open Utilities
open System

type Line = A | B | C | D | E
type stationData = JsonProvider<"Samples/StationsAndPlatformsSample.json">
type rideData = JsonProvider<"Samples/RidesAndRideStopsAndPlatformAndStation.json">

type RideStop = {
    Name : string
    Arrival : DateTime
    Departure: DateTime
    Position : Vector2
} with
    static member Create(rideStop: rideData.RideStop, scaler: Vector2 -> Vector2, wait: int) =
        {
            Name = rideStop.Platform.Code
            Arrival = rideStop.Time
            Departure = rideStop.Time + (new TimeSpan(0,0,wait))
            Position = scaler (new Vector2((float32)rideStop.Platform.X, (float32)rideStop.Platform.Y))
        }

type Station = {
    Name : string
    Position : Vector2
} with
  member this.Draw(texture: Texture2D, spriteBatch: SpriteBatch) =
        spriteBatch.Draw(texture, new Rectangle((int)this.Position.X - 10, (int)this.Position.Y - 10, 20, 20), Color.White);
  
  static member Create(station: stationData.Value, scaler: Vector2 -> Vector2) =
      {
          Name = station.Name
          Position = scaler (new Vector2((float32)station.X, (float32)station.Y))
      }

type TrainStatus = 
    | Waiting of float32
    | Moving of float32
    | Arrived

type Metro = {
    Line : Line
    RideStops : RideStop list
    Position : Vector2
    Status : TrainStatus
    Behaviour : GameTime -> Coroutine<Unit, Metro> 
} with
    static member Update (dt : GameTime) (x: Metro) =
        let _, metro' = costep (x.Behaviour dt) x
        metro'

    member this.Draw(texture: Texture2D, spriteBatch: SpriteBatch) =
            spriteBatch.Draw(texture, new Rectangle((int)this.Position.X - 2, (int)this.Position.Y - 2, 6, 6), Color.Red)
    static member Create (line: Line, rideStops: RideStop list, behaviour: GameTime -> Coroutine<Unit, Metro>) =
        {
            Line = line;
            RideStops = rideStops;
            Position = rideStops.Head.Position;
            Status = Waiting 0.0f;
            Behaviour = behaviour;
        }


type GameState = {
    Metros : Metro list
    Stations : Station list
    Textures : Map<String, Texture2D>
    Rides: rideData.Value list
    Time    :   DateTime
} with
    static member Draw(gameState: GameState, spriteBatch: SpriteBatch) =
        let backgroundImage = gameState.Textures.["background"]
        spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), Color.White)

        gameState.Metros |> List.iter(fun m -> m.Draw(gameState.Textures.["metro"], spriteBatch))
        gameState.Stations |> List.iter(fun s -> s.Draw(gameState.Textures.["station"], spriteBatch))

    static member Create(scaler: Vector2 -> Vector2) =
        let stationList =  (stationData.Load("http://145.24.222.212/ret/odata/Stations").Value |> Array.map (fun st -> Station.Create(st, scaler))) |> List.ofArray
        let rides = (rideData.Load("http://145.24.222.212/ret/odata/Rides/?$expand=RideStops/Platform&$top=20&$orderby=Date").Value) |> List.ofArray
        { GameState.Zero() with
            Stations = stationList
            Rides = rides
        }

    static member Zero() =
        {
            Metros = []
            Stations = []
            Textures = Map.empty
            Rides = []
            Time = new DateTime()
        }

    static member Update(gameState: GameState, dt: GameTime) =
        let metros = gameState.Metros |> List.map (fun x -> Metro.Update dt x)
        { gameState with Metros = metros }