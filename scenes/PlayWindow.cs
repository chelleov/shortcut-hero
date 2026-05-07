using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class PlayWindow : Control
{
    private List<ShortcutData> _shortcuts = new();
    private Button timeAddButton;
    private Label _shortcutNameLabel;
    private Label _shortcutDescriptionLabel;
    private Label _neededShortcutLabel;
    private RichTextLabel _userKeysLabel;
    private List<string> _pressedKeys = new();
	private ShortcutData currentShortcut;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	private GameOverProgress _gameOverProgress;

    public override void _Ready()
    {
		_rng.Randomize();

        timeAddButton = GetNode<Button>("Time Add Button");
		_userKeysLabel = GetNode<RichTextLabel>("User Keys");
		_gameOverProgress = GetNode<GameOverProgress>("Game Over Progress");

        GD.Print("PlayWindow ready");
        LoadShortcuts();	
		LoadNewShortcut();


    }

    private void LoadNewShortcut()
    {
		_userKeysLabel.Text = "";
		_pressedKeys.Clear();
		int randomInt = _rng.RandiRange(0, _shortcuts.Count - 1);
		currentShortcut = _shortcuts[randomInt];
		
        _shortcutNameLabel = GetNode<Label>("Current Shortcut Name");
        _shortcutDescriptionLabel = GetNode<Label>("Current Shortcut Description");
        _neededShortcutLabel = GetNode<Label>("Current Shortcut Target");
        
        _shortcutNameLabel.Text = currentShortcut.Name;
        _shortcutDescriptionLabel.Text = currentShortcut.Description;
        _neededShortcutLabel.Text = string.Join(" + ", currentShortcut.Keys);
    }

    private void UpdateUserKeysUI()
    {
        if (_userKeysLabel == null) return;

        if (_pressedKeys.Count == 0)
        {
            _userKeysLabel.Text = ""; // Or some default text
            return;
        }

        List<string> formattedKeys = new();
        bool sequenceMatches = true;

        for (int i = 0; i < _pressedKeys.Count; i++)
        {
            string key = _pressedKeys[i];
            
            // A key is "correct" only if it matches the JSON sequence AND all keys before it were also correct
            if (sequenceMatches && i < currentShortcut.Keys.Count && key == currentShortcut.Keys[i])
            {
                formattedKeys.Add($"[color=green]{key}[/color]");
            }
            else
            {
                sequenceMatches = false; // Once the sequence is broken, remaining keys are marked red
                formattedKeys.Add($"[color=red]{key}[/color]");
            }
        }

        _userKeysLabel.Text = string.Join(" + ", formattedKeys);

        // Success condition: If all keys match and the count is the same, load the next one
        if (sequenceMatches && _pressedKeys.Count == currentShortcut.Keys.Count)
        {
            GD.Print($"Match found for {currentShortcut.Name}! Loading next...");
            // CallDeferred is used to ensure the state change happens safely
            CallDeferred(MethodName.LoadNewShortcut);
			_gameOverProgress.AddTime(0.5f);
        }
    }

    private void LoadShortcuts()
    {
        string filePath = "res://data/office_shortcuts.json";

        if (!FileAccess.FileExists(filePath))
        {
            GD.PrintErr($"JSON file not found at: {filePath}");
            return;
        }

        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = file.GetAsText();

        try
        {
            _shortcuts = JsonSerializer.Deserialize<List<ShortcutData>>(jsonString);
            GD.Print($"Loaded {_shortcuts.Count} shortcuts successfully.");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to parse JSON: {e.Message}");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
            string keyName = OS.GetKeycodeString(keyEvent.Keycode);
            
            // Normalize names to match JSON (e.g., "Control" -> "Ctrl")
            if (keyName == "Control") keyName = "Ctrl";

            if (keyEvent.Pressed && !keyEvent.IsEcho())
            {
                if (!_pressedKeys.Contains(keyName))
                {
                    _pressedKeys.Add(keyName);
                    UpdateUserKeysUI();
                }
            }
            else if (!keyEvent.Pressed)
            {
                if (_pressedKeys.Contains(keyName))
                {
                    _pressedKeys.Remove(keyName);
                    UpdateUserKeysUI();
                }
            }
		}
	}
}
