using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hexagonal;

public class Tile
{
    public Tile(Type type)
    {
        Node = GetTileNode(type);
    }
    
    public enum Type
    {
        Farm,
        House,
        Village
    }
    
    private static readonly Dictionary<Type, string> _names = new()
    {
        { Type.Farm, "building-farm.glb" },
        { Type.House, "building-house.glb" },
        { Type.Village, "building-village.glb" }
    };
    
    public Node3D Node;

    public static Node3D GetTileNode(Type type)
    {
        Debug.Assert(_names.ContainsKey(type));
        var scene = 
            GD.Load<PackedScene>("res://Assets/Tiles/" + _names[type]);
        Debug.Assert(scene != null, $"Could not find {_names[type]} in Assets/Tiles");
        return scene.Instantiate() as Node3D;
    }
}
