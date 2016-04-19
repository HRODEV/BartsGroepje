module SnakeDiagram
open System
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type SnakeDiagram =
    {
        points      :   snakePoint list
        rect        :   Rectangle
        max         :   float32
        speed       :   Vector2
        playhead    :   snakePoint
        texture     :   Texture2D
    } with

    static member Create (rect: Rectangle) (speed: Vector2) (texture: Texture2D) =
        {
            points = []
            rect = rect
            max = 80.0f
            speed = speed;
            playhead = snakePoint.Zero
            texture = texture
        }

    member this.Draw (spriteBatch: SpriteBatch) =
        this.points |> List.fold (fun prev curr -> 
                                    do snakeLine.Draw spriteBatch curr.position prev.position this.texture
                                    curr) this.playhead |> ignore

    static member Update (d: SnakeDiagram) s =
        let s' = 
            match s with
            | 0 -> 0.1f
            | 1 -> 0.1f
            | 50 -> 1.0f
            | _ -> 5.0f

        let speed = new Vector2(d.speed.X * s', d.speed.Y * s')
        let points' = d.points |> List.map (fun x -> x.Update speed) |> List.filter (fun x -> x.position.X >= float32 d.rect.Left)
        { d with points = points'}

    static member AddPoint (d: SnakeDiagram) (value: float32) =
        let Y = (((float32(d.rect.Height) / d.max) * value) - 80.0f) * -1.0f
        let pos = new Vector2(float32 d.rect.Right, Y + (float32)d.rect.Top)
        let newPoint = (snakePoint.Create value pos)
        { d with points = newPoint :: d.points; playhead = newPoint }

and snakePoint =
    {
        value: float32
        position: Vector2
    } with

    static member Zero = 
        { value = 0.0f; position = Vector2.Zero }

    static member Create value position = { value = value; position = position; }

    member this.Update speed = { this with position = this.position + speed }

and snakeLine =
    static member Draw (spriteBatch: SpriteBatch) (origin: Vector2) (destination: Vector2) (sprite: Texture2D) =
        let distance = Vector2.Distance(origin, destination)
        let angle = float32 (Math.Atan2(float (destination.Y - origin.Y), float (destination.X - origin.X)))
        let middle = new Vector2(0.0f, 0.5f)
        let scale = new Vector2(distance, 4.0f)
        spriteBatch.Draw(sprite, origin, System.Nullable(), Color.Red, angle, middle, scale, SpriteEffects.None, 0.0f);