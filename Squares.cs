using Godot;
using othello;
using System;
using System.Collections.Generic;

public partial class Squares : Node
{
	private Square[] squares = new Square[64];
	public override void _Ready()
	{
		for (int i = 0; i < 64; i++) 
		{
			squares[i] = GetNode<Square>(i.ToString());
		}
	}
	public void SetStateFromBoard(Board board) 
	{
		for (int i = 0; i < 8; i++) 
		{
			for (int j = 0; j < 0; j++) 
			{
				squares[i * 8 + j].setState(board.board[i, j]);
			}
		}
	}
	public void setLegalMoves(List<int> moves) 
	{
		foreach (int move in moves) 
		{
			squares[move].setState(Square.State.LEGALMOVE);
		}
	}
}
