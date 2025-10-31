using Godot;
using System;
using System.Collections.Generic;
using TorchSharp;

namespace othello
{
    public struct EdgeNode
    {
        public MCEdge edge;
        public MCNode node;
        public EdgeNode(MCEdge edge, MCNode node)
        {
            this.edge = edge;
            this.node = node;
        }
    }
    public class MCEdge
    {
        public int? move;
        public int N=0;
        public float W = 0;
        public float Q = 0;
        public float P = 0;
        #nullable enable
        public MCNode? parentNode;

        public MCEdge(int? move, MCNode? parentNode) 
        {
            this.move = move;
            this.parentNode = parentNode;
        }
    }
    public class MCNode 
    {
        public Board board = new Board();
        public MCEdge parentEdge = new MCEdge(null, null);
        public List<EdgeNode> childEdgeNodes = new List<EdgeNode>();
        public float Expand(OthelloModel model) 
        {
            List<int> moves = board.LegalMoves();
            List<EdgeNode> childEdgeNodes = new List<EdgeNode>();
            foreach (int move in moves) 
            {
                Board childBoard = new Board(board);
                childBoard.ApplyMove(move);
                MCEdge childEdge = new MCEdge(move, this);
                MCNode childNode = new MCNode(childBoard, childEdge);
                childEdgeNodes.Add(new EdgeNode(childEdge, childNode));
            }
            var (prob, value) = model.Predict(board.ModelInput());
            float probSum = 0;
            foreach (EdgeNode edgeNode in childEdgeNodes) 
            {
                edgeNode.edge.P = (float)prob[0,edgeNode.edge.move].ToDouble();
                probSum += edgeNode.edge.P;
            }
            foreach (EdgeNode edgeNode in childEdgeNodes)
            {
                edgeNode.edge.P /= probSum;
            }
            return (float)value[0,0].ToDouble();
        }
        public bool IsLeaf() 
        {
            return childEdgeNodes.Count == 0;
        }
        public MCNode(Board board, MCEdge parentEdge) 
        {
            this.board = board;
            this.parentEdge = parentEdge;
        }
    }
    public class MCTS 
    {
        public OthelloModel model;
        #nullable enable
        public MCNode? rootNode = null;
        public float tau = 1;
        public float cPuct = 1;
        int times;
        public MCTS(OthelloModel model, int times) 
        {
            this.model = model;
            this.times = times;
        }
        public float UctValues(MCEdge edge, int ParentN)  
        {
            return cPuct * edge.P * ((float)Math.Sqrt(ParentN) / (1 + edge.N));
        }
        public MCNode Select(MCNode node) 
        {
            if (node.IsLeaf()) 
            {
                return node;
            }
            else 
            {
                MCNode? maxUctChild = null;
                float maxUctValue = -100000000000;
                foreach (EdgeNode edgeNode in node.childEdgeNodes) 
                {
                    float uctValues = UctValues(edgeNode.edge, edgeNode.edge.parentNode.parentEdge.N);
                    float val = edgeNode.edge.Q;
                    if (edgeNode.edge.parentNode.board.turn == Square.State.BLACK) 
                    {
                        val = -val;
                    }
                    float uctValChild = val + uctValues;
                    if (uctValChild > maxUctValue) 
                    {
                        maxUctValue = uctValChild;
                        maxUctChild = edgeNode.node;
                    }
                }
                List<MCNode> allBestChild = new List<MCNode>();
                return Select(maxUctChild);
            }
        }
        public void BackUp(float value, MCEdge edge) 
        {
            edge.N += 1;
            edge.W += value;
            edge.Q = edge.W / edge.N;
            if (edge.parentNode != null) 
            {
                if(edge.parentNode.parentEdge != null) 
                {
                    BackUp(value, edge.parentNode.parentEdge);
                }
            }
        }
        public void ExpandAndEvaluate(MCNode node) 
        {
            Square.State winner = node.board.IsTerminal();
            float v = 0;
            if (winner != Square.State.LEGALMOVE) 
            {
                if(winner == Square.State.WHITE) 
                {
                    v = 1;
                }
                else if (winner == Square.State.BLACK) 
                {
                    v = -1;
                }
                BackUp(v, node.parentEdge);
            }
            v = node.Expand(model);
            BackUp(v, node.parentEdge);
        }
        public float[] Search(MCNode rootNode) 
        {
            this.rootNode = rootNode;
            this.rootNode.Expand(model);
            for (int i = 0; i < times; i++) 
            {
                MCNode selectedNode = Select(rootNode);
                ExpandAndEvaluate(selectedNode);
            }
            int NSum = 0;
            float[] moveProbs = new float[65];
            foreach (EdgeNode edgeNode in rootNode.childEdgeNodes) 
            {
                NSum += edgeNode.edge.N;
            }
            foreach (EdgeNode edgeNode in rootNode.childEdgeNodes)
            {
                float prob = ((float)Math.Pow(edgeNode.edge.N, (1 / tau))) / (float)Math.Pow(NSum, (1 / tau));
                moveProbs[edgeNode.edge.move.Value] = prob;
            }
            return moveProbs;
        }
    }
}
