module Game

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data

//open Stations

let easeInOutQuad2 (currentTime : float32) (startPos : float32) (endPos : float32) (duration : float32) =   let newTime = currentTime / (duration / 2.0f)
                                                                                                            if ( newTime < 1.0f ) then
                                                                                                                startPos + newTime * newTime * endPos / 2.0f
                                                                                                                //let b = startPos + (endPos / 2.0f * newTime * newTime)
                                                                                                                //b
                                                                                                            else 
                                                                                                                let newTime' = newTime - 1.0f
                                                                                                                //startPos + (newTime' * (newTime' - 2.0f) - 1.0f) * endPos / 2.0f
                                                                                                                startPos + (-endPos / 2.0f * (newTime' * (newTime' - 2.0f) - 1.0f))
                                                                                                                //-endPos / 2.0f * ( newTime' * ( newTime' - 2.0f) - 1.0f ) + startPos
//
//  t /= d/2;
//	if (t < 1) return c/2*t*t + b;
//	t--;
//	return -c/2 * (t*(t-2) - 1) + b;

//QuadEaseInOut( double t, double b, double c, double d )
//        {
//            if ( ( t /= d / 2 ) < 1 )
//                return c / 2 * t * t + b;
//
//            return -c / 2 * ( ( --t ) * ( t - 2 ) - 1 ) + b;

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



type Coroutine<'a, 's> = 's -> CoroutineStep<'a, 's>
    and CoroutineStep<'a, 's> = 
    |   Done of 'a * 's
    |   Wait of Coroutine<'a, 's> * 's

let rec (>>) p k =
    fun s ->
        match p s with
        | Done(a, s') -> k a s'
        | Wait(leftOver, s') -> Wait((leftOver >> k), s')


type CoroutineBuilder() = 
    member this.Return(x) = (fun s -> Done(x, s))
    member this.Bind(p, k) = p >> k

let co = CoroutineBuilder()

let getState = fun s -> Done(s, s)
(*
type metro
    stations : station list
    currentstation : station
    position : vector2
    Behaviors : Coroutine list
    factor : float

metro -> coroutine 
    {
        coroutine that uses time to measure speed
    }

    { 
        while true
            for s in stations 
                .||
                    { wait position above s 
                      wait for online notification }
                    { online notification its arrived }
                .||
                    { wait 10 second }
                    { online notification its left the station }
    
    }

*)


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
            //let! newMetro = fun (s : Metro) -> Done((), {s with Position = new Vector2(s.Position.X + 1.0f, 0.0f)})
            //return newMetro
        }


(*
let! metro = getstate
do! metro.update
*)

let yield_ = fun s -> Wait((fun s -> Done((), s)), s)

let MetroProgram2() (dt : GameTime) : Coroutine<Unit, Metro> = 
    let DriveMetro (time : float32) (dt : GameTime) : Coroutine<bool, Metro> = fun metro -> 
        match metro.Station.Next with
        | Some nextStation ->   let duration = nextStation.Arrival - metro.Station.Arrival
                                let newTime = (time + ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f))
                                let disX = (nextStation.Position.X - metro.Station.Position.X)
                                let disY = (nextStation.Position.Y - metro.Station.Position.Y)
                                let newPosX = if metro.Station.Position.X <> nextStation.Position.X then easeInOutQuad2 time metro.Station.Position.X disX duration else nextStation.Position.X
                                let newPosY = if metro.Station.Position.Y <> nextStation.Position.Y then easeInOutQuad2 time metro.Station.Position.Y disY duration else nextStation.Position.Y
                                Done(newTime >= duration , {metro with Position = new Vector2(newPosX, newPosY); Status = Moving newTime})
        | None -> Done(true, metro)

    let inline WaitMetro (r : float32) (g : GameTime) : Coroutine<float32, Metro> = fun metro ->   
        let newTime = (r - ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f))
        Done(newTime, metro)

    let inline SetMetroStatus (status : TrainStatus) : Coroutine<Unit, Metro> = fun metro -> 
        Done((), {metro with Status = status})

    let inline SetNextStation metro = 
        match metro.Station.Next with
        | Some nextStation -> Done((), {metro with Station = nextStation})
        | None -> Done((), metro)

    co{
        let! metro = getState
        match metro.Status with
        | Waiting r ->  let! timeRemaining = WaitMetro r dt; 
                        if timeRemaining > 0.0f then
                            do! SetMetroStatus (Waiting timeRemaining)
                            do! yield_
                        else
                            do! SetMetroStatus (Moving (0.0f))
                            return ()
        | Moving t ->   let! arrived = DriveMetro t dt
                        if (arrived = true) then
                            do! SetNextStation
                            do! SetMetroStatus (Waiting (2.0f))
                            return ()
                        else
                            do! yield_
        | Arrived ->    return ()
      }

type stationData = JsonProvider<"http://145.24.222.212/ret/odata/Stations/?$expand=PlatForms">

type StationDrawable = {
    X:float
    Y:float
}

type GameState = {
    Metro : Metro
    StationList : Station list
}

//let connectStation (s1 : Station) (s2 : Station) =
//    {s2 with Next = Some(s1)}
//
//let (<<>>) = connectStation

