using System;
using System.Threading.Tasks;

namespace TheMage.Scripts.Weapons;

public partial class ZeroSlimeWeapon : WeaponScene
{
	public override async Task Attack(Targetable source, Targetable target = null)
	{
		if (!CanAttack) return;
		await ToSignal(GetTree().CreateTimer(WarmupTime), Timer.SignalName.Timeout);
		CanAttack = false;
		_delayTimer.WaitTime =
			float.Max(DelayTime * 1f - ItemSource.BaseAttributes.AtkSpd + source.Attributes.AtkSpd, 0.0001f);
		_delayTimer.Start();
		DirectAttack(source, target);
	}
}