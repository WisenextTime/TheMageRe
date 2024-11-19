using System.Threading.Tasks;

namespace TheMage.Scripts.Enemies;

public partial class ZeroSlime : Enemy
{
	protected Area2D _special;
	public override void _Ready()
	{
		EnemyReady();
		Elements.Add(new ElementData
		{
			Element = "Zero",
			Def = 600
		});
		Elements.Add(new ElementData
		{
			Element = "Physics",
			Def = -300
		});
		_special = GetNode<Area2D>("Special");
	}

	protected override async Task UseSpecialPower()
	{
		alert = true;
		var newTarget = FindTarget();
		if (target != null)
		{
			_canMove = false;
			_special.GlobalPosition = target.GlobalPosition;
			_special.Show();
			await ToSignal(GetTree().CreateTimer(2),Timer.SignalName.Timeout);
			var tween = GetTree().CreateTween();
			_special.Hide();
			tween.TweenProperty(this, new NodePath(Node2D.PropertyName.GlobalPosition), GlobalPosition, 0);
			tween.TweenProperty(this, new NodePath(Node2D.PropertyName.GlobalPosition), _special.GlobalPosition, 0.1);
			tween.TweenCallback(Callable.From(SpecialAttack));
		}
	}

	protected void SpecialAttack()
	{
		foreach (var body in _special.GetOverlappingBodies())
		{
			if (body is not Targetable target) continue;
			if (target.Team == Team) continue;
			Damage(target);
		}
		_canMove = true;
	}

	private void Damage(Targetable targetable)
	{
		var data = new DamageDate();
		foreach (var element in Element)
		{
			var attackData = Elements.Find(x => x.Element == element);
			var weaponData = MainWeapon.ItemSource.Elements.Find(x => x.Element == element);
			var damage = (attackData.Atk + weaponData.Atk) * (1 + attackData.AtkMul + weaponData.AtkMul) * 2;
			data.Elements.Add(element, damage);
		}
		targetable.TakeDamage(data);
	}
}