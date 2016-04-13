module Utilities

open System.Collections.Generic
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open FSharp.Data




let easeInOutQuad2 (currentTime : float32) (startPos : float32) (endPos : float32) (duration : float32) =   
    let newTime = currentTime / (duration / 2.0f)
    if ( newTime < 1.0f ) then
        startPos + newTime * newTime * endPos / 2.0f
    else 
        let newTime' = newTime - 1.0f
        startPos + (-endPos / 2.0f * (newTime' * (newTime' - 2.0f) - 1.0f))

let CreateViewRectangle (p1: Point, p2: Point) = new Rectangle(p1, new Point(p2.X - p1.X, p1.Y - p2.Y))

let ScalePosition (screen: Vector2) (view: Rectangle) (padding: float32) (position: Vector2) =
    let paddedHeight = screen.Y * padding
    let paddedWidth = screen.X * padding
    let viewwidth = (float32)view.Width
    let viewheight = (float32)view.Height

    let pixelsPerUnit = 
        if (viewwidth / (viewheight / paddedHeight)) > screen.X then
            (viewwidth / paddedWidth)
        else
            (viewheight / paddedHeight)

    
    let posX = (position.X - (float32)view.Left) / pixelsPerUnit
    let posY = ((position.Y - (float32)view.Top) / pixelsPerUnit) * -1.0f
    let areaPixelWidth = viewwidth / pixelsPerUnit
    let areaPixelHeight = viewheight / pixelsPerUnit

    let thisX = posX + ((screen.X - areaPixelWidth) / 2.0f)
    let thisY = posY + ((screen.Y - areaPixelHeight) / 2.0f)

    new Vector2(posX + ((screen.X - areaPixelWidth) / 2.0f), posY + ((screen.Y - areaPixelHeight) / 2.0f))