// r |> List.map (fun st -> st)

let oostplein =  {Name = "Oostplein"; Next = None; Arrival = 25.0f; Position = new Vector2(50.0f, 300.0f)}
let blaak = {Name = "Blaak"; Next = None; Arrival = 20.0f; Position = new Vector2(350.0f, 200.0f)}
let beurs = {Name = "Beurs"; Next = None; Arrival = 12.0f; Position = new Vector2(350.0f, 0.0f)}
let eendrachtsplein = {Name = "Eendrachtsplein"; Next = None; Arrival = 9.0f; Position = new Vector2(250.0f, 0.0f)}
let dijkzigt = {Name = "Dijkzigt"; Next = None; Arrival = 0.0f; Position = new Vector2(300.0f, 300.0f) }

//let track = station <<>> station0 <<>> station1 <<>> station2 <<>> station3

let combinedTrack x xs = 
    xs 
    |> List.rev
    |> List.fold (fun next start -> {start with Next = Some(next)}) x 
    

//let easeInOutQuad (time : float32) = if time < 0.5f then 2.0f * time * time else -1.0f + ( 4.0f - 2.0f * time) * time 

//let stops = Stations.GetAllStationStopRecords ()
//let ritten = Ritten.ritten ()

//let metroStops = Stations.FilterMetroStops stops ritten
//let metroStations = Stations.ConnvertStopsToStations metroStops

printfn "%A" combinedTrack

let rec costep coroutine state =
  match coroutine state with
  | Done(_, newState) ->  (fun s -> Done((), s)), newState
  | Wait(c', s') -> costep c' s'

type TrainSimulation() as this =
    inherit Game()
 
    do this.Content.RootDirectory <- "Content"

    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>

    let mutable GameState = {   
        Metro = Unchecked.defaultof<Metro>
        StationList = []
    }

    override x.Initialize() =
        graphics.PreferredBackBufferWidth <- 1200;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight <- 900;   // set this value to the desired height of your window
        graphics.ApplyChanges();

        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        //let parkImage = spriteLoader "parking.png" this.GraphicsDevice

        //movableThingsImages <- [carImage; boatImage];

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        let mutable count = 2.0f

        let stationList = 
            (stationData.GetSample().Value
            |> Array.toList
            |> List.map (fun st ->
                count <- count + 2.0f
                printfn "Name: %A X: %A Y: %A" st.Name ((((float32)st.X - 80000.0f) / 20.0f) - 200.0f) (((((float32)st.Y - 420300.0f) / 20.0f) - 300.0f) * -1.0f)
                {Name = st.Name; Next = None; Arrival = count; Position = new Vector2((((float32)st.X - 80000.0f) / 20.0f) - 100.0f, (((((float32)st.Y - 430000.0f) / 20.0f) - 300.0f) * -1.0f) + 400.0f)}
            ))


        //let newTrain = {Line = A; Station = combinedTrack; Position = combinedTrack.Position; Status = TrainStatus.Waiting 0.0f}
        let newMetro = {Line = A; Station = (combinedTrack stationList.Head stationList.Tail); Position = stationList.Head.Position; Status = TrainStatus.Waiting 0.0f; Behaviour = MetroProgram2()}

        GameState <- {GameState with Metro = newMetro; StationList = stationList}
        
        //let a = {ID = 0; Components = [Position(10.0f, 10.0f)];}

        (*GameState <-{ GameState with 
                        Entities = [a; bouncyBall]
                        Systems = [OtherSystem; LogicSystem]
                    }
                    *)
        //State <- MainUpdate State
        ()
 
    override this.Update (gameTime) =

        (* let rec worldLoop (systems : ISystem list) world = // Find out a better solution to this. =D
            match systems with
            | [] -> world
            | x::xs -> worldLoop xs (x.Update world); *)

(*
        let newWorld = GameState.Systems |> List.fold (fun s system -> system.Update s) GameState

        GameState <- newWorld

        //let newWorld = GameState.Systems |> List.map(fun s -> s.Update s GameState) |> Seq.head

        //GameState <- newWorld
        
        *)

        let leftCo, newTrain = costep (Metro.Update gameTime) GameState.Metro

        GameState <- {GameState with Metro = newTrain}
        ()
        
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.CornflowerBlue

        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied)
        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        GameState.StationList 
        |> List.iter(
            fun s -> 
                spriteBatch.Draw(
                    texture, new Rectangle(
                        (int)s.Position.X, (int)s.Position.Y, 20, 20
                    ),
                    match s.Name with
(*                  | "Blaak" -> Color.Aqua
                    | "Pernis" -> Color.Aquamarine
                    | "Rotterdam Centraal" -> Color.OrangeRed
                    | "Pijnacker Centrum" -> Color.Beige
                    | "Nesselande" -> Color.BlanchedAlmond
 *)                 | _ -> Color.Green
                )
        );

        spriteBatch.Draw(
            texture, new Rectangle(
                (int)GameState.Metro.Position.X, (int)GameState.Metro.Position.Y, 15, 15
            ), 
            Color.Red
        ); 

        spriteBatch.End()