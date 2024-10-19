using System;
using Godot;
using Hexagonal;
using System.Collections.Generic;

[GlobalClass]
public partial class World : Resource
{
    World()
    {
        Width = 6;
        Height = 6;
        tiles = new List<Tile>(Width * Height);
        Size = 1.0f / Math.Sqrt(3.0f);
        OriginQ = 3;
        OriginR = 3;
    }

    [Export] public int Width { get; private set; }

    [Export] public int Height { get; private set; }

    [Export]
    public double Size { get; set; }

    [Export]
    public int OriginQ { get; set; }
    
    [Export]
    public int OriginR { get; set; }
    
    private List<Tile> tiles;

    public Layout Layout
    {
        get
        {
            var layout = new Layout(Layout.pointy, new Point(Size, Size),
                new Point(0,0));
            // Offset board so the center of the screen is at 3/3
            var corner = layout.HexToPixel(new Hex(-OriginQ, -OriginR));
            layout = new Layout(Layout.pointy, new Point(Size, Size), corner);
            return layout;
        }
    }
    
    

    public void SetTile(Tile tile, Hex location)
    {
        
    }
    
    public Tile  GetTile(Hex location)
    {
        return null;
    }
}