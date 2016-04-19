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
open GameState
open SnakeDiagram

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
    let mutable snakeDiagram = SnakeDiagram.Create (new Rectangle(0,0,100,50)) (new Vector2(0.1f, 0.0f))

    override x.Initialize() =
        graphics.PreferredBackBufferWidth <- (int) screenWidth;  // set this value to the desired width of your window
        graphics.PreferredBackBufferHeight <- (int) screenHeight;   // set this value to the desired height of your window

        graphics.IsFullScreen <- false
        graphics.ApplyChanges();
        x.IsMouseVisible <- true;

        do base.Initialize()

    override this.LoadContent() =
        System.IO.Directory.SetCurrentDirectory("Content")
        do spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        let mutable plainTexture = new Texture2D(this.GraphicsDevice, 1, 1)
        plainTexture.SetData([| Color.White |])
       
        let textures : Map<String, Texture2D> =
            Map.empty.
                Add("plain", plainTexture).
                Add("background", spriteLoader "Rotterdam.png" this.GraphicsDevice).
                Add("InfoBox", spriteLoader "infobox.png" this.GraphicsDevice).
                Add("station", spriteLoader "metroicon.png" this.GraphicsDevice).
                Add("pause", spriteLoader "pause.png" this.GraphicsDevice).
                Add("metro", plainTexture).
                Add("line", plainTexture)

        let fonts = 
            Map.empty.
                Add("font1", {Image = (spriteLoader "Font.png" this.GraphicsDevice); Data = FontLoader.Load("Font.fnt")}).
                Add("Arial", {Image = (spriteLoader "Arial_0.png" this.GraphicsDevice); Data = FontLoader.Load("Arial.fnt")}).
                Add("Timer", {Image = (spriteLoader "Timer_0.png" this.GraphicsDevice); Data = FontLoader.Load("Timer.fnt")})

        gameState <- {GameState.Create(scaler, [StateFetchRideLogic(); MainStateLogic()], textures) with Fonts = fonts}
        ()
 
    override this.Update (gameTime) =
        gameState <- GameState.Update(gameState, gameTime)
        ()
        
    override this.Draw (gameTime) =
        let mutable metrotexture = new Texture2D(this.GraphicsDevice, 1, 1)
        metrotexture.SetData([| Color.White |])

        do this.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        GameState.Draw(gameState, spriteBatch)
        spriteBatch.End()