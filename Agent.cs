using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var rand_index = torch.multinomial(moveProb, num_samples: 1);
            int idx = 0;
            for (int i = 0; i < 65; i++) 
            {
                if ((int)rand_index[i] == 1) 
                {
                    idx = i;
                    break;
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
