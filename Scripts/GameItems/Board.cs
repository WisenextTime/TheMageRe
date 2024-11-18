using System;

namespace TheMage.Scripts.GameItems;

public partial class Board : ItemObject
{
	[Export] public int Theme;
	[Export] public string Text = string.Empty;
	protected Label _text;

	public override void _Ready()
	{
		GameObjectReady();
		BaseReady();
		GetNode<Sprite2D>("Image").Frame = Theme;
		_text = GetNode<Label>("Info/Text");
		_text.Text = Text;
	}

	public override void OnSelect()
	{
		_text.Show();
	}

	public override void OnUnselect()
	{
		_text.Hide();
	}
}