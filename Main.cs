using Godot;
using othello;
using System;
using System.Threading.Tasks;
using TorchSharp;
public partial class Main : Node
{
	[Signal]
	public delegate void GameOverEventHandler();

	Square.State playerColor;
	Agent agent;
	Squares squares;

	public void Reset() 
	{
		squares.board = new Board();
		squares.SetStateFromBoard();
		GetNode<Button>("Black").Visible = true;
		GetNode<Button>("White").Visible = true;
		GetNode<Label>("Label").Visible = false;
	}
	public void OnBlackPressed() 
	{
		playerColor = Square.State.BLACK;
		squares.setLegalMoves();
		GetNode<Button>("Black").Visible = false;
		GetNode<Button>("White").Visible = false;
	}
	public void OnwhitePressed() 
	{
		playerColor = Square.State.WHITE;
		GetNode<Button>("Black").Visible = false;
		GetNode<Button>("White").Visible = false;
		AIMove();
	}
	public async void AIMove() 
	{
		squares.board.ApplyMove(agent.SelectMove(new Board(squares.board)));
		if(squares.board.IsTerminal() != Square.State.LEGALMOVE) 
		{
			EmitSignal(SignalName.GameOver);
		}
		await ToSignal(GetTree().CreateTimer(0.5), Timer.SignalName.Timeout);
		squares.CallDeferred("SetStateFromBoard");
		squares.CallDeferred("setLegalMoves");
	}
	
	public void Moved() 
	{
		squares.CallDeferred("SetStateFromBoard");
		CallDeferred("AIMove");
	}
	public void Passed() 
	{
		squares.board.ApplyMove(64);
		if (squares.board.IsTerminal() != Square.State.LEGALMOVE)
		{
			EmitSignal(SignalName.GameOver);
		}
		else
		{
			Moved();
		}
	}
	public async void GameOvered() 
	{
		Square.State winner = squares.board.IsTerminal();
		string message = "";
		if(winner == Square.State.EMPTY) 
		{
			message = "Draw";
		}
		else if (winner == Square.State.BLACK) 
		{
			message = "Black Win";
		}
		else 
		{
			message = "White Win";
		}
		await ShowMessage(message);
		Reset();
	}
	public async Task ShowMessage(string message) 
	{
		GetNode<Label>("Label").Text = message;
		GetNode<Label>("Label").Visible = true;
		await ToSignal(GetTree().CreateTimer(3), Timer.SignalName.Timeout);
		GetNode<Label>("Label").Visible = false;
	}

	public override void _Ready()
	{
		squares = GetNode<Squares>("Squares");
		agent = new Agent(new OthelloModel());
		Reset();
	}
}
