using Godot;
using System;
using System.Collections.Generic;

public partial class GameState : Node
{
	public bool IsGameStarted = false;
	public int Score = 0;
	public int Round = 1;
	public int DifficultyLevel = 1;
	public float TimeLimit = 20.0f;
	public float TimeAddition = 1f;
	public string JsonFilePath;

	public bool IsDarkMode = true;
	private AudioStreamPlayer _successSoundPlayer;
	private AudioStreamPlayer _failSoundPlayer;

	// Store original LabelSettings so we can always detect colors from the originals
	private readonly Dictionary<ulong, LabelSettings> _originalLabelSettings = new();
	private readonly Dictionary<ulong, Dictionary<string, StyleBox>> _originalStyleBoxes = new();

	private StyleBox GetOriginalStyleBox(Control control, string name)
	{
		ulong id = control.GetInstanceId();
		if (!_originalStyleBoxes.ContainsKey(id))
			_originalStyleBoxes[id] = new Dictionary<string, StyleBox>();

		if (!_originalStyleBoxes[id].ContainsKey(name))
		{
			if (control.HasThemeStyleboxOverride(name))
				_originalStyleBoxes[id][name] = (StyleBox)control.GetThemeStylebox(name).Duplicate();
			else
				_originalStyleBoxes[id][name] = null;
		}

		return _originalStyleBoxes[id][name];
	}

	private void RestoreOriginalStyleBox(Control control, string name)
	{
		var orig = GetOriginalStyleBox(control, name);
		if (orig != null)
			control.AddThemeStyleboxOverride(name, orig);
		else
			control.RemoveThemeStyleboxOverride(name);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().NodeAdded += OnNodeAdded;
		
		_successSoundPlayer = new AudioStreamPlayer();
		_successSoundPlayer.Stream = GD.Load<AudioStream>("res://audio/success.wav");
		AddChild(_successSoundPlayer);

		_failSoundPlayer = new AudioStreamPlayer();
		_failSoundPlayer.Stream = GD.Load<AudioStream>("res://audio/fail.wav");
		AddChild(_failSoundPlayer);

		CallDeferred(MethodName.UpdateTheme, GetTree().Root);
	}

	public void PlaySuccessSound()
	{
		_successSoundPlayer.Play();
	}

	public void PlayFailSound()
	{
		_failSoundPlayer.Play();
	}

	public void ToggleTheme()
	{
		IsDarkMode = !IsDarkMode;
		UpdateTheme(GetTree().Root);
	}

	private void OnNodeAdded(Node node)
	{
		UpdateTheme(node);
	}

