using Godot;
using System;

public partial class GameState : Node
{
	public bool IsGameStarted = false;
	public int Score = 0;
	public int Round = 1;
	public int DifficultyLevel = 1;
	public float TimeLimit = 10.0f;
	public float TimeAddition = 1.0f;
	public string JsonFilePath;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}