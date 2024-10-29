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

internal class GoodCollection : Dictionary<Good, double> {}


public class Tile
{
    // Actual Values
    private GoodCollection _storage = new();
    
    // Required Values
    private GoodCollection _maxStorage = new();
    private GoodCollection _required = new(); 
    private GoodCollection _output = new();
    
    public enum Type
    {
        Farm,
        House,
        Village
    }
    
    // Storage = storage + output * dt - required*dt (if storage > required for all)
    // maxStorage, required and output could depend on modifiers 
    
    private static readonly Dictionary<Type, string> Names = new()
    {
        { Type.Farm, "building-farm.glb" },
        { Type.House, "building-house.glb" },
        { Type.Village, "building-village.glb" }
    };
    
    public Node3D Node;
    
    public static Node3D GetTileNode(Type type)
    {
        Debug.Assert(Names.ContainsKey(type));
        var scene = 
            GD.Load<PackedScene>("res://Assets/Tiles/" + Names[type]);
        Debug.Assert(scene != null, $"Could not find {Names[type]} in Assets/Tiles");
        return scene.Instantiate() as Node3D;
    }
    
    public Tile(Type type)
    {
        Node = GetTileNode(type);
        var json = GD.Load<Json>("res://Assets/simulation_data.json");
    
        InitCollection(_storage);
        InitCollection(_maxStorage);
        InitCollection(_required);
        InitCollection(_output);

        var collections = new Dictionary<CollectionType, GoodCollection>()
        {
            { CollectionType.Required, _required },
            { CollectionType.Production, _output },
            { CollectionType.MaxStorage, _maxStorage }
        };
        
        // Check for 
        if (!json.Data.AsGodotDictionary().ContainsKey(type.ToString()))
        {
            GD.PrintErr($"Could not find JSON data for {type}");
            return;
        }
        
        // Init current values from JSON 
        var tileData = json.Data.AsGodotDictionary()[type.ToString()].AsGodotDictionary();

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
                    GD.PrintErr($"For {type} in {collectionType} could not parse {goodName}: {value}");
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
    
    public void Process(float dt)
    {
        var canProduce = true;
        foreach (var (key, value) in _required)
        {
            if (_storage[key] < value)
            {
                canProduce = false;
                break;
            }
        }
        
        if (!canProduce) return;

        foreach (var (key, value) in _required)
        {
            _storage[key] -= value;
        }

        foreach (var (key, value) in _output)
        {
            _storage[key] += value;
        }
    }
}
