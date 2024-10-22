using Godot;
using System;
using Hexagonal;
using System.Collections.Generic;
using System.Diagnostics;

static class HexagonalExtensions {
    public static Vector3 ToVector3(this Point p)
    {
        return new Vector3((float)p.x, 0, (float)p.y);
    }
}

public partial class Board : Node
{
    private Layout _layout;
    private List<PackedScene> _tile_scenes = new();

    [Export]
    public World World;
    
    public Board()
    {
    }
    
    public override void _Ready()
    {
        base._Ready();
        _layout = World.Layout;
        
        Debug.Assert(World != null);
        
        var random = new Random();
        var newType = random.NextEnum<Tile.Type>();
        var current_hex = new Hex(3, 3);
        var pos = _layout.HexToPixel(current_hex);

        Tile t = new Tile(newType);
        
        World.SetTile(t, current_hex);
        t.Node.Position = pos.ToVector3();
        AddChild(t.Node);
        for (int i = 0; i < 6; ++i)
        {
            newType = random.NextEnum<Tile.Type>();
            t = new Tile(newType);
            var hex = current_hex.Add(Hex.directions[i]);
            pos = _layout.HexToPixel(hex);
            t.Node.Position = pos.ToVector3();
            World.SetTile(t, hex);
            AddChild(t.Node);
        }
    }
}
