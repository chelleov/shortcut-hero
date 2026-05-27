using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class PlayWindow : Control
{
    private List<ShortcutData> _shortcuts = new();
    private ShortcutInstruction _currentShortcutInstruction;
    private ShortcutInstruction _nextShortcutInstruction;
    private ScorePanel _scorePanel;
    private RichTextLabel _userKeysLabel;
    private List<string> _pressedKeys = new();
	private ShortcutData currentShortcut;
	private RandomNumberGenerator _rng = new RandomNumberGenerator();
	private GameOverProgress _gameOverProgress;
    private GameState _gameState;
    private List<ShortcutData> _shortcutRound = new();
    private int _currentIndex = 0;
    private const int RoundSize = 10;

    public override void _Ready()
    {
		_rng.Randomize();

        _currentShortcutInstruction = GetNode<ShortcutInstruction>("Instructions Container/Current Group/Current Instruction");
		_nextShortcutInstruction = GetNode<ShortcutInstruction>("Instructions Container/Next Group/Next Instruction");
		_scorePanel = GetNode<ScorePanel>("Score Panel");
		_userKeysLabel = GetNode<RichTextLabel>("User Keys");
		_gameOverProgress = GetNode<GameOverProgress>("Game Over Progress");
        _gameState = GetNode<GameState>("/root/GameState");

        GD.Print("PlayWindow ready");
        LoadShortcuts();
        GenerateNewRound();
		LoadNewShortcut();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void GenerateNewRound()
    {
        _shortcutRound.Clear();
        _currentIndex = 0;
        for (int i = 0; i < RoundSize; i++)
        {
            int randomInt = _rng.RandiRange(0, _shortcuts.Count - 1);
            _shortcutRound.Add(_shortcuts[randomInt]);
        }
    }

    private void LoadNewShortcut()
    {
		_userKeysLabel.Text = "";
		_pressedKeys.Clear();

        // If we've used all shortcuts in this round, go to pre-game
        if (_currentIndex >= _shortcutRound.Count)
        {
            GetTree().ChangeSceneToFile("res://scenes/pre_game.tscn");
            return;
        }

        currentShortcut = _shortcutRound[_currentIndex];
        GD.Print($"Setting shortcut instruction to: {currentShortcut.Name} ({_currentIndex + 1}/{RoundSize})");

        // Show current shortcut with full instructions
        _currentShortcutInstruction.SetShortcut(currentShortcut);
        _currentShortcutInstruction.SetInstructionsVisible(true);

        // Show the next shortcut (name only, no details), or hide if last
        if (_currentIndex + 1 < _shortcutRound.Count)
        {
            _nextShortcutInstruction.SetShortcut(_shortcutRound[_currentIndex + 1]);
            _nextShortcutInstruction.SetInstructionsVisible(false);
        }
        else
        {
            _nextShortcutInstruction.Visible = false;
        }

        _currentIndex++;
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
                if (sequenceMatches)
                {
                    _gameState.PlayFailSound();
                }
                sequenceMatches = false; // Once the sequence is broken, remaining keys are marked red
                formattedKeys.Add($"[color=red]{key}[/color]");
            }
        }

        _userKeysLabel.Text = string.Join(" + ", formattedKeys);

        // Success condition: If all keys match and the count is the same, load the next one
        if (sequenceMatches && _pressedKeys.Count == currentShortcut.Keys.Count)
        {
            GD.Print($"Match found for {currentShortcut.Name}! Loading next...");
            _gameState.PlaySuccessSound();
            // CallDeferred is used to ensure the state change happens safely
            CallDeferred(MethodName.LoadNewShortcut);
            _scorePanel.AddScore((int)_gameOverProgress.Value);
			_gameOverProgress.AddTime();
        }
    }

    private void LoadShortcuts()
    {
        string filePath = _gameState.JsonFilePath;

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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent)
		{
            string keyName = OS.GetKeycodeString(keyEvent.Keycode);
            
            // Normalize Godot key names to match JSON shortcut key names
            keyName = NormalizeKeyName(keyName);

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

    public void OnGameOverCountdownTimeout()
    {
        GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
    }

    private static readonly Dictionary<string, string> KeyNameMap = new()
    {
        { "Control", "Ctrl" },
        { "Minus", "-" },
        { "Plus", "+" },
        { "Equal", "=" },
        { "Semicolon", ";" },
        { "Colon", ":" },
        { "Period", "." },
        { "Comma", "," },
        { "Slash", "/" },
        { "Backslash", "\\" },
        { "BracketLeft", "[" },
        { "BracketRight", "]" },
        { "Apostrophe", "'" },
        { "QuoteLeft", "`" },
        { "Greater", ">" },
        { "Less", "<" },
        { "Percent", "%" },
        { "KP Enter", "Enter" },
        { "KP Add", "+" },
        { "KP Subtract", "-" },
    };

    private static string NormalizeKeyName(string godotKeyName)
    {
        return KeyNameMap.TryGetValue(godotKeyName, out string mapped) ? mapped : godotKeyName;
    }
}
