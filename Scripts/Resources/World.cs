using System;
using Godot;
using Hexagonal;
using System.Collections.Generic;

[GlobalClass]
public partial class World : Resource
{
    World() : this(6, 6, 1.0f / Math.Sqrt(3.0f), 3, 3) {}
    World(int width, int height, double size, int originq, int originr)
    {
        Width = width;
        Height = height;
        Size = size;
        OriginQ = originq;
        OriginR = originr;
        _tiles = new List<Tile>(new Tile[width*height]);
        _layout = new Layout(Layout.pointy, new Point(Size, Size),
            new Point(0,0));
        // Offset board so the center of the screen is at 3/3
        var corner = _layout.HexToPixel(new Hex(-OriginQ, -OriginR));
        _layout = new Layout(Layout.pointy, new Point(Size, Size), corner);
    }

    [Export] public int Width { get; private set; }

    [Export] public int Height { get; private set; }

    [Export]
    public double Size { get; set; }

    [Export]
    public int OriginQ { get; set; }
    
    [Export]
    public int OriginR { get; set; }
    
    public Node RootNode { get; set; }
    
    private List<Tile> _tiles;

    private Layout _layout;
    public Layout Layout => _layout;

    public void AddTile(Tile.Kind type, Hex location)
    {
        var tile = new Tile(type);
        RootNode.AddChild(tile);
        _tiles[location.r * Height + location.q] = tile;
        var pos = _layout.HexToPixel(location);
        tile.Node.Position = pos.ToVector3();
        tile.AddChild(tile.Node);
    }
    
    public Tile  GetTile(Hex location)
    {
        return _tiles[location.r * Height + location.q];
    }
}