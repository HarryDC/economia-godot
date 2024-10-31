using Godot;
using System;

public partial class Hud : Control
{
    [Export] World world;
    // Cursor where ? 

    public override void _Ready()
    {
        
    }

    public void OnSelectionChanged(int q, int r)
    {
        var title = GetNode<Label>("GridContainer/Title");
        var label = GetNode<Label>("GridContainer/Data");
        label.Text = $"{q}/{r}";
        title.Text = "Woods:";
    }
    
}
