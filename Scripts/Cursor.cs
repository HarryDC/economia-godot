using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using Hexagonal;

public partial class Cursor : Node3D
{
    [Export] public World World;
    [Export] public MeshInstance3D Selection;
    [Export] public Node3D CurrentTile;
    


    private Hex _current_hex;
    private Layout _layout;
    private Tile.Kind _kind;
    private Dictionary<Tile.Kind, Node3D> _nodes = new();
    
    [Signal]
    public delegate void SelectionChangedEventHandler(int newQ, int newR);
    
    public override void _Ready()
    {
        base._Ready();
        _layout = World.Layout;
        _current_hex = new Hex(World.OriginQ, World.OriginR);
        _kind = Tile.Kind.Farm;
        var mesh = new BoxMesh();
        mesh.Size = new Vector3(1, 1, 1);
        Selection.Mesh = mesh;
        
        foreach(var type in Enum.GetValues<Tile.Kind>())
        {
            _nodes[type] = Tile.GetTileNode(type);
            _nodes[type].Visible = false;
            CurrentTile.AddChild(_nodes[type]);
        }

        _nodes[_kind].Visible = true;
    }
    
    public override void _Input(InputEvent inputEvent)
    {
        int current_r = _current_hex.r;
        int current_q = _current_hex.q;
        int newType = (int)_kind;
        
        base._Input(inputEvent);
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorDown)) current_r += 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorUp)) current_r -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorLeft)) current_q -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorRight)) current_q += 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionNewTileNext))
        {
            newType = (newType + 1) % Enum.GetValues<Tile.Kind>().Length;
        }

        if (inputEvent.IsActionPressed(ActionNames.ActionNewTilePrevious))
        {
            int count = Enum.GetValues<Tile.Kind>().Length; 
            newType = (newType - 1 + count) % count;
        }

        // Update Current position before place action
        current_q = (current_q + World.Height) % World.Height;
        current_r = (current_r + World.Width) % World.Width;
        if (_current_hex.q != current_q || _current_hex.r != current_r)
        {
            _current_hex = new Hex(current_q, current_r);
            EmitSignal(SignalName.SelectionChanged, current_q, current_r);
        } 

        if (inputEvent.IsActionPressed(ActionNames.ActionNewTilePlace))
        {
            World.AddTile(_kind, _current_hex);
        }
        
        if (newType != (int)_kind)
        {
            _kind = (Tile.Kind)newType;
            foreach (var (type, node) in _nodes)
            {
                node.Visible = (type == _kind);
            }
        }

        if (World.GetTile(_current_hex) == null)
        {
            Selection.Visible = false;
            CurrentTile.Visible = true;
        }
        else
        {
            Selection.Visible = true;
            CurrentTile.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        Selection.Position = _layout.HexToPixel(_current_hex).ToVector3();
        CurrentTile.Position = Selection.Position;
    }
}
