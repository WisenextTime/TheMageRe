using System;
using System.Linq;
using System.Threading.Tasks;

namespace TheMage.Scripts;

public partial class Enemy : Targetable
{
	protected NavigationAgent2D _agent;
	protected Area2D _findTarget;
	protected AnimationPlayer _animation;
	protected Sprite2D _image;
	protected TextureProgressBar _hpBar;
	protected Timer _specialTimer;
	
	protected Label _damage;
	
	protected Vector2 _startPosition;
	protected bool _readyToSpecialPower;
	
	public bool alert;
	[Export] public float MaxFindTargetDistance = 320f;
	[Export] public float FindTargetDistance = 80f;
	[Export] public float AttackRange = 10f;
	[Export] public float Speed = 10f;
	[Export] public int Coin = 1;
	[Export] public int SpecialPower = 10;
	
	public Targetable target;
	protected bool _canMove = true;

	public void EnemyReady()
	{
		TargetableReady();
		if (Team == 0) Team = 1;
		_agent = GetNode<NavigationAgent2D>("Agent");
		_findTarget = GetNode<Area2D>("FindTarget");
		_startPosition = GlobalPosition;
		_animation = GetNode<AnimationPlayer>("Animation");
		_image = GetNode<Sprite2D>("Image");
		_hpBar = GetNode<TextureProgressBar>("HpBar");
		_damage = GetNode<Label>("Damage");
		_specialTimer = GetNode<Timer>("SpecialTimer");
		_specialTimer.WaitTime = SpecialPower;
		_specialTimer.Start();
		Attributes.MovSpd = Speed;
		Attributes.MaxHp = (float)(_hpBar.Value = _hpBar.MaxValue = Health = MaxHealth);
	}

	public override void _Ready()
	{
		EnemyReady();
	}

	public override void _PhysicsProcess(double delta)
	{
		BaseMovement();
		_hpBar.Value = Health;
	}

	public virtual void SpecialReady()
	{
		_readyToSpecialPower = true;
	}

	protected virtual async Task Special()
	{
		if(!_readyToSpecialPower) return;
		_readyToSpecialPower = false;
		await UseSpecialPower();
		_specialTimer.Start();
	}

	protected virtual Task UseSpecialPower()
	{
		return Task.CompletedTask;
	}

	protected void BaseMovement()
	{
		target ??= FindTarget();
		if (target!=null && target.GlobalPosition.DistanceTo(GlobalPosition) > (alert ? MaxFindTargetDistance : FindTargetDistance))
		{
			target = null;
			alert = false;
		}
		else
		{
			alert = target!=null;
		}
		if (!alert)
		{
			if (_startPosition.DistanceTo(GlobalPosition) <= 16)
			{
				_animation.Play(_animation.HasAnimation("Custom/Idle") ? "Custom/Idle" : "Idle");
				Velocity = Vector2.Zero;
				Health = int.Min(MaxHealth, Health + (int)Math.Ceiling(MaxHealth * 0.001));
			}
			else
			{
				_animation.Play(_animation.HasAnimation("Custom/Walk") ? "Custom/Walk" : "Walk");
				Move(_startPosition);
			}
		}
		else
		{
			if (_readyToSpecialPower)
			{
				_ = Special();
			}
			
			if (target!.GlobalPosition.DistanceTo(GlobalPosition) < AttackRange)
			{
				_animation.Play(_animation.HasAnimation("Custom/Attack") ? "Custom/Attack" : "Attack");
				Velocity = Vector2.Zero;
				Attack(target);
			}
			else
			{
				_animation.Play(_animation.HasAnimation("Custom/Walk") ? "Custom/Walk" : "Walk");
				Move(target.GlobalPosition);
			}
			
		}

		if (_canMove) MoveAndSlide();
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
		_ = MainWeapon.Attack(this,attackTarget);
	}

	protected virtual Targetable FindTarget()
	{
		return _findTarget.GetOverlappingBodies()
			.Where(body =>
				body is Targetable target && target.Team != Team &&
				target.GlobalPosition.DistanceTo(_startPosition) <= MaxFindTargetDistance * 2)
			.OrderBy(body => body.GlobalPosition.DistanceTo(GlobalPosition)).Select(body1 => body1 as Targetable)
			.FirstOrDefault();
	}
	
	public override void TakeDamage(DamageDate data)
	{
		var damageData = BaseTakeDamage(data);
		if (Health <= 0) Die();
		alert = true;
		target = FindTarget();
		var damageAnimation = (Label)_damage.Duplicate();
		damageAnimation.Text = damageData.Item1.ToString();
		damageAnimation.GlobalPosition = GlobalPosition;
		var startColor = damageData.Item2? new Color(0, 1, 1) : new Color(1, 1, 1);
		GetGame(this).GetNode<Node2D>("GameObjects").AddChild(damageAnimation);
		damageAnimation.Show();
		var tween = GetTree().CreateTween();
		tween.TweenProperty(damageAnimation, new NodePath(Control.PropertyName.GlobalPosition), GlobalPosition, 0);
		tween.Parallel().TweenProperty(damageAnimation, new NodePath(CanvasItem.PropertyName.Modulate), startColor, 0);
		tween.TweenProperty(damageAnimation, new NodePath(Control.PropertyName.GlobalPosition),
			GlobalPosition + Vector2.Up * 32, 1);
		tween.Parallel().TweenProperty(damageAnimation, new NodePath(CanvasItem.PropertyName.Modulate),
			new Color(startColor.R, startColor.G, startColor.B, 0), 1);
		tween.TweenCallback(Callable.From(damageAnimation.QueueFree));

	}

	protected void Die()
	{
		GetGame(this).Coins += Coin;
		QueueFree();
	}
}