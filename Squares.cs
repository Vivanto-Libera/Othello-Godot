using Godot;
using othello;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Squares : Node
{
	[Signal]
	public delegate void PassedEventHandler();
	[Signal]
	public delegate void PlayerMoveEventHandler();

	private Square[] squares = new Square[64];
	public Board board = new Board();
	public override void _Ready()
	{
		for (int i = 0; i < 64; i++) 
		{
			squares[i] = GetNode<Square>(i.ToString());
		}
	}
	public void SetStateFromBoard() 
	{
		for (int i = 0; i < 8; i++) 
		{
			for (int j = 0; j < 8; j++) 
			{
				squares[i * 8 + j].setState(board.board[i, j]);
			}
		}
	}
	public void setLegalMoves() 
	{
		List<int> moves = board.LegalMoves();

		foreach (int move in moves) 
		{
			if (move == 64) 
			{
				EmitSignal(SignalName.Passed);
				break;
			}
			squares[move].setState(Square.State.LEGALMOVE);
		}
	}
	public void PlayerMoved(int index)
	{
		board.ApplyMove(index);
		EmitSignal(SignalName.PlayerMove);
	}
}
