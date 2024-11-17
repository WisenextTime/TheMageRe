using System;

namespace TheMage.Scripts.Weapons;

public partial class ZeroSlimeWeapon : WeaponScene
{
	public override void Attack(Targetable source, Targetable target = null)
	{
		if (target == null) return;
		DirectAttack(source, target);
	}
}