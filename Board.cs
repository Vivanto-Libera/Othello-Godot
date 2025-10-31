using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using static Square.State;
using static TorchSharp.torch;

namespace othello
{
    public class Board
    {
        public Square.State[,] board = new Square.State[8,8];
        public Square.State turn = BLACK;
        private int passCount = 0;
        public Square.State IsTerminal() 
        {
            if(passCount != 2) 
            {
                return LEGALMOVE;
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
            int[] rowDirection = [-1, -1, 0, 1, 1, 1, 0, -1];
            int[] colDirection = [0, 1, 1, 1, 0, -1, -1, -1];
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
        public void ApplyMove(int move) 
        {
            if (move == 64) 
            {
                passCount++;
            }
            else 
            {
                passCount = 0;
                int[] theMove = IndexToMove(move);
                int r = theMove[0];
                int c = theMove[1];
                board[r, c] = turn;
                bool[] fliped = [false, false, false, false, false, false, false, false];
                int[] rowDirection = [-1, -1, 0, 1, 1, 1, 0, -1];
                int[] colDirection = [0, 1, 1, 1, 0, -1, -1, -1];
                for (int i = 0; i < 8; i++) 
                {
                    int newR = r + rowDirection[i];
                    int newC = c + colDirection[i];
                    if (newR >= 8 || newR < 0 || newC >= 8 || newC < 0) 
                    {
                        continue;
                    }
                    if (board[newR, newC] == EMPTY || board[newR, newC] == turn) 
                    {
                        continue;
                    }
                    while (true)
                    {
                        newR += rowDirection[i];
                        newC += colDirection[i];
                        if (newR >= 8 || newR < 0 || newC >= 8 || newC < 0) 
                        {
                            break;
                        }
                        if (board[newR, newC] == EMPTY)
                        {
                            break;
                        }
                        if (board[newR,newC] == turn) 
                        {
                            fliped[i] = true;
                            break;
                        }
                    }
                }
                for (int j = 0; j < 8; j++)
                {
                    if (!fliped[j])
                    {
                        continue;
                    }
                    int newR = r;
                    int newC = c;
                    while (true)
                    {
                        newR += rowDirection[j];
                        newC += rowDirection[j];
                        if (board[newR, newC] == turn) 
                        {
                            break;
                        }
                        board[newR, newC] = turn;
                    }
                }
            }
            if(turn == BLACK) 
            {
                turn = WHITE;
            }
            else 
            {
                turn = BLACK;
            }
        }
        public static int[] IndexToMove(int index) 
        {
            if (index == 64) 
            {
                return new int[2] { -1, -1 };
            }
            int[] move = new int[2] { index / 8, index % 8 };
            return move;
        }
        public static int MoveToIndex(int r, int c) 
        {
            if (r == -1) 
            {
                return 64;
            }
            return r * 8 + c;
        }
        public Tensor ModelInput() 
        {
            Tensor input = zeros(new long[] {1,3,8,8});
            for (int i = 0; i < 8; i++) 
            {
                for(int j = 0; j < 8; j++) 
                {
                    if (board[i,j] == turn) 
                    {
                        input[0, 0, i, j] = 1;
                    }
                    else if (board[i, j] != EMPTY) 
                    {
                        input[0, 1, i, j] = 1;
                    }
                }
            }
            List<int> moves = LegalMoves();
            foreach (int m in moves) 
            {
                if(m != 64) 
                {
                    int[] index = IndexToMove(m);
                    int i = index[0];
                    int j = index[1];
                    input[0, 2, i, j] = 1;
                }
            }
            return input;
        }
        public Board(Board aBoard) 
        {
            Array.Copy(aBoard.board, board, board.Length);
            turn = aBoard.turn;
            passCount = aBoard.passCount;
        }
        public Board() 
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = EMPTY;
                }
            }
            board[3, 3] = WHITE;
            board[4, 4] = WHITE;
            board[3, 4] = BLACK;
            board[4, 3] = BLACK;
        }
    }
}
