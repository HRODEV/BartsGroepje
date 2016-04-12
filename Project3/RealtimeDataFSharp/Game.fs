module Game
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data
open Coroutines
open Entities
open Utilities

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

let oostplein =  {Name = "Oostplein"; Next = None; Arrival = 25.0f; Position = new Vector2(50.0f, 300.0f)}
let blaak = {Name = "Blaak"; Next = None; Arrival = 20.0f; Position = new Vector2(350.0f, 200.0f)}
let beurs = {Name = "Beurs"; Next = None; Arrival = 12.0f; Position = new Vector2(350.0f, 0.0f)}
let eendrachtsplein = {Name = "Eendrachtsplein"; Next = None; Arrival = 9.0f; Position = new Vector2(250.0f, 0.0f)}
let dijkzigt = {Name = "Dijkzigt"; Next = None; Arrival = 0.0f; Position = new Vector2(300.0f, 300.0f) }

let combinedTrack x xs = 
    xs
    |> List.rev
    |> List.fold (fun next start -> {start with Next = Some(next)}) x 

printfn "%A" combinedTrack

let spriteLoader (path) graphics = 
    use imagePath = System.IO.File.OpenRead(path)
    let texture = Texture2D.FromStream(graphics, imagePath)
    let textureData = Array.create<Color> (texture.Width * texture.Height) Color.Transparent
    texture.GetData(textureData)
    texture

type TrainSimulation() as this =
    inherit Game()
    do this.Content.RootDirectory <- "Content"
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable texture = Unchecked.defaultof<Texture2D>

    let mutable GameState = {   
        Metro = Unchecked.defaultof<Metro>
        StationList = []
        Map = Unchecked.defaultof<Texture2D>
    }

    override x.Initialize() =
        graphics.PreferredBackBufferWidth <- 1920;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight <- 1080;   // set this value to the desired height of your window
        graphics.ApplyChanges();
        x.IsMouseVisible <- true;
        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        let BackgroundMap = spriteLoader "Rotterdam.png" this.GraphicsDevice

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        let mutable count = 2.0f

        let stationList = 
            (stationData.GetSample().Value
            |> Array.toList
            |> List.map (fun st ->
                count <- count + 2.0f
                printfn "Name: %A X: %A Y: %A" st.Name ((((float32)st.X - 80000.0f) / 20.0f) - 200.0f) (((((float32)st.Y - 420300.0f) / 20.0f) - 300.0f) * -1.0f)
                {Name = st.Name; Next = None; Arrival = count; Position = new Vector2((((float32)st.X - 80000.0f) / 20.0f) + 20.0f, (((((float32)st.Y - 430000.0f) / 20.0f) - 300.0f) * -1.0f) + 400.0f)}
            ))

        let newMetro = {Line = A; Station = (combinedTrack stationList.Head stationList.Tail); Position = stationList.Head.Position; Status = TrainStatus.Waiting 0.0f; Behaviour = MetroProgram2()}

        GameState <- {GameState with Metro = newMetro; StationList = stationList; Map = BackgroundMap}
        ()
 
    override this.Update (gameTime) =

        let leftCo, newTrain = costep (Metro.Update gameTime) GameState.Metro

        GameState <- {GameState with Metro = newTrain}
        ()
        
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.CornflowerBlue

        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        spriteBatch.Draw(GameState.Map, new Rectangle(0, 0, GameState.Map.Width, GameState.Map.Height), Color.White)

        GameState.StationList 
        |> List.iter(
            fun s -> 
                spriteBatch.Draw(
                    texture, new Rectangle(
                        (int)s.Position.X, (int)s.Position.Y, 10, 10
                    ),
                    match s.Name with
                    | "Blaak" -> Color.Aqua
                    | "Pernis" -> Color.Aquamarine
                    | "Rotterdam Centraal" -> Color.OrangeRed
                    | "Pijnacker Centrum" -> Color.Beige
                    | "Nesselande" -> Color.BlanchedAlmond
                    | _ -> Color.Green
                )
        );

        spriteBatch.Draw(
            texture, new Rectangle(
                (int)GameState.Metro.Position.X, (int)GameState.Metro.Position.Y, 15, 15
            ), 
            Color.Red
        ); 

        spriteBatch.End()