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
}
