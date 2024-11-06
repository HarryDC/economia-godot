using Godot;
using System;

public partial class CameraControl : Camera3D
{
    public override void _Input(InputEvent inputEvent)
    {
        base._Input(inputEvent);

        float fov = Fov;
        //var fov = self.fov;
        
        if (inputEvent.IsActionPressed(ActionNames.CameraZoomIn)) fov -= 1;
        if (inputEvent.IsActionPressed(ActionNames.CameraZoomOut)) fov += 1;
        fov = Math.Clamp(fov, 30, 90);
        Fov = fov;

        //self.fov = fov;

    }
}
