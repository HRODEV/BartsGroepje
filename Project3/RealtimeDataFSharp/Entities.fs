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


type Font = {
    Image : Texture2D
    Data  : FontFile
}


type GameSpeed = {
    Speed : int
} with
    member x.GetSpeed = x.Speed
    static member Zero =
        {  
            Speed = 1
        }
    static member Draw(lastGameSpeed : GameSpeed, texture: Texture2D, spriteBatch: SpriteBatch) =
        match lastGameSpeed.GetSpeed with
        | 0 -> spriteBatch.Draw(texture, new Rectangle(1500, 800, 25, 25), Color.Yellow)
        | 1 -> spriteBatch.Draw(texture, new Rectangle(1500, 800, 25, 25), Color.Yellow)
        | 50 -> for i in 0 .. 1 do spriteBatch.Draw(texture, new Rectangle(1500 + i * 65, 800, 25, 25), Color.Yellow)
        | 250 -> for i in 0 .. 2 do spriteBatch.Draw(texture, new Rectangle(1500 + i * 65, 800, 25, 25), Color.Yellow)
        | 1000 -> for i in 0 .. 3 do spriteBatch.Draw(texture, new Rectangle(1500 + i * 65, 800, 25, 25), Color.Yellow)
        | _ -> ()

        

    static member Update(lastGameSpeed : GameSpeed) =
        let currentKeyboard = Keyboard.GetState()
        {
            Speed = if currentKeyboard.IsKeyDown(Keys.D1) then 1 
                    else if currentKeyboard.IsKeyDown(Keys.D2) then 50 
                    else if currentKeyboard.IsKeyDown(Keys.D3) then 250 
                    else if currentKeyboard.IsKeyDown(Keys.D4) then 1000 
                    else if currentKeyboard.IsKeyDown(Keys.P) then 0 
                    else lastGameSpeed.GetSpeed
        }

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
    | Waiting of DateTime
    | Moving of TimeSpan
    | Arrived

type CounterBox = {
    Position : Vector2
    Time : DateTime
} with
    static member Create(position: Vector2, time : DateTime) =
        {
            Position = position
            Time = time
        }
    static member Draw(box: CounterBox, font: Font, texture: Texture2D, spriteBatch: SpriteBatch) = 
        spriteBatch.Draw(texture, new Rectangle((int)box.Position.X, (int)box.Position.Y, 385, 125), Color.Black)
        let fr = new FontRenderer(font.Data, font.Image)
        fr.DrawText(spriteBatch, (int)box.Position.X + 15, (int)box.Position.Y + 15, box.Time.DayOfWeek.ToString())
        fr.DrawText(spriteBatch, (int)box.Position.X + 15, (int)box.Position.Y + 65, box.Time.ToString())
        
    static member Update(box: CounterBox, newTime : DateTime) = 
        {
            Position = box.Position
            Time = newTime
        }


type Metro = {
    Line : Line
    RideStops : RideStop list
    Position : Vector2
    Status : TrainStatus
    Behaviour : DateTime -> Coroutine<Unit, Metro> 
} with
    static member Update (dt : DateTime) (x: Metro) =
        let _, metro' = costep (x.Behaviour dt) x
        metro'

    member this.Draw(texture: Texture2D, spriteBatch: SpriteBatch) =
            spriteBatch.Draw(texture, new Rectangle((int)this.Position.X - 2, (int)this.Position.Y - 2, 6, 6), Color.Red)

    static member Create (line: Line, rideStops: RideStop list, behaviour: DateTime -> Coroutine<Unit, Metro>) =
        {
            Line = line;
            RideStops = rideStops;
            Position = rideStops.Head.Position;
            Status = Waiting (rideStops.Head.Departure)
            Behaviour = behaviour;
        }

