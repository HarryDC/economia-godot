using Godot;
using System;
using System.Collections.Generic;
using Hexagonal;

public partial class Cursor : Node3D
{
    [Export] public World World;
    [Export] public MeshInstance3D Selection;
    [Export] public Node3D CurrentTile;
    [Export] public bool KeyMove = true;
    


    private Hex _current_hex;
    private Layout _layout;
    private Tile.Kind _kind;
    private Dictionary<Tile.Kind, Node3D> _nodes = new();
    private Plane _ground = Plane.PlaneXZ;
    
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
        base._Input(inputEvent);
        
        if (KeyMove)
        {
            MoveWithKeyboard(inputEvent);
        }
        else
        {
            MoveWithMouse(inputEvent);
        }

        int newType = (int)_kind;
        if (inputEvent.IsActionPressed(ActionNames.ActionNewTileNext))
        {
            newType = (newType + 1) % Enum.GetValues<Tile.Kind>().Length;
        }

        if (inputEvent.IsActionPressed(ActionNames.ActionNewTilePrevious))
        {
            int count = Enum.GetValues<Tile.Kind>().Length; 
            newType = (newType - 1 + count) % count;
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

    private void MoveWithMouse(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion eventMouseMotion)
        {
            var pos = GetViewport().GetMousePosition();
            var camera = GetViewport().GetCamera3D();
            var start = camera.ProjectRayOrigin(pos);
            var direction = camera.ProjectRayNormal(pos);

            var intersection = _ground.IntersectsRay(start, direction);
            if (!intersection.HasValue) return;
            
            var fractionalHex = World.Layout.PixelToHex(new Point(intersection.Value.X, intersection.Value.Z));
            var hex = new Hex((int)(fractionalHex.q + 0.5), (int)(fractionalHex.r + 0.5));
            if (_current_hex.q == hex.q && _current_hex.r == hex.r) return;
            
            _current_hex = hex;
            EmitSignal(SignalName.SelectionChanged, hex.q, hex.r);
        }
    }

    private void MoveWithKeyboard(InputEvent inputEvent)
    {
        int current_r = _current_hex.r;
        int current_q = _current_hex.q;
        
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorDown)) current_r += 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorUp)) current_r -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorLeft)) current_q -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorRight)) current_q += 1;
        // Update Current position before place action
        current_q = (current_q + World.Height) % World.Height;
        current_r = (current_r + World.Width) % World.Width;
        
        if (_current_hex.q != current_q || _current_hex.r != current_r)
        {
            _current_hex = new Hex(current_q, current_r);
            EmitSignal(SignalName.SelectionChanged, current_q, current_r);
        }
    }

    public override void _Process(double delta)
    {
        Selection.Position = _layout.HexToPixel(_current_hex).ToVector3();
        CurrentTile.Position = Selection.Position;
    }
}
