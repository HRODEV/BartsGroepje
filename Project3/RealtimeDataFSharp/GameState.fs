module GameState
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data
open Coroutines
open Utilities
open System
open Entities

let rec metroreadyfilter (x: rideData.Value list) (d: DateTime) (acc: rideData.Value list)=
    match x with
    | h :: t when h.Date <= d -> metroreadyfilter t d (h :: acc)
    | h :: t -> List.rev acc
    | _ -> List.rev acc

let rec metroremainfilter (x: rideData.Value list) (d: DateTime) =
    match x with
    | h :: t when h.Date <= d -> metroremainfilter t d
    | h :: t -> x
    | _ -> x

type GameState = {
    Metros      : Metro list
    Stations    : Station list
    Textures    : Map<string, Texture2D>
    Fonts       : Map<string, Font>
    Rides       : rideData.Value list
    Time        : DateTime
    GameSpeed   : GameSpeed
    CounterBox  : CounterBox
    Behaviour   : Coroutine<unit, GameState>
    Count       : int
} with
    static member Draw(gameState: GameState, spriteBatch: SpriteBatch) =
        let backgroundImage = gameState.Textures.["background"]
        spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), Color.White)
        gameState.Stations |> List.iter(fun s -> s.Draw(gameState.Textures.["station"], spriteBatch))
        gameState.Metros |> List.iter(fun m -> m.Draw(gameState.Textures.["metro"], spriteBatch))
        CounterBox.Draw(gameState.CounterBox, gameState.Fonts.["font1"], gameState.Textures.["metro"], spriteBatch)
        GameSpeed.Draw(gameState.GameSpeed, gameState.Textures.["metro"], spriteBatch)

    static member Create(scaler: Vector2 -> Vector2, behaviour: Coroutine<unit, GameState>) =
        let stationList =  (stationData.Load("http://145.24.222.212/v2/odata/Stations").Value |> Array.map (fun st -> Station.Create(st, scaler))) |> List.ofArray
        let rides = (rideData.Load("http://145.24.222.212/v2/odata/Rides/?$expand=RideStops/Platform&$top=20&$orderby=Date").Value) |> List.ofArray

        { GameState.Zero() with
            Stations = stationList
            Rides = rides
            Time = rides.Head.Date
            Behaviour = behaviour
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
            Behaviour = fun s -> Done((), s)
            Count = 0
        }

    static member Update(gameState: GameState, dt: GameTime) =
        let newCounterBox = CounterBox.Update(gameState.CounterBox, gameState.Time)
        let newMetros = gameState.Rides |> fun r -> metroreadyfilter r gameState.Time [] |> List.map (fun r -> Metro.Create(A, r.RideStops, MetroProgram2()))
        let remainingRides = gameState.Rides |> fun r -> metroremainfilter r gameState.Time
        let newGameSpeed = GameSpeed.Update(gameState.GameSpeed)
        let updatedTime = gameState.Time + (new TimeSpan(0,0,0,0,dt.ElapsedGameTime.Milliseconds * newGameSpeed.GetSpeed))
        let UpdatedMetros = newMetros @ gameState.Metros |> List.filter (fun x -> x.Status <> Arrived) |> List.map (fun x -> Metro.Update updatedTime x)
        let behaviour', gameState' = singlestep gameState.Behaviour {gameState with Rides = remainingRides}
        printfn "%A" gameState'.Rides.Length

        { gameState' with Metros = UpdatedMetros; Time = updatedTime; CounterBox = newCounterBox; GameSpeed = newGameSpeed; Behaviour = behaviour' }

// Metros
let rec GetNextReadyRide : Coroutine<rideData.Value, GameState> =
    fun (s: GameState) ->
        match s.Rides with
        | h :: t when h.Date < s.Time -> Done(h, {s with Rides = t})
        | h :: t -> Wait(GetNextReadyRide, s)
        | _ -> Wait(GetNextReadyRide, s)

let StartReadyMetro (x: rideData.Value) : Coroutine<unit, GameState> =
    fun (s: GameState) ->
        Done((), {s with Metros = Metro.Create(A, x.RideStops, MetroProgram2()) :: s.Metros})

let UpdateTime (dt: GameTime): Coroutine<unit, GameState> =
    fun (s: GameState) ->
        let newGameSpeed = GameSpeed.Update(s.GameSpeed)
        let elapsedTime = new TimeSpan(0, 0, 0, 0, dt.ElapsedGameTime.Milliseconds * newGameSpeed.GetSpeed)
        Done((), {s with Time = s.Time + elapsedTime})

let UpdateMetros : Coroutine<unit, GameState> =
    fun (s: GameState) ->
        let metros = s.Metros |> List.filter (fun x -> x.Status <> Arrived) |> List.map (fun x -> Metro.Update s.Time x)
        Done((), {s with Metros = metros})

// Async Fetching
let ASyncDataRequest url =
    fun s ->
        let task = Async.StartAsTask (async {
            let! start = rideData.AsyncLoad(url)
            return start.Value
        })
        Done(task, s)

let rec ASyncDataParse (task:Threading.Tasks.Task<rideData.Value[]>) =
    fun s ->
        match task.IsCompleted with
        | true  -> Done(task.Result, s)
        | false -> Wait(ASyncDataParse task, s)

let rec LoadRides (rides: rideData.Value[]) =
    fun (s: GameState) ->
        let ridelist = rides |> List.ofArray
        Done((), {s with Rides = List.append s.Rides ridelist; Count = s.Count + 20})

let rec FetchRides url =
    co {
        let! (state: GameState) = getState
        if state.Rides.Length > 400 then
            do! yield_
            return! (FetchRides url)
        else
            let! task = ASyncDataRequest url
            let! rides = ASyncDataParse task
            do! LoadRides rides
            return ()
    }

let Fetcher() =
    co {
        let! state = getState
        let str = sprintf "http://145.24.222.212/v2/odata/Rides/?$expand=RideStops/Platform&$top=100&$orderby=Date&$skip=%i" state.Count
        do! FetchRides str
    } |> repeat_