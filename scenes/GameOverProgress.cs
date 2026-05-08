using Godot;
using System;

public partial class GameOverProgress : ProgressBar
{
	private GameState _gameState;
	private Timer gameOverTimer;

	float maxTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameOverTimer = GetNode<Timer>("Game Over Countdown");
		_gameState = GetNode<GameState>("/root/GameState");
		maxTime = _gameState.TimeLimit;
		gameOverTimer.Start(maxTime);

		GD.Print("Game Over Progress Loaded");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Value = 100 * (gameOverTimer.TimeLeft / maxTime);
	}

	public Boolean AddTime()
	{
		float currentTimeLeft = (float)gameOverTimer.TimeLeft;

        gameOverTimer.Stop();
        gameOverTimer.Start(currentTimeLeft + _gameState.TimeAddition);
		return true;
	}
}
