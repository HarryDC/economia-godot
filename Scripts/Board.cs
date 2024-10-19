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

    private string[] _names = new[]
    {
        "building-farm.glb",
        "building-house.glb",
        "building-village.glb"
    };
    public Board()
    {

        foreach (var name in _names)
        {
            var scene = 
                GD.Load<PackedScene>("res://Assets/Tiles/" + name);
            if (scene != null)
            {
                _tile_scenes.Add(scene);
                Debug.Print("Loaded {0}", name);
            }
            else
            {
                Debug.Print("Failed to load {0}", name);
            }
        }
        

    }
    
    public override void _Ready()
    {
        base._Ready();
        _layout = World.Layout;
        
        Debug.Assert(World != null);
        
        var scene = _tile_scenes[(int)(GD.Randi() % _tile_scenes.Count)];
        var current_hex = new Hex(3, 3);
        var pos = _layout.HexToPixel(current_hex);
        
        Node3D instance = scene.Instantiate() as Node3D;
        Tile t = new Tile
        {
            Node = instance
        };
        World.SetTile(t, current_hex);
        instance.Position = pos.ToVector3();
        AddChild(instance);
        for (int i = 0; i < 6; ++i)
        {
            scene = _tile_scenes[(int)(GD.Randi() % _tile_scenes.Count)];
            var hex = current_hex.Add(Hex.directions[i]);
            pos = _layout.HexToPixel(hex);
            instance = scene.Instantiate() as Node3D;
            instance.Position = pos.ToVector3();
            t = new Tile
            {
                Node = instance
            };
            World.SetTile(t, hex);
            AddChild(instance);
        }
    }
}
