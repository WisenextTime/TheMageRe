using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Range = System.Range;
using Timer = Godot.Timer;

namespace TheMage.Scripts;

public partial class Ui : CanvasLayer
{
	private RichTextLabel _text;
	private Control _dialogue;
	[Signal] public delegate void InputActionEventHandler();

	public override void _Ready()
	{
		_text = GetNode<RichTextLabel>("Dialogue/Back/Content");
		_dialogue = GetNode<Control>("Dialogue");
	}

	public async Task Dialogue(Json json)
	{
		GetGame(this).Player.LockInput = true;
		await ToSignal(GetTree().CreateTimer(0.1),Timer.SignalName.Timeout);
		var data = (Dictionary)json.Data;
		var content = (Array)data["content"];
		_dialogue.Show();
		foreach (var info in content)
		{
			switch (info.VariantType)
			{
				case Variant.Type.String:
				{
					var text = (string)info;
					if (text.Find("action") == 0) DialogueAction(text);
					else
					{
						_text.ParseBbcode(text);
						var i = 0;
						do
						{
							_text.ScrollToLine(i);
							i++;
							await ToSignal(this, SignalName.InputAction);
						} while (i < _text.GetLineCount() - 2);
					}

					break;
				}
				case Variant.Type.Dictionary:
					break;
			}
		}
		_dialogue.Hide();
		await ToSignal(GetTree().CreateTimer(0.1),Timer.SignalName.Timeout);
		GetTree().Paused = false;
		GetGame(this).Player.LockInput = false;
	}

	private void DialogueAction(string text)
	{
		
	}

	public void PopupInfo(string text = "")
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GetGame(this).Player.LockInput && Input.IsActionJustPressed("Action"))
			EmitSignal(SignalName.InputAction);
	}
}