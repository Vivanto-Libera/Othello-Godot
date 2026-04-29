using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tensorboard;
using TorchSharp;

namespace othello
{
	public partial class Agent : Node
	{
		[Signal]
		public delegate void AiSelectedMoveEventHandler(int moveIndex);
		public OthelloModel model;
		private Thread aiThread;
		public void StartThread(Board board)
		{
			aiThread = new Thread(() => SelectMove(board));
			aiThread.Start();
		}
		private void SelectMove(Board board) 
		{
			MCEdge rootEdge = new MCEdge(null, null);
			rootEdge.N = 1;
			MCNode rootNode = new MCNode(board, rootEdge);
			MCTS MctsSearcher = new MCTS(model, 200);
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
			CallDeferred(nameof(EmitMove), idx);
		}
		public Agent(OthelloModel model) 
		{
			this.model = model;
		}
		private void EmitMove(int moveIndex)
		{
			EmitSignal(SignalName.AiSelectedMove, moveIndex);
		}
	}
}
