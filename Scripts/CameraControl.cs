using Godot;
using System;

public partial class CameraControl : Camera3D
{
    [Export] public double RotationTime = .5; 

    
    private int _rotation = 0;
    private double _angleFraction = 0;
    private double _direction = 1;
    private double _currentTime = 0;
    private bool _isMoving = false;
    public override void _Ready()
    {
        base._Ready();
        _angleFraction = 2 * Math.PI / 6;
    }

    public override void _Input(InputEvent inputEvent)
    {
        base._Input(inputEvent);

        float fov = Fov;
        //var fov = self.fov;
        
        if (inputEvent.IsActionPressed(ActionNames.CameraZoomIn)) fov -= 1;
        if (inputEvent.IsActionPressed(ActionNames.CameraZoomOut)) fov += 1;
        fov = Math.Clamp(fov, 30, 90);
        Fov = fov;
        
        if (!_isMoving)
        {
            if (inputEvent.IsActionPressed(ActionNames.CameraOrbitCw))
            {
                _direction = -1;
                _isMoving = true;
            }

            if (inputEvent.IsActionPressed(ActionNames.CameraOrbitCcw))
            {
                _direction = 1;
                _isMoving = true;
            }
        }
    }

    public override void _Process(double dt)
    {
        if (_isMoving)
        {
            _currentTime += dt * 1 / RotationTime;
            if (_currentTime > 1.0)
            {
                _currentTime = 1.0;
                _isMoving = false;
            }

            var t = EaseInOutCubic(_currentTime);
            var angle = _direction * t * _angleFraction + Math.PI * 2 * _rotation / 6.0;
            var parent = GetParent<Node3D>();
            var transform = parent.Transform;
            transform.Basis = new Basis(Vector3.Up, (float)angle);
            parent.Transform = transform;
            
            if (!_isMoving)
            {
                _rotation = (_rotation + (int)_direction + 6) % 6;
                _currentTime = 0;
            }
        }
    }
    
    float EaseInOutCubic(double x) {
        return (float)(x < 0.5 ? 4 * x * x * x : 1 - Math.Pow(-2 * x + 2, 3) / 2);
    }
}