	public void UpdateTheme(Node node)
	{
		// Background color
		if (IsDarkMode)
			RenderingServer.SetDefaultClearColor(new Color(0.05882353f, 0.16470589f, 0.26666668f, 1f));
		else
			RenderingServer.SetDefaultClearColor(new Color(0.92f, 0.93f, 0.95f, 1f));

		// Labels - store originals, then apply based on mode
		if (node is Label label && label.LabelSettings != null)
		{
			ulong id = label.GetInstanceId();

			// Store the original on first encounter
			if (!_originalLabelSettings.ContainsKey(id))
				_originalLabelSettings[id] = (LabelSettings)label.LabelSettings.Duplicate();

			var orig = _originalLabelSettings[id];
			var origColor = orig.FontColor;

			if (IsDarkMode)
			{
				// Restore original dark mode settings
				label.LabelSettings = (LabelSettings)orig.Duplicate();
			}
			else
			{
				var ls = (LabelSettings)orig.Duplicate();

				// Categorize the ORIGINAL color to decide light mode treatment
				bool isAccentColor = origColor.R < 0.5f && origColor.B > 0.7f;  // cyan/blue accent
				bool isRedColor = origColor.R > 0.8f && origColor.G < 0.4f;      // red (GAME OVER)
				bool isSubdued = origColor.R < 0.6f && origColor.G < 0.7f && origColor.B < 0.8f; // muted (ROUND, SCORE, etc.)

				if (isRedColor)
				{
					// Keep red visible in both modes
				}
				else if (isAccentColor)
				{
					ls.FontColor = new Color(0.0f, 0.4f, 0.7f, 1f);
				}
				else if (isSubdued)
				{
					ls.FontColor = new Color(0.4f, 0.45f, 0.5f, 1f);
				}
				else
				{
					ls.FontColor = new Color(0.1f, 0.1f, 0.15f, 1f);
				}

				ls.ShadowColor = new Color(0, 0, 0, 0.1f);
				label.LabelSettings = ls;
			}
		}
		else if (node is Label labelNoSettings && labelNoSettings.LabelSettings == null)
		{
			if (IsDarkMode)
				labelNoSettings.RemoveThemeColorOverride("font_color");
			else
				labelNoSettings.AddThemeColorOverride("font_color", new Color(0.1f, 0.1f, 0.15f, 1f));
		}

		// RichTextLabel
		if (node is RichTextLabel rtl)
		{
			if (IsDarkMode)
				rtl.RemoveThemeColorOverride("default_color");
			else
				rtl.AddThemeColorOverride("default_color", new Color(0.1f, 0.1f, 0.15f, 1f));
		}

		// PanelContainer backgrounds (cards, score panel)
		if (node is PanelContainer panel)
		{
			if (IsDarkMode)
			{
				RestoreOriginalStyleBox(panel, "panel");
			}
			else
			{
				// Read the now-clean original style
				var style = GetOriginalStyleBox(panel, "panel") ?? panel.GetThemeStylebox("panel");
				if (style is StyleBoxFlat flat)
				{
					var newFlat = (StyleBoxFlat)flat.Duplicate();
					if (node is ShortcutInstruction)
					{
						newFlat.BgColor = new Color(0.85f, 0.92f, 0.98f, 0.95f);
						newFlat.BorderColor = new Color(0.55f, 0.75f, 0.90f, 0.5f);
					}
					else
					{
						newFlat.BgColor = new Color(1f, 1f, 1f, 0.95f);
						newFlat.BorderColor = new Color(0.78f, 0.8f, 0.84f, 0.6f);
					}
					panel.AddThemeStyleboxOverride("panel", newFlat);
				}
			}
		}

		// Button styles
		if (node is Button btn && btn is not CheckButton)
		{
			if (IsDarkMode)
			{
				// Restore original tscn styles
				foreach (string s in new[] { "normal", "hover", "pressed", "focus" })
					RestoreOriginalStyleBox(btn, s);
				foreach (string s in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_hover_pressed_color", "font_focus_color" })
					btn.RemoveThemeColorOverride(s);
			}
			else
			{
				bool isBtnBlueAccent = false;
				foreach (string state in new[] { "normal", "hover", "pressed", "focus" })
				{
					var style = GetOriginalStyleBox(btn, state) ?? btn.GetThemeStylebox(state);
					if (style is StyleBoxFlat flat)
					{
						var newFlat = (StyleBoxFlat)flat.Duplicate();
						bool isBlueAccent = flat.BgColor.B > 0.7f && flat.BgColor.R < 0.5f;
						if (isBlueAccent)
						{
							isBtnBlueAccent = true;
							// Make it dark blue in light mode
							if (state == "normal")
								newFlat.BgColor = new Color(0.06f, 0.18f, 0.35f, 1f);
							else if (state == "hover")
								newFlat.BgColor = new Color(0.10f, 0.22f, 0.42f, 1f);
							else if (state == "pressed")
								newFlat.BgColor = new Color(0.04f, 0.12f, 0.25f, 1f);
							else
								newFlat.BgColor = new Color(0.06f, 0.18f, 0.35f, 1f);
							
							newFlat.BorderColor = new Color(0.04f, 0.12f, 0.25f, 1f);
						}
						else
						{
							newFlat.BgColor = state == "hover"
								? new Color(0.88f, 0.93f, 0.98f, 1f)
								: new Color(0.90f, 0.94f, 0.98f, 1f);
							newFlat.BorderColor = new Color(0.55f, 0.72f, 0.88f, 1f);
						}
						btn.AddThemeStyleboxOverride(state, newFlat);
					}
				}

				Color btnTextColor = isBtnBlueAccent
					? Colors.White
					: new Color(0.15f, 0.15f, 0.2f, 1f);

				btn.AddThemeColorOverride("font_color", btnTextColor);
				btn.AddThemeColorOverride("font_hover_color", btnTextColor);
				btn.AddThemeColorOverride("font_pressed_color", btnTextColor);
				btn.AddThemeColorOverride("font_hover_pressed_color", btnTextColor);
				btn.AddThemeColorOverride("font_focus_color", btnTextColor);
			}
		}

		// ProgressBar
		if (node is ProgressBar bar)
		{
			if (IsDarkMode)
			{
				RestoreOriginalStyleBox(bar, "background");
			}
			else
			{
				var bg = GetOriginalStyleBox(bar, "background") ?? bar.GetThemeStylebox("background");
				if (bg is StyleBoxFlat bgFlat)
				{
					var newBg = (StyleBoxFlat)bgFlat.Duplicate();
					newBg.BgColor = new Color(0.82f, 0.84f, 0.88f, 1f);
					newBg.BorderColor = new Color(0.7f, 0.72f, 0.76f, 0.5f);
					bar.AddThemeStyleboxOverride("background", newBg);
				}
			}
		}

		// Tree (game over score list)
		if (node is Tree tree)
		{
			if (IsDarkMode)
			{
				RestoreOriginalStyleBox(tree, "title_button_normal");
				RestoreOriginalStyleBox(tree, "title_button_hover");
				RestoreOriginalStyleBox(tree, "title_button_pressed");
				tree.RemoveThemeColorOverride("font_color");
				tree.RemoveThemeColorOverride("title_button_color");
			}
			else
			{
				tree.AddThemeColorOverride("font_color", new Color(0.1f, 0.1f, 0.15f, 1f));
				tree.AddThemeColorOverride("title_button_color", new Color(0.3f, 0.35f, 0.4f, 1f));

				var titleStyle = new StyleBoxFlat();
				titleStyle.BgColor = new Color(0.85f, 0.87f, 0.9f, 1f);
				titleStyle.ContentMarginLeft = 8;
				titleStyle.ContentMarginRight = 8;
				titleStyle.ContentMarginTop = 4;
				titleStyle.ContentMarginBottom = 4;
				tree.AddThemeStyleboxOverride("title_button_normal", titleStyle);
				tree.AddThemeStyleboxOverride("title_button_hover", titleStyle);
				tree.AddThemeStyleboxOverride("title_button_pressed", titleStyle);
			}
		}

		// CheckButton
		if (node is CheckButton check)
		{
			if (IsDarkMode)
			{
				foreach (string s in new[] { "font_color", "font_pressed_color", "font_hover_color", "font_hover_pressed_color", "font_focus_color", "font_disabled_color" })
					check.RemoveThemeColorOverride(s);
			}
			else
			{
				var checkColor = new Color(0.15f, 0.15f, 0.2f, 1f);
				check.AddThemeColorOverride("font_color", checkColor);
				check.AddThemeColorOverride("font_pressed_color", checkColor);
				check.AddThemeColorOverride("font_hover_color", checkColor);
				check.AddThemeColorOverride("font_hover_pressed_color", checkColor);
				check.AddThemeColorOverride("font_focus_color", checkColor);
				check.AddThemeColorOverride("font_disabled_color", checkColor);
			}
		}

		// FoldableContainer
		if (node is FoldableContainer fold)
		{
			fold.AddThemeColorOverride("font_color", Colors.White);
		}

		foreach (Node child in node.GetChildren())
		{
			UpdateTheme(child);
		}
	}
}