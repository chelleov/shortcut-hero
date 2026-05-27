using Godot;
using System;

public partial class GameOver : Control
{
	private Tree _scoreList;
	private GameState _gameState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scoreList = GetNode<Tree>("VBox/Score List");
		_gameState = GetNode<GameState>("/root/GameState");

		// Play game over sound
		var gameOverSound = new AudioStreamPlayer();
		gameOverSound.Stream = GD.Load<AudioStream>("res://audio/GAME OVER 1.mp3");
		AddChild(gameOverSound);
		gameOverSound.Play();

		_scoreList.Clear();

		_scoreList.HideRoot = true;
		_scoreList.HideFolding = true;

		_scoreList.SetColumnTitle(0, "Player");
		_scoreList.SetColumnTitle(1, "Score");

		_scoreList.SetColumnExpand(0, true);
		_scoreList.SetColumnExpand(1, true);

		TreeItem root = _scoreList.CreateItem();

		var scoreDevChelle = _scoreList.CreateItem(root);
		scoreDevChelle.SetText(0, "Chelle");
		scoreDevChelle.SetText(1, "999,999,999");
		scoreDevChelle.SetTextAlignment(0, HorizontalAlignment.Center);
		scoreDevChelle.SetTextAlignment(1, HorizontalAlignment.Center);

		var scoreDevShahaf = _scoreList.CreateItem(root);
		scoreDevShahaf.SetText(0, "Shahaf");
		scoreDevShahaf.SetText(1, "999,999,998");
		scoreDevShahaf.SetTextAlignment(0, HorizontalAlignment.Center);
		scoreDevShahaf.SetTextAlignment(1, HorizontalAlignment.Center);

		var scoreUser = _scoreList.CreateItem(root);
		scoreUser.SetText(0, "You");
		scoreUser.SetText(1, _gameState.Score.ToString("N0"));
		scoreUser.SetTextAlignment(0, HorizontalAlignment.Center);
		scoreUser.SetTextAlignment(1, HorizontalAlignment.Center);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ResetGameState()
	{
		_gameState.IsGameStarted = false;
		_gameState.Score = 0;
		_gameState.Round = 1;
		_gameState.DifficultyLevel = 1;
		_gameState.TimeLimit = 10.0f;
		_gameState.TimeAddition = 0.5f;
	}

	private void OnPlayAgainPressed()
	{
		ResetGameState();
		GetTree().ChangeSceneToFile("res://scenes/pre_game.tscn");
	}

	private void OnMainMenuPressed()
	{
		ResetGameState();
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
