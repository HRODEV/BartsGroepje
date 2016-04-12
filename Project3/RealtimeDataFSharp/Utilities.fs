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