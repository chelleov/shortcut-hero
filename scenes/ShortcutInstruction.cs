using Godot;
using System;

public partial class ShortcutInstruction : Control
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
		if (_shortcutName.LabelSettings == null)
		{
			_shortcutName.LabelSettings = new LabelSettings();
		}
		_shortcutName.LabelSettings.FontColor = visible ? Colors.White : Colors.Gray;
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
