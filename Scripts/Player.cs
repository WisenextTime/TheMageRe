namespace TheMage.Scripts;

public partial class Player : Targetable
{
	public Vector2 MoveDirection = Vector2.Right;
	
	//Basic attributes
	public float BaseMoveSpeed = 80f;
	
	//Addition attributes
	public float AdditionMoveSpeed = 0f;
	
	//Nodes
	private Sprite2D _image;
	private AnimationPlayer _animation;
	private RayCast2D _itemFinder;

	public GameObject NowItem;
	
	public override void _Ready()
	{
		GameObjectReady();
		TargetableReady();
		_image = GetNode<Sprite2D>("Image");
		_animation = GetNode<AnimationPlayer>("Animation");
		_itemFinder = GetNode<RayCast2D>("ItemFinder");
	}

	public override void _PhysicsProcess(double delta)
	{
		Move();
		if (Input.IsActionPressed("Attack") && !Input.IsActionPressed("MagicLeft") &&
		    !Input.IsActionPressed("MagicRight"))
			_ = MainWeapon.Attack(this);
		if (_itemFinder.GetCollider() is GameObject newItem)
		{
			if (newItem == NowItem) return;
			NowItem.OnUnselect();
			NowItem = newItem;
			newItem.OnSelect();
		}
		else
		{
			if (NowItem == null) return;
			NowItem.OnUnselect();
			NowItem = null;
		}
	}

	private void Move()
	{
		var moveDir = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown").Normalized();
		if (moveDir != Vector2.Zero)
		{
			_itemFinder.LookAt(moveDir);
			MoveDirection = moveDir;
			_image.FlipH = moveDir.X switch
			{
				< 0 => true,
				> 0 => false,
				_ => _image.FlipH
			};
			Velocity = moveDir * BaseMoveSpeed * (1 + AdditionMoveSpeed);
			if (Input.IsActionPressed("Run"))
			{
				_animation.Play("Run");
				Velocity *= 2;
			}
			else
			{
				_animation.Play("Move");
			}
		}
		else
		{
			Velocity = Vector2.Zero;
			_animation.Play("Idle");
		}
		MoveAndSlide();
	}

	public override float GetDir()
	{
		return MoveDirection.Angle();
	}
}