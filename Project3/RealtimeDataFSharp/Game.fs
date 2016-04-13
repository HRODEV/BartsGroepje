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
        match metro.RideStops with
        | current :: t ->       let next = t.Head
                                let duration = (float32)(next.Arrival - current.Arrival).TotalSeconds
                                
                                let newTime = (time + ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f))
                                let disX = (next.Position.X - current.Position.X)
                                let disY = (next.Position.Y - current.Position.Y)
                                let newPosX = if current.Position.X <> next.Position.X then easeInOutQuad2 time current.Position.X disX duration else next.Position.X
                                let newPosY = if current.Position.Y <> next.Position.Y then easeInOutQuad2 time current.Position.Y disY duration else next.Position.Y
                                Done(newTime >= duration , {metro with Position = new Vector2(newPosX, newPosY); Status = Moving newTime})
        | _ -> Done(true, metro)

    let inline WaitMetro (r : float32) (g : GameTime) : Coroutine<float32, Metro> = fun metro ->   
        let newTime = (r - ((float32)dt.ElapsedGameTime.Milliseconds / 1000.0f))
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



let screenWidth = 1920.0f
let screenHeight = 1080.0f
let scaler = ScalePosition (new Vector2(screenWidth, screenHeight)) (CreateViewRectangle(new Point(81493, 443705), new Point(100854, 427738))) 0.95f

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
    let mutable gameState = GameState.Zero()

    override x.Initialize() =
        graphics.PreferredBackBufferWidth <- (int) screenWidth;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight <- (int) screenHeight;   // set this value to the desired height of your window

        graphics.IsFullScreen <- true
        graphics.ApplyChanges();
        x.IsMouseVisible <- true;

        do base.Initialize()

    override this.LoadContent() =
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let mutable metrotexture = new Texture2D(this.GraphicsDevice, 1, 1)
        metrotexture.SetData([| Color.White |])
       
        let textures : Map<String, Texture2D> =
            Map.empty.
                Add("background", spriteLoader "Rotterdam.png" this.GraphicsDevice).
                Add("station", spriteLoader "metroicon.png" this.GraphicsDevice).
                Add("metro", metrotexture)

        gameState <- {GameState.Create(scaler) with Textures = textures}
        gameState <- {gameState with Metros = [Metro.Create(A, gameState.Rides.Head.RideStops |> Array.map(fun x -> RideStop.Create(x, scaler, 0)) |> List.ofArray, MetroProgram2())]
        }
        ()
 
    override this.Update (gameTime) =
        gameState <- GameState.Update(gameState, gameTime)
        ()
        
    override this.Draw (gameTime) =
        do this.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        GameState.Draw(gameState, spriteBatch)
        spriteBatch.End()