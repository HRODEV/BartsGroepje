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
    } with

    static member Create (rect: Rectangle) (speed: Vector2) =
        {
            points = [];
            rect = rect;
            max = 60.0f
            speed = speed;
            playhead = snakePoint.Zero
        }

    member this.Draw (spriteBatch: SpriteBatch) (sprite: Texture2D) =
        this.points |> List.fold (fun prev curr -> 
                                    do snakeLine.Draw spriteBatch curr.position prev.position sprite
                                    curr) this.playhead |> ignore

    static member Update (d: SnakeDiagram) s =
        let max = d.points |> List.fold (fun a x -> if x.value > a then x.value else a) 0.0f
        let s' = match s with
                | 0 -> 0.1f
                | 1 -> 0.1f
                | 50 -> 1.0f
                | _ -> 5.0f

        let speed = new Vector2(d.speed.X * s', d.speed.Y * s')
        let points' = d.points |> List.map (fun x -> x.Update speed) |> List.filter (fun x -> x.position.X <= float32 d.rect.Right)
        { d with points = points'; max = max}

    static member AddPoint (d: SnakeDiagram) (value: float32) =
        let Y = (float32(d.rect.Height) / d.max * value)
        let pos = new Vector2(float32 d.rect.Left, Y)
        let newPoint = (snakePoint.Create value pos)
        { d with points = newPoint :: d.points; playhead = newPoint}

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
        let scale = new Vector2(distance, 1.0f)
        spriteBatch.Draw(sprite, origin, System.Nullable(), Color.Red, angle, middle, scale, SpriteEffects.None, 0.0f);

    (*
        let r = new Rectangle((int)origin.X, (int)origin.Y, (int)((origin - destination).Length())+1, 1)
        let v = Vector2.Normalize(origin - destination)
        let dot = Vector2.Dot(v, Vector2.UnitX)
        let angle = (float32)(Math.Acos((float)dot))
        if origin.Y > destination.Y then
            let angle = MathHelper.TwoPi - angle
            spriteBatch.Draw(sprite, r, System.Nullable(), Color.White, angle, Vector2.Zero, SpriteEffects.None, 0.0f)
        else
            spriteBatch.Draw(sprite, r, System.Nullable(), Color.White, angle, Vector2.Zero, SpriteEffects.None, 0.0f)

            *)