module HeatMap2
open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Entities

type Cell = 
    {
        Position : Vector2
        Width : float32
        Height : float32
        Power : float32
        Bounds : Rectangle
    } with
    static member Create(pos : Vector2, w : float32, h : float32) =
        {
            Position = pos
            Width = w
            Height = h
            Power = 0.0f
            Bounds = new Rectangle((int)pos.X, (int)pos.Y, (int)w, (int)h);
        }
    static member Draw(cell : Cell, texture : Texture2D, spriteBatch : SpriteBatch) =
        spriteBatch.Draw(texture, new Rectangle((int)cell.Position.X, (int)cell.Position.Y, (int)cell.Width, (int)cell.Height), new Color(255.0f * (cell.Power * 2.0f), 255.0f - (255.0f * cell.Power), 0.0f, 0.25f))
    static member Update(cell : Cell, metros : Metro list, fetchNew : Boolean) =
        let amountToAdd = if fetchNew then metros |> List.fold(fun i m -> if cell.Bounds.Intersects(new Rectangle((int)m.Position.X, (int)m.Position.Y, 20, 20)) then i + 0.025f else i) 0.0f else 0.0f
        let newPower = if cell.Power > 0.0f then cell.Power - 0.005f else 0.0f
        // Yellow when half, Red when full, Green when empty
        {cell with
            Power = if newPower < 0.0f then min (0.0f + amountToAdd) 1.0f else min (newPower + amountToAdd) 1.0f}

type HeatMap =
    {
        Rows : int
        Length : int
        Cells : Cell list
        Active : Boolean
    } with
    static member Create(r : int, l : int) =
        let rec createCellList i j list : Cell list =  
            if i >= 0 then
                let width = (1920 / l)
                let height = (1080 / r)
                let cell = Cell.Create(new Vector2(float32(i * width) + 3.0f, float32(j * height) + 3.0f), float32(width - 6), float32(height - 6))
                let newList = [cell] @ list
                createCellList (i - 1) j newList
            else if j > 0 then
                createCellList (r) (j - 1) list
            else 
                list
        {
            Rows = r
            Length = l
            Cells = createCellList r l []
            Active = false
        }
    static member Draw(heatmap : HeatMap, texture : Texture2D, spriteBatch : SpriteBatch) =
        heatmap.Cells |> List.iter(fun cell -> Cell.Draw(cell, texture, spriteBatch))
    static member Update(heatmap : HeatMap, metros : Metro list) =
        let toggle = if Keyboard.GetState().IsKeyDown(Keys.H) then true else false
        {heatmap with Active = toggle; Cells = heatmap.Cells |> List.map(fun cell -> Cell.Update(cell, metros, heatmap.Active))}