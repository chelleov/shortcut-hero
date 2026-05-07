using Godot;
using System;

public partial class ShortcutInstruction : Control
{
	Label _shortcutName;
	Label _shortcutDescription;
	Label _shortcutKeys;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_shortcutName = GetNode<Label>("Instruction List/Current Shortcut Name");
		_shortcutDescription = GetNode<Label>("Instruction List/Current Shortcut Description");
		_shortcutKeys = GetNode<Label>("Instruction List/Current Shortcut Target");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetShortcutVisible(Boolean visible)
	{
		_shortcutName.Visible = visible;
	}

	public void SetInstructionsVisible(Boolean visible)
	{
		_shortcutDescription.Visible = visible;
		_shortcutKeys.Visible = visible;
	}

	public void SetShortcut(ShortcutData shortcut)
	{
		_shortcutName.Text = shortcut.Name;
		_shortcutDescription.Text = shortcut.Description;
		_shortcutKeys.Text = string.Join(" + ", shortcut.Keys);
	}
}
