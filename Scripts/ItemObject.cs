namespace TheMage.Scripts;

public partial class ItemObject : GameObject
{
	[Export] public bool Triggerable;
	[Export] public bool Usable;

	protected Node2D _info;

	public override void _Ready()
	{
		GameObjectReady();
		BaseReady();
	}

	protected void BaseItemSelect()
	{
		if (!Triggerable) return;
		_info.Show();
		SetIcon();
	}

	private void SetIcon()
	{
		var infoKey = GetNode<Sprite2D>("Info/Key");
		var infoJoy = GetNode<Sprite2D>("Info/Joy");
		var infoTouch = GetNode<Sprite2D>("Info/Touch");
		switch (GetGlobal().GetInputDevice())
		{
			case InputDevice.KeyboardAndMouse:
				infoKey.Show();
				infoJoy.Hide();
				infoTouch.Hide();
				break;
			case InputDevice.Joystick:
				infoKey.Hide();
				infoJoy.Show();
				infoTouch.Hide();
				break;
			default:
				infoKey.Hide();
				infoJoy.Hide();
				infoTouch.Show();
				break;
		}
	}

	protected void BaseUnSelect()
	{
		_info.Hide();
	}

	protected void BaseReady()
	{
		_info = GetNode<Node2D>("Info");
	}

	public override void OnSelect()
	{
		BaseItemSelect();
	}

	public override void OnUnselect()
	{
		BaseUnSelect();
	}
}