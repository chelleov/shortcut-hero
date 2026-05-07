using Godot;
using System;

public partial class ScorePanel : Control
{
	private int _round;
	private Label _currentScore;
	private Label _currentRound;
	private GameState _gameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_currentScore = GetNode<Label>("Current Score");
		_currentRound = GetNode<Label>("Current Round");
		_gameState = GetNode<GameState>("/root/GameState");
		ResetRound();
		ResetScore();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ResetRound()
	{
		_currentRound.Text = "1";
		_round = 1;
	}

	public void IncreaseRound()
	{
		_currentRound.Text = (int.Parse(_currentRound.Text) + 1).ToString();
		_round++;
	}

	public void AddScore(int amount)
	{
		_gameState.Score += amount;
		_currentScore.Text = _gameState.Score.ToString("N0");
	}

	public void ResetScore()
	{
		_currentScore.Text = "0";
		_gameState.Score = 0;
	}
}
