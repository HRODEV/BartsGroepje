module SnakeDiagram
open System
open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type SnakeDiagram =
    {
        lines       :   snakeLine list
        location    :   Vector2
        speed       :   Vector2
    } with
    static member Create (location: Vector2) (speed: Vector2) =
        {
            lines = [];
            location = location;
            speed = speed;
        }
    static member Update (diagram: SnakeDiagram)=
        { diagram with
            lines = diagram.lines |> List.map (fun x -> x.Update diagram.speed)
        }


and snakeLine =
    {
        origin      : Vector2
        destination : Vector2
        sprite      : Texture2D
    } with
    member this.Draw (spriteBatch: SpriteBatch) =
        let r = new Rectangle((int)this.origin.X, (int)this.origin.Y, (int)((this.origin - this.destination).Length())+1, 1)
        let v = Vector2.Normalize(this.origin - this.destination)
        let dot = Vector2.Dot(v, Vector2.UnitX)
        let angle = (float32)(Math.Acos((float)dot))
        if this.origin.Y > this.destination.Y then
            let angle = MathHelper.TwoPi - angle
            spriteBatch.Draw(this.sprite, r, System.Nullable(), Color.White, angle, Vector2.Zero, SpriteEffects.None, 0.0f)
        else
            spriteBatch.Draw(this.sprite, r, System.Nullable(), Color.White, angle, Vector2.Zero, SpriteEffects.None, 0.0f)
    member this.Update (speed: Vector2) =
        { this with
            origin      =   this.origin + speed;
            destination =   this.destination + speed;
        }