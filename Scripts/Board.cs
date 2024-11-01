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
        World.RootNode = this;
        
        Debug.Assert(World != null);
        
        var random = new Random();
        var center_hex = new Hex(3, 3);

        Tile t = new Tile(random.NextEnum<Tile.Kind>());
        World.SetTile(t, center_hex);

        for (int i = 0; i < 6; ++i)
        {
            t = new Tile(random.NextEnum<Tile.Kind>());
            World.SetTile(t, center_hex.Add(Hex.directions[i]));
        }
    }
}
