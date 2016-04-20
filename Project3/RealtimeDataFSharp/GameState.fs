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
open HeatMap2

#REGION
//GameState Type
type GameState = {
    Metros      : Metro list
    Stations    : Station list
    Textures    : Map<string, Texture2D>
    Fonts       : Map<string, Font>
    Rides       : rideData.Value list
    Time        : DateTime
    GameSpeed   : GameSpeed
    CounterBox  : CounterBox
    HeatMap     : HeatMap
    Behaviour   : Coroutine<unit, GameState> list
    Count       : int
    dt          : GameTime
    infobox     : InfoBox
} with
    static member Draw(gameState: GameState, spriteBatch: SpriteBatch) =
        let backgroundImage = gameState.Textures.["background"]
        let fr = new FontRenderer(gameState.Fonts.["font1"].Data, gameState.Fonts.["font1"].Image)
        spriteBatch.Draw(backgroundImage, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), Color.White)
        gameState.Metros |> List.iter(fun m -> m.Draw(gameState.Textures.["metro"], spriteBatch))
        gameState.Stations |> List.iter(fun s -> s.Draw(gameState.Textures.["station"], spriteBatch))
        CounterBox.Draw(gameState.CounterBox, gameState.Fonts.["Timer"], gameState.Textures.["plain"], spriteBatch)
        GameSpeed.Draw(gameState.GameSpeed, gameState.Fonts.["Timer"], spriteBatch)
        if gameState.HeatMap.Active then HeatMap.Draw(gameState.HeatMap, gameState.Textures.["plain"], spriteBatch) else ()
        gameState.infobox.Draw spriteBatch (gameState.Metros.Length) (gameState.Metros |> List.fold (fun a m -> Metro.CalculateDistance m gameState.Metros + a) 0 |> fun x -> if gameState.Metros.Length <> 0 then x / gameState.Metros.Length else 0) gameState.Fonts

    static member Create(scaler: Vector2 -> Vector2, behaviour: Coroutine<unit, GameState> list, textures: Map<string, Texture2D>) =
        let stationList =  (stationData.Load("http://145.24.222.212/v2/odata/Stations").Value |> Array.map (fun st -> Station.Create(st, scaler))) |> List.ofArray
        let rides = (rideData.Load("http://145.24.222.212/v2/odata/Rides/?$expand=RideStops/Platform,Line&$top=20&$orderby=Date").Value) |> List.ofArray

        { GameState.Zero() with
            Stations = stationList
            Rides = rides
            Time = rides.Head.Date
            Behaviour = behaviour
            infobox = InfoBox.Create(textures)
            Textures = textures
        }

    static member Zero() =
        {
            Metros = []
            Stations = []
            Textures = Map.empty
            Fonts = Map.empty
            Rides = []
            Time = new DateTime()
            GameSpeed = GameSpeed.Create(new Vector2(1475.0f, 985.f))
            CounterBox = CounterBox.Create(new Vector2(1200.f, 900.f), new DateTime())
            Behaviour = []
            Count = 20
            dt = new GameTime()
            infobox = InfoBox.Zero()
            HeatMap = HeatMap.Create(32, 32)
        }

    static member Update(gameState: GameState, dt: GameTime) =
        gameState.Behaviour |>
        List.fold (
            fun (acc: GameState) x ->
                let behaviour', state' = (singlestep x acc)
                {state' with Behaviour = behaviour' :: state'.Behaviour}
        ) {gameState with Behaviour = []; dt = dt; HeatMap = HeatMap.Update(gameState.HeatMap, gameState.Metros); infobox = InfoBox.Update gameState.infobox gameState.GameSpeed (float32 gameState.Metros.Length)}
#ENDREGION

#REGION
//Metro creation coroutine
let private GetNextReadyRides =
    fun (s: GameState) ->
        let rec looper (x: rideData.Value list) acc =
            match x with
                | h :: t when h.Date <= s.Time -> looper t (h :: acc)
                | h :: t -> List.rev acc
                | _ -> List.rev acc
        Done(looper s.Rides [], s)

let private CreateMetrosFromRides (rides: rideData.Value list)=
    fun (s: GameState) ->
        let rec looper (rides: rideData.Value list) =
            match rides with
            | h :: t    ->
                let line = match h.Line.Name with
                    | "A" -> A
                    | "B" -> B
                    | "C" -> C
                    | "D" -> D
                    | "E" -> E
                    | _ -> A
                Metro.Create(line, h.RideStops, MetroProgram2()) :: (looper t)
            | _         -> []
        Done((), {s with Metros = (looper rides) @ s.Metros})

let private RemoveDepartedRides =
    fun (s: GameState) ->
        let rec looper (rides: rideData.Value list) =
            match rides with
                | h :: t when h.Date <= s.Time -> looper t
                | h :: t -> rides
                | _ -> rides
        Done((), {s with Rides = looper s.Rides})

let private UpdateTime =
    fun (s: GameState) ->
        let newCounterBox = CounterBox.Update(s.CounterBox, s.Time)
        let newGameSpeed = GameSpeed.Update(s.GameSpeed)
        let elapsedTime = new TimeSpan(0, 0, 0, 0, s.dt.ElapsedGameTime.Milliseconds * newGameSpeed.GetSpeed)
        Done((), {s with Time = s.Time + elapsedTime; GameSpeed = newGameSpeed; CounterBox = newCounterBox;})

let private UpdateMetros : Coroutine<unit, GameState> =
    fun (s: GameState) ->
        let metros = s.Metros |> List.map (fun x -> Metro.Update s.Time x) |> List.filter (fun x -> x.Status <> Arrived)
        Done((), {s with Metros = metros})

let MainStateLogic() =
    co {
        do! UpdateTime
        let! readyRides = GetNextReadyRides
        do! CreateMetrosFromRides readyRides
        do! RemoveDepartedRides
        do! UpdateMetros
        do! yield_
    } |> repeat_
#ENDREGION

#REGION
// Async ride fetching from JSON API coroutine.
let private ASyncDataRequest url =
    fun s ->
        let task = Async.StartAsTask (async {
            let! start = rideData.AsyncLoad(url)
            return start.Value
        })
        Done(task, s)

let rec private ASyncDataParse (task:Threading.Tasks.Task<rideData.Value[]>) =
    fun s ->
        match task.IsCompleted with
        | true  -> Done(task.Result, s)
        | false -> Wait(ASyncDataParse task, s)

let rec private LoadRides (rides: rideData.Value[]) =
    fun (s: GameState) ->
        let ridelist = rides |> List.ofArray
        Done((), {s with Rides = List.append s.Rides ridelist; Count = s.Count + 100})

let rec StateFetchRideLogic () =
    co {
        let! state = getState
        if state.Rides.Length > 400 then
            do! yield_
        else
            let str = sprintf "http://145.24.222.212/v2/odata/Rides/?$expand=RideStops/Platform,Line&$top=100&$orderby=Date&$skip=%i" state.Count
            let! task = ASyncDataRequest str
            let! rides = ASyncDataParse task
            if rides.Length <> 0 then
                do! LoadRides rides
            else
                do! yield_
                return ()
    } |> repeat_
#ENDREGION