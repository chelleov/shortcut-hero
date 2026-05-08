using Godot;
using System;

public partial class PreGame : Control
{
	private GameState _gameState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		if (_gameState.IsGameStarted)
		{
			_gameState.IsGameStarted = true;
			_gameState.Score = 0;
			_gameState.Round = 1;
		}
		else
		{
			// _gameState.DifficultyLevel = 1;
			_gameState.TimeLimit -= 0.5f;
			_gameState.TimeAddition -= 0.1f;

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
            GetTree().ChangeSceneToFile("res://scenes/play_window.tscn");
		}
	}
}
