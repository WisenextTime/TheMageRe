namespace TheMage.Scripts;

public partial class ItemObject : GameObject
{
	[Export] public bool Triggerable;
	[Export] public bool Usable;

	protected Node2D _info;
	protected Sprite2D infoKey => GetNode<Sprite2D>("Info/Key");
	protected Sprite2D infoJoy => GetNode<Sprite2D>("Info/Joy");
	protected Sprite2D infoTouch => GetNode<Sprite2D>("Info/Touch");

	public override void _Ready()
	{
		GameObjectReady();
		BaseReady();
	}

	protected void BaseItemSelect()
	{
		if (!Triggerable) return;
		SetIcon();
	}

	private void SetIcon()
	{
		
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
		infoKey.Hide();
		infoJoy.Hide();
		infoTouch.Hide();
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