using Godot;
using System;
using Hexagonal;

public partial class Hud : Control
{
    [Export] World World;
    // Cursor where ? 

    public override void _Ready()
    {
        
    }

    public void OnSelectionChanged(int q, int r)
    {
        var title = GetNode<Label>("GridContainer/Title");
        var data = GetNode<Label>("GridContainer/Data");

        var tile = World.GetTile(new Hex(q, r));
        if (tile != null)
        {
            title.Text = $"{tile.Type.ToString()}";
            string message = "";
            foreach (var good in Enum.GetValues<Good>())
            {
                if (tile.Storage.ContainsKey(good))
                {
                    message = message + $"{good.ToString()}:{tile.Storage[good]:F0} ";
                }
            }

            data.Text = message;
        }
        else
        {
            title.Text = "No Tile Selected";
            data.Text = "";
        }
    }
}
