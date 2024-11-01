using System;
using Godot;
using System.Collections.Generic;
using System.Diagnostics;

public enum Good
{
    Wood = 0,
    Wheat = 1
}

internal enum CollectionType
{
    Required,
    MaxStorage,
    Production
}

public class GoodCollection : Dictionary<Good, double> {}


public partial class Tile : Node
{
    // Actual Values
    public GoodCollection Storage = new();
    
    // Required Values
    public GoodCollection MaxStorage = new();
    public GoodCollection Required = new(); 
    public GoodCollection Output = new();
    
    public enum Kind
    {
        Farm,
        House,
        Village,
        Forest,
    }
    
    // Storage = storage + output * dt - required*dt (if storage > required for all)
    // maxStorage, required and output could depend on modifiers 
    
    private static readonly Dictionary<Kind, string> Names = new()
    {
        { Kind.Farm, "building-farm.glb" },
        { Kind.House, "building-house.glb" },
        { Kind.Village, "building-village.glb" },
        { Kind.Forest, "grass-forest.glb" },
    };
    
    public Node3D Node;
    public Kind Type;
    private double _elapsed = 0.0;
    private double _tick = 1.0;
    
    public static Node3D GetTileNode(Kind kind)
    {
        Debug.Assert(Names.ContainsKey(kind));
        var scene = 
            GD.Load<PackedScene>($"res://Assets/Tiles/{Names[kind]}");
        Debug.Assert(scene != null, $"Could not find {Names[kind]} in Assets/Tiles");
        return scene.Instantiate() as Node3D;
    }
    
    public Tile(Kind kind)
    {
        Node = GetTileNode(kind);
        Type = kind;
        var json = GD.Load<Json>("res://Assets/simulation_data.json");
    
        InitCollection(Storage);
        InitCollection(MaxStorage);
        InitCollection(Required);
        InitCollection(Output);

        var collections = new Dictionary<CollectionType, GoodCollection>()
        {
            { CollectionType.Required, Required },
            { CollectionType.Production, Output },
            { CollectionType.MaxStorage, MaxStorage }
        };
        
        // Check for 
        if (!json.Data.AsGodotDictionary().ContainsKey(kind.ToString()))
        {
            GD.PrintErr($"Could not find JSON data for {kind}");
            return;
        }
        
        // Init current values from JSON 
        var tileData = json.Data.AsGodotDictionary()[kind.ToString()].AsGodotDictionary();

        foreach (var collectionType in Enum.GetValues<CollectionType>())
        {
            if (!tileData.ContainsKey(collectionType.ToString())) continue;
            var currentCollection = collections[collectionType];
            foreach (var (goodName, value) in tileData[collectionType.ToString()].AsGodotDictionary())
            {
                try
                {
                    var goodType = Enum.Parse<Good>(goodName.ToString());
                    currentCollection[goodType] = value.AsDouble();
                }
                catch (Exception e)
                {
                    GD.PrintErr($"For {kind} in {collectionType} could not parse {goodName}: {value}");
                    GD.PrintErr(e);
                }
            }
        }
    }
    
    private void InitCollection(GoodCollection goodCollection)
    {
        foreach (var good in Enum.GetValues<Good>())
        {
            goodCollection[good] = 0;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        _elapsed += delta;
        
        if (_elapsed < _tick) return;
        _elapsed -= _tick;
        
        var canProduce = true;
        foreach (var (key, value) in Required)
        {
            if (Storage[key] < value)
            {
                canProduce = false;
                break;
            }
        }
        
        if (!canProduce) return;

        foreach (var (key, value) in Required)
        {
            Storage[key] -= value;
        }

        foreach (var (key, value) in Output)
        {
            Storage[key] += Math.Clamp(Storage[key] + value, 0, MaxStorage[key]);
        }
    }
}
