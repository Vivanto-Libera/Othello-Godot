using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Square : TextureRect
{
	[Export]
	public int index;
	[Signal]
	public delegate void SelectedEventHandler(int index);
	public enum State
	{
		EMPTY,
		BLACK,
		WHITE,
		LEGALMOVE,
	}
	private State state = State.EMPTY;
	public void setState(State newState) 
	{
		state = newState;
		GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, true);
		switch (state) 
		{
			case State.EMPTY:
				Texture = null;
				break;
			case State.BLACK:
				Texture = GD.Load<Texture2D>("res://image/black.png");
				break;
			case State.WHITE:
				Texture = GD.Load<Texture2D>("res://image/white.png");
				break;
			default:
				Texture = GD.Load<Texture2D>("res://image/gray.png");
				GetNode<Button>("Button").SetDeferred(Button.PropertyName.Disabled, false);
				break;
		}
		ForceUpdateTransform();
	}
	public void OnButtonPressed() 
	{
		EmitSignal(SignalName.Selected, index);
	}

}