let MetroProgram2() (dt : DateTime) : Coroutine<Unit, Metro> =
    let DriveMetro (time : TimeSpan) (dt : DateTime) : Coroutine<bool, Metro> = fun metro ->
        match metro.RideStops with
        | current :: t when not t.IsEmpty ->      
                                let next = t.Head
                                let duration = next.Arrival - current.Arrival

                                let disX = (next.Position.X - current.Position.X)
                                let disY = (next.Position.Y - current.Position.Y)
                                let newPosX = if current.Position.X <> next.Position.X then easeInOutQuad2 ((float32)time.TotalSeconds) current.Position.X disX ((float32)duration.TotalSeconds) else next.Position.X
                                let newPosY = if current.Position.Y <> next.Position.Y then easeInOutQuad2 ((float32)time.TotalSeconds) current.Position.Y disY ((float32)duration.TotalSeconds) else next.Position.Y
                                Done(dt >= next.Arrival , {metro with Position = new Vector2(newPosX, newPosY); Status = Moving ((dt - current.Departure))})
        | _ -> Done(true, metro)

    let inline WaitMetro (departureTime : DateTime) (currentTime : DateTime) : Coroutine<TimeSpan, Metro> = fun metro ->   
        let newTime = departureTime - currentTime
        Done(newTime, metro)

    let inline SetMetroStatus (status : TrainStatus) : Coroutine<Unit, Metro> = fun metro -> 
        Done((), {metro with Status = status})

    let inline SetNextStation (metro : Metro) = 
        match metro.RideStops with
        | h :: t -> Done((), {metro with RideStops = t})
        | _ -> Done((), metro)

    co{
        let! metro = getState
        match metro.Status with
        | Waiting r ->  let! timeRemaining = WaitMetro r dt; 
                        if timeRemaining > TimeSpan.Zero then
                            do! SetMetroStatus (Waiting r)
                            do! yield_
                        else
                            do! SetMetroStatus (Moving (TimeSpan.Zero))
                            return ()
        | Moving t ->   let! arrived = DriveMetro t dt
                        if ((arrived = true) && (metro.RideStops.Length > 0)) then
                            do! SetMetroStatus (Waiting (metro.RideStops.Head.Departure))
                            do! SetNextStation
                            return ()
                        else if ((arrived = true) && (metro.RideStops.Length <= 0)) then
                            do! SetMetroStatus Arrived
                        else
                            do! yield_
        | Arrived ->    return ()
      }

type GameState = {
    Metros      : Metro list
    Stations    : Station list
    Textures    : Map<String, Texture2D>
    Fonts       : Map<String, Font>
    Rides       : rideData.Value list
    Time        : DateTime
    GameSpeed   : GameSpeed
    CounterBox  : CounterBox
} with
    static member Draw(gameState: GameState, spriteBatch: SpriteBatch) =
        let backgroundImage = gameState.Textures.["background"]
        spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), Color.White)


        gameState.Stations |> List.iter(fun s -> s.Draw(gameState.Textures.["station"], spriteBatch))
        gameState.Metros |> List.iter(fun m -> m.Draw(gameState.Textures.["metro"], spriteBatch))


        CounterBox.Draw(gameState.CounterBox, gameState.Fonts.["font1"], gameState.Textures.["metro"], spriteBatch)
        GameSpeed.Draw(gameState.GameSpeed, gameState.Textures.["metro"], spriteBatch)

    static member Create(scaler: Vector2 -> Vector2) =
        let stationList =  (stationData.Load("http://145.24.222.212/v2/odata/Stations").Value |> Array.map (fun st -> Station.Create(st, scaler))) |> List.ofArray
        let rides = (rideData.Load("http://145.24.222.212/v2/odata/Rides/?$expand=RideStops/Platform&$top=100&$orderby=Date").Value) |> List.ofArray
        { GameState.Zero() with
            Stations = stationList
            Rides = rides
            Time = rides.Head.Date
        }

    static member Zero() =
        {
            Metros = []
            Stations = []
            Textures = Map.empty
            Fonts = Map.empty
            Rides = []
            Time = new DateTime()
            GameSpeed = GameSpeed.Zero
            CounterBox = CounterBox.Create(new Vector2(1500.f, 950.f), new DateTime())
        }

    static member Update(gameState: GameState, dt: GameTime) =
        let newCounterBox = CounterBox.Update(gameState.CounterBox, gameState.Time)
        let newMetros = gameState.Rides |> List.filter (fun m -> m.Date <= gameState.Time) |> List.map (fun r -> Metro.Create(A, r.RideStops |> Array.map(fun x -> RideStop.Create(x, scaler, 0)) |> List.ofArray, MetroProgram2()))
        let remainingRides = gameState.Rides |> List.filter (fun m -> m.Date > gameState.Time)

        let newGameSpeed = GameSpeed.Update(gameState.GameSpeed)

        let updatedTime = gameState.Time + (new TimeSpan(0,0,0,0,dt.ElapsedGameTime.Milliseconds * newGameSpeed.GetSpeed))
        let UpdatedMetros = newMetros @ gameState.Metros |> List.filter (fun x -> x.Status <> Arrived) |> List.map (fun x -> Metro.Update updatedTime x)
        { gameState with Metros = UpdatedMetros; Time = updatedTime; Rides = remainingRides; CounterBox = newCounterBox; GameSpeed = newGameSpeed }