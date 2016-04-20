module HeatMap
open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type HeatPoint =
    {
        location: Vector2
        heat    : float
        color   : Color

    } with
    static member Create =
        {
            location = Vector2.Zero
            heat = 0.0
            color = new Color(0,0,0,0)
        }

let GenerateMap width height = Array2D.init width height (fun x y -> {HeatPoint.Create with location = new Vector2(float32 x, float32 y) } )

let CoolDown (map: HeatPoint [,]) = map |> Array2D.map (fun x -> {x with heat = x.heat - 0.1})