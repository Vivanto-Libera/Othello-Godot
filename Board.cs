using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using static Square.State;

namespace othello
{
    public class Board
    {
        public Square.State[,] board = new Square.State[8,8];
        public Square.State turn = BLACK;
        private int passCount = 0;
        public Square.State isTerminal() 
        {
            if(passCount != 2) 
            {
                return Square.State.EMPTY;
            }
            int black = 0;
            int white = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == BLACK) 
                    {
                        black++;
                    }
                    else if (board[i, j] == WHITE) 
                    {
                        white++;
                    }
                }
            }
            if (black > white) 
            {
                return BLACK;
            }
            else if (black < white) 
            {
                return WHITE;
            }
            return EMPTY;
        }
        public List<int> LegalMoves() 
        {
            List<int> legalMoves = new List<int>();
            int[] rowDirection = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };
            int[] colDirection = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };
            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < 8; j++) 
                {
                    if (board[i, j] != EMPTY) 
                    {
                        continue;
                    }
                    for (int k = 0; k < 8; k++) 
                    {
                        if (legalMoves.Contains(MoveToIndex(i, j))) 
                        {
                            break;
                        }
                        int newRow = i + rowDirection[k];
                        int newCol = j + colDirection[k];
                        if (newRow >= 8 || newRow < 0 || newCol >= 8 || newCol < 0) 
                        {
                            break;
                        }
                        if (board[newRow, newCol] != EMPTY && board[newRow, newCol] != turn) 
                        {
                            while (true) 
                            {
                                newRow += rowDirection[k];
                                newCol += colDirection[k];
                                if (newRow >= 8 || newRow < 0 || newCol >= 8 || newCol < 0 || board[newRow, newCol] == EMPTY) 
                                {
                                    break;
                                }
                                if(board[newRow, newCol] == turn) 
                                {
                                    legalMoves.Add(MoveToIndex(i, j));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if(legalMoves.Count == 0) 
            {
                legalMoves.Add(64);
            }
            return legalMoves;
        }
        public int[] IndexToMove(int index) 
        {
            if (index == 64) 
            {
                return new int[2] { -1, -1 };
            }
            int[] move = new int[2] { index / 8, index % 8 };
            return move;
        }
        public int MoveToIndex(int r, int c) 
        {
            if (r == -1) 
            {
                return 64;
            }
            return r * 8 + c;
        }
        public Board(Board aBoard) 
        {
            Array.Copy(aBoard.board, board, board.Length);
            turn = aBoard.turn;
        }
    }
}
