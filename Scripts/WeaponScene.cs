using System;
using System.Threading.Tasks;
using TheMage.Scripts.ItemServices;

namespace TheMage.Scripts;

public partial class WeaponScene : Node
{
	[Export] public PackedScene BulletScene;
	[Export] public float DelayTime = 1f;
	[Export] public float WarmupTime;
	public Weapon Data;
	public Item ItemSource => GetGlobal().Items[Data.ItemSource];
	
	[Export] public string DataSource;
	protected Bullet _bullet => BulletScene.Instantiate<Bullet>();

	public bool CanAttack = true;
	protected Timer _delayTimer;

	protected void BaseReady()
	{
		Data = GetGlobal().Weapons[DataSource];
		_delayTimer = GetNode<Timer>("Delay");
	}

	public override void _Ready()
	{
		BaseReady();
	}

	public virtual async Task Attack(Targetable source, Targetable target = null)
	{
		if (!CanAttack) return;
		CanAttack = false;
		_delayTimer.WaitTime =
			float.Max(DelayTime * 1f - ItemSource.BaseAttributes.AtkSpd + source.Attributes.AtkSpd, 0.0001f);
		_delayTimer.Start();
		await ToSignal(GetTree().CreateTimer(WarmupTime), Timer.SignalName.Timeout);
		var bullet = _bullet;
		bullet.Attacker = source;
		bullet.Source = Data;
		bullet.GlobalPosition = source.GlobalPosition;
		bullet.GlobalRotation = source.GetDir();
		bullet.Target = target;
		GetGame(source).AddChild(bullet);
	}

	protected virtual void DirectAttack(Targetable source, Targetable target)
	{
		if (!CanAttack) return;
		CanAttack = false;
		_delayTimer.WaitTime =
			float.Max(DelayTime * 1f - ItemSource.BaseAttributes.AtkSpd + source.Attributes.AtkSpd, 0.0001f);
		_delayTimer.Start();
		var data = new DamageDate();
		foreach (var element in Element)
		{
			var attackData = source.Elements.Find(x => x.Element == element);
			var weaponData = ItemSource.Elements.Find(x => x.Element == element);
			var damage = (attackData.Atk + weaponData.Atk) * (1 + attackData.AtkMul + weaponData.AtkMul);
			var crit = source.Attributes.CritCnc + ItemSource.BaseAttributes.CritCnc <
			           Random.Shared.NextSingle();
			if (crit)
				damage *= 1 + source.Attributes.CritDmg + ItemSource.BaseAttributes.CritDmg;
			data.Elements.Add(element, damage);
		}
		target.TakeDamage(data);
	}
	
	public void DelayReady() => CanAttack = true;
}

public class Weapon
{
	public string ItemSource = string.Empty;
	public bool Hidden = false;
}