using Godot;
using System;

public partial class PlayWindow : Control
{
	Button timeAddButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timeAddButton = GetNode<Button>("Time Add Button");
        GD.Print("PlayWindow ready");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (keyEvent.AltPressed && keyEvent.Keycode == Key.T)
			{
				GD.Print($"{keyEvent.Keycode} detected and cancelled");
				GetViewport().SetInputAsHandled();
			} else
			{
				GD.Print($"Key pressed: {keyEvent.Keycode}");
			}
		}
	}
}
