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
}
