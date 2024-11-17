using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TheMage.Scripts;

public partial class Bullet : Area2D
{
	[Export] public float Speed = 4f;
	[Export] public float LifeTime = 10f;
	[Export] public float FindRadius = 320f;
	[Export] public bool TargetGround;
	[Export] public bool IgnoreWall;
	[Export] public bool MultiTarget;
	[Export] public float Acceleration;
	[Export] public float MaxSpeed;
	
	[ExportGroup("AttackMode")]
	[Export] public bool DirectAttack;
	[Export] public bool RangeAttack;
	[Export] public int RangeRadius;
	
	protected Area2D ExplodeArea;
	protected Area2D FindTargetArea;
	
	public Targetable Target;
	public Weapon Source;
	public Targetable Attacker;
	
	public override void _Ready()
	{
		Init();
	}
	public override void _PhysicsProcess(double delta)
	{
		FindTarget();
		GlobalPosition += Speed * Vector2.FromAngle(GlobalRotation) * (float)delta * 120f;
	}
	protected void Init()
	{
		ExplodeArea = GetNode<Area2D>("ExplodeRange");
		FindTargetArea = GetNode<Area2D>("FindRange");
		GlobalPosition += Speed * Vector2.FromAngle(GlobalRotation);
	} 
	protected virtual void FindTarget()
	{
		if(TargetGround || Target == null) return;
		LookAt(Target.GetGlobalPosition());
	}

	public virtual void LifeTimeEnd()
	{
		Explode();
	}

	protected virtual void Explode()
	{
		if (RangeAttack)
		{
			
		}
		QueueFree();
	}

	protected virtual void Damage(Targetable target, float multiplier = 1)
	{
		var data = new DamageDate();
		foreach (var element in Element)
		{
			var attackData = Attacker.Elements.Find(x => x.Element == element);
			var weaponData = GetGlobal().Items[Source.ItemSource].Elements.Find(x => x.Element == element);
			var damage = (attackData.Atk + weaponData.Atk) * (1 + attackData.AtkMul + weaponData.AtkMul) * multiplier;
			var crit = Attacker.Attributes.CritCnc + GetGlobal().Items[Source.ItemSource].BaseAttributes.CritCnc <
			           Random.Shared.NextSingle();
			if (crit)
				damage *= 1 + Attacker.Attributes.CritDmg + GetGlobal().Items[Source.ItemSource].BaseAttributes.CritDmg;
			data.Elements.Add(element, damage);
		}
		target.TakeDamage(data);
	}

	public virtual void Hit(Node body)
	{
		if (body is not Targetable target)
		{
			Explode();
			return;
		}
		if(target.Team == Attacker.Team)return;
		Explode();
		Damage(target);
	}
}