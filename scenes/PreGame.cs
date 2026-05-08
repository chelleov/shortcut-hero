using Godot;
using System;

public partial class PreGame : Control
{
	private GameState _gameState;
	private Label _prepareMessage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameState = GetNode<GameState>("/root/GameState");
		_prepareMessage = GetNode<Label>("CenterContainer/VBox/Prepare Message");
		_prepareMessage.Text = "";

		if (!_gameState.IsGameStarted)
		{
			_gameState.IsGameStarted = true;
			_gameState.Score = 0;
			_gameState.Round = 1;
		}
		else
		{
			// _gameState.DifficultyLevel = 1;
			_gameState.Round++;
			
			if(_gameState.TimeAddition > 0.2f)
			{
				_gameState.TimeAddition -= 0.1f;
			}

			if(_gameState.TimeLimit > 5f)
			{
				_gameState.TimeLimit -= 0.5f;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.IsEcho())
		{
            GetTree().ChangeSceneToFile("res://scenes/play_window.tscn");
		}
	}
}
