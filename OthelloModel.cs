using System;
using System.Collections.Generic;
using System.Linq;
using static TorchSharp.torch;
using TorchSharp.Modules;
using TorchSharp;

namespace othello
{
    public class OthelloModel
    {
        public OthelloModel() 
        {
            var model = torch.jit.load("res://model/Othello.pt");
            model.eval();
        }
    }
}
