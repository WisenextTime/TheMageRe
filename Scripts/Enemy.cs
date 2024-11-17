using System.Linq;
namespace TheMage.Scripts;

public partial class Enemy : Targetable
{
	protected NavigationAgent2D _agent;
	protected Area2D _findTarget;
	protected AnimationPlayer _animation;
	protected Sprite2D _image;
	
	protected Vector2 _startPosition;
	
	public bool alert;
	[Export] public float MaxFindTargetDistance = 160f;
	[Export] public float FindTargetDistance = 80f;
	[Export] public float AttackRange = 10f;
	[Export] public float speed = 10f;
	
	public Targetable target;

	public void EnemyReady()
	{
		_agent = GetNode<NavigationAgent2D>("Agent");
		_findTarget = GetNode<Area2D>("FindTarget");
		_startPosition = GlobalPosition;
		_animation = GetNode<AnimationPlayer>("Animation");
		_image = GetNode<Sprite2D>("Image");
		Attributes.MovSpd = speed;
	}

	public override void _Ready()
	{
		EnemyReady();
	}

	public override void _PhysicsProcess(double delta)
	{
		BaseMovement();
	}

	protected void BaseMovement()
	{
		if (target!=null && target.GlobalPosition.DistanceTo(GlobalPosition) > (alert ? MaxFindTargetDistance : FindTargetDistance))
		{
			target = null;
			alert = false;
		}
		target ??= FindTarget();
		if (target == null)
		{
			if (_startPosition.DistanceTo(GlobalPosition) <= 16)
			{
				_animation.Play(_animation.HasAnimation("Custom/Idle") ? "Custom/Idle" : "Idle");
				Velocity = Vector2.Zero;
			}
			else
			{
				_animation.Play(_animation.HasAnimation("Custom/Walk") ? "Custom/Walk" : "Walk");
				Move(_startPosition);
			}
		}
		else
		{
			if (target.GlobalPosition.DistanceTo(GlobalPosition) < AttackRange)
			{
				_animation.Play(_animation.HasAnimation("Custom/Attack") ? "Custom/Attack" : "Attack");
				Attack(target);
			}
			else
			{
				_animation.Play(_animation.HasAnimation("Custom/Walk") ? "Custom/Walk" : "Walk");
				Move(target.GlobalPosition);
			}
		}
		MoveAndSlide();
	}

	protected virtual void Move(Vector2 position)
	{
		_agent.TargetPosition = position;
		var nextTargetDirection = ToLocal(_agent.GetNextPathPosition()).Normalized();
		Velocity = nextTargetDirection * Attributes.MovSpd;
		_image.FlipH = Velocity.X switch
		{
			< 0 => true,
			> 0 => false,
			_ => _image.FlipH
		};
	}

	protected virtual void Attack(Targetable attackTarget)
	{
		MainWeapon.Attack(this,attackTarget);
	}

	protected virtual Targetable FindTarget()
	{
		return _findTarget.GetOverlappingBodies().Count == 0
			? null
			: _findTarget.GetOverlappingBodies()
				.Where(body =>
					body is Targetable target && target.Team != Team &&
					target.GlobalPosition.DistanceTo(_startPosition) <= MaxFindTargetDistance * 2)
				.OrderBy(body => body.GlobalPosition.DistanceTo(GlobalPosition)).Select(body1 => body1 as Targetable)
				.FirstOrDefault();
	}
}