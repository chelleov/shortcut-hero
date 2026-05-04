using Godot;
using System;

public partial class GameOverProgress : ProgressBar
{
	Timer gameOverTimer;

	float maxTime = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gameOverTimer = GetNode<Timer>("Game Over Countdown");
		gameOverTimer.Start(maxTime);

		GD.Print("Game Over Progress Loaded");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Value = 100 * (gameOverTimer.TimeLeft / maxTime);
	}

	public Boolean AddTime(float value)
	{
		float currentTimeLeft = (float)gameOverTimer.TimeLeft;

        gameOverTimer.Stop();
        gameOverTimer.Start(currentTimeLeft + value);
		return true;
	}

	public void OnTimeAddButtonPressed()
	{
		AddTime((float)2);
	}
}
