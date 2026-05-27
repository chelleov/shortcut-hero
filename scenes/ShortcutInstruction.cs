using Godot;
using System;

public partial class ShortcutInstruction : PanelContainer
{
	Label _emptySpace;
	Label _shortcutName;
	Label _shortcutDescription;
	Label _shortcutKeys;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_emptySpace = GetNode<Label>("Instruction List/Empty Space");
		_shortcutName = GetNode<Label>("Instruction List/Current Shortcut Name");
		_shortcutDescription = GetNode<Label>("Instruction List/Current Shortcut Description");
		_shortcutKeys = GetNode<Label>("Instruction List/Current Shortcut Target");
		_emptySpace.Text = "";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetInstructionsVisible(Boolean visible)
	{
		var gameState = GetNode<GameState>("/root/GameState");
		if (_shortcutName.LabelSettings == null)
		{
			_shortcutName.LabelSettings = new LabelSettings();
		}
		if (visible)
		{
			_shortcutName.LabelSettings.FontColor = gameState.IsDarkMode
				? Colors.White
				: new Color(0.1f, 0.1f, 0.15f, 1f);
		}
		else
		{
			_shortcutName.LabelSettings.FontColor = gameState.IsDarkMode
				? Colors.Gray
				: new Color(0.45f, 0.5f, 0.55f, 1f); // Darker grey for light mode readability
		}
		_shortcutDescription.Visible = visible;
		_shortcutKeys.Visible = visible;
		_emptySpace.Visible = !visible;
	}

	public void SetShortcut(ShortcutData shortcut)
	{
		_shortcutName.Text = shortcut.Name;
		_shortcutDescription.Text = shortcut.Description;
		_shortcutKeys.Text = string.Join(" + ", shortcut.Keys);
		GD.Print($"Set instruction to {shortcut.Name} with keys: {string.Join(" + ", shortcut.Keys)}");
	}
}
