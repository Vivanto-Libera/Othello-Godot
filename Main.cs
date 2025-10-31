using Godot;
using othello;
using System;
using TorchSharp;
public partial class Main : Node
{
	public override void _Ready()
	{
		var model = torch.jit.load("Othello.pt");
		model.eval();
		var input = torch.randn(new long[] { 1, 3, 8, 8 });
		using (torch.no_grad()) 
		{
			var output = model.forward(input);
			GD.Print(output.GetType());
		}
	}
}
