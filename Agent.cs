using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorboard;
using TorchSharp;

namespace othello
{
	public class Agent
	{
		public OthelloModel model;
		public int SelectMove(Board board) 
		{
			MCEdge rootEdge = new MCEdge(null, null);
			rootEdge.N = 1;
			MCNode rootNode = new MCNode(board, rootEdge);
			MCTS MctsSearcher = new MCTS(model, 400);
			float[] moveProb = MctsSearcher.Search(rootNode);
			int idx = -1;
			float maxProb = -1;
			for (int i = 0; i < 65; i++) 
			{
				if (moveProb[i] > maxProb) 
				{
					maxProb = moveProb[i];
					idx = i;
				}
			}
			return idx;
		}
		public Agent(OthelloModel model) 
		{
			this.model = model;
		}
	}
}
