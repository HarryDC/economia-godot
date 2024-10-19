using Godot;
using System;
using Hexagonal;

public partial class Cursor : Node3D
{
    [Export] public World World;
    [Export] public MeshInstance3D Selection;
    [Export] public Node3D Tile;

    private Hex _current_hex;
    private Layout _layout;
    public override void _Ready()
    {
        base._Ready();
        _layout = World.Layout;
        _current_hex = new Hex(World.OriginQ, World.OriginR);
        var mesh = new BoxMesh();
        mesh.Size = new Vector3(1, 1, 1);
        Selection.Mesh = mesh;
    }

    public override void _Input(InputEvent inputEvent)
    {
        int current_r = _current_hex.r;
        int current_q = _current_hex.q;
        base._Input(inputEvent);
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorDown)) current_r += 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorUp)) current_r -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorLeft)) current_q -= 1;
        if (inputEvent.IsActionPressed(ActionNames.ActionCursorRight)) current_q += 1;

        current_q = (current_q + World.Height) % World.Height;
        current_r = (current_r + World.Width) % World.Width;

        _current_hex = new Hex(current_q, current_r);
    }

    public override void _Process(double delta)
    {
        Selection.Position = _layout.HexToPixel(_current_hex).ToVector3();
    }
}
