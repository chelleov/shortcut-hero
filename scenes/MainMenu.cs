using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public partial class MainMenu : Control
{
	FoldableContainer _gameMode;
	VBoxContainer _gameModeList;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameMode = GetNode<FoldableContainer>("Game Mode");
		_gameModeList = GetNode<VBoxContainer>("Game Mode/Game Mode List");
		
		string configurationsPath = "res://data";
		string[] jsonFiles = GetJsonFiles(configurationsPath);

		foreach (string file in jsonFiles)
		{
			var button = new Button
			{
				Text = Path.GetFileNameWithoutExtension(file)
			};

			button.Pressed += () =>
			{
				GD.Print($"Clicked: {file}");
				_gameMode.Folded = true;
				_gameMode.Title = button.Text;
			};

			button.Visible = true;
			_gameModeList.AddChild(button);
			_gameModeList.Visible = true;
		}
		_gameMode.Title = Path.GetFileNameWithoutExtension(jsonFiles[0]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private string[] GetJsonFiles(string path)
	{
		var files = new List<string>();
		var dir = DirAccess.Open(path);

		if (dir == null)
			return Array.Empty<string>();

		dir.ListDirBegin();
		string file = dir.GetNext();

		while (file != "")
		{
			if (!dir.CurrentIsDir() && file.EndsWith(".json"))
				files.Add($"{path}/{file}");

			file = dir.GetNext();
		}

		dir.ListDirEnd();
		return files.ToArray();
	}

	private void OnPlayButtonPressed()
	{
		GD.Print("Play button pressed");
		GetTree().ChangeSceneToFile("res://scenes/pre_game.tscn");
	}

	public void OnExitButtonPressed()
	{
		GD.Print("Exit button pressed");
		GetTree().Quit();
	}
}
