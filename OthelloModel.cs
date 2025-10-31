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
        torch.jit.ScriptModule model;
        public ValueTuple<torch.Tensor, torch.Tensor> Predict(torch.Tensor input) 
        {
            return (ValueTuple<torch.Tensor, torch.Tensor>)model.forward(input);
        }
        public OthelloModel() 
        {
            model = torch.jit.load("Othello.pt");
            model.eval();
        }
    }
}
