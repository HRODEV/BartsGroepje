module Game
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data
open Coroutines
open Entities
open Utilities
open System

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
                            do! SetMetroStatus (Waiting (0.0f))
                            return ()
                        else
                            do! yield_
        | Arrived ->    return ()
      }



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
        Metros = []
        StationList = []
        Map = Unchecked.defaultof<Texture2D>
        Time = new DateTime()
    }

    override x.Initialize() =
        graphics.PreferredBackBufferWidth <- 1920;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight <- 1080;   // set this value to the desired height of your window
        graphics.IsFullScreen <- true
        graphics.ApplyChanges();
        x.IsMouseVisible <- true;
        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)

        let BackgroundMap = spriteLoader "Rotterdam.png" this.GraphicsDevice
        let MetroIcon = spriteLoader "metroicon.png" this.GraphicsDevice

        texture <- new Texture2D(this.GraphicsDevice, 1, 1)
        texture.SetData([| Color.White |])

        let mutable count = 2.0f

        let stationList = 
            (stationData.GetSample().Value
            |> Array.toList
            |> List.map (fun st ->
                count <- count + 2.0f
                printfn "Name: %A X: %A Y: %A" st.Name st.X st.Y
                let scaler = ScaleStationPosition (new Vector2((float32)graphics.PreferredBackBufferWidth, (float32)graphics.PreferredBackBufferHeight)) (CreateViewRectangle(new Point(81493, 443705), new Point(100854, 427738))) 0.95f
                //printfn "Name: %A X: %A Y: %A" st.Name ((((float32)st.X - 83299.0f) / 12.329f) - 0.0f) (((((float32)st.Y - 452622.0f) / 12.863f) - 0.0f) * -1.0f)
                {
                    Name = st.Name;
                    Next = None;
                    Arrival = count;
                    Position = scaler (new Vector2((float32)st.X, (float32)st.Y))
                    Texture = MetroIcon;
                }
            ))

        let newMetro = {Line = A; Station = (combinedTrack stationList.Head stationList.Tail); Position = stationList.Head.Position; Status = TrainStatus.Waiting 1.0f; Behaviour = MetroProgram2()}
        let secondMetro = {Line = A; Station = (combinedTrack stationList.Tail.Tail.Head stationList.Tail.Tail.Tail.Tail); Position = stationList.Head.Position; Status = TrainStatus.Waiting 1.0f; Behaviour = MetroProgram2()}

        GameState <- {GameState with Metros = [newMetro; secondMetro]; StationList = stationList; Map = BackgroundMap}
        ()
 
    override this.Update (gameTime) =

        let leftCo, newTrain = costep (Metro.Update gameTime) GameState.Metros.Head
        let leftCo, newTrain2 = costep (Metro.Update gameTime) GameState.Metros.[1]

        GameState <- {GameState with Metros = [newTrain; newTrain2]}
        ()
        
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.CornflowerBlue

        
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

        spriteBatch.Draw(GameState.Map, new Rectangle(0, 0, GameState.Map.Width, GameState.Map.Height), Color.White)

        GameState.StationList 
        |> List.iter(
            fun s -> 
                spriteBatch.Draw(
                    s.Texture, new Rectangle(
                        (int)s.Position.X - 10, (int)s.Position.Y - 10, 20, 20
                    ),
                    match s.Name with
                    | "De Akkers" -> Color.Aqua
                    | "Pernis" -> Color.Aquamarine
                    | "Rotterdam Centraal" -> Color.OrangeRed
                    | "De Terp" -> Color.Beige
                    | "Nesselande" -> Color.BlanchedAlmond
                    | "Den Haag Centraal" -> Color.Red
                    | _ -> Color.White
                )
        );

        GameState.Metros
        |> List.iter(
            fun m ->
                spriteBatch.Draw(
                    texture, new Rectangle(
                        (int)m.Position.X - 7, (int)m.Position.Y - 7, 15, 15
                    ), 
                    Color.Red
            ); 
        )
        
        spriteBatch.End()