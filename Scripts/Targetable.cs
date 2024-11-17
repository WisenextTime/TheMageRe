using System.Collections.Generic;
using System.Linq;

namespace TheMage.Scripts;

[GlobalClass]
public abstract partial class Targetable : GameObject
{

	[Export] public WeaponScene MainWeapon;
	[Export] public int MaxHealth;
	public int Health;
	[Export] public int MaxMana;
	public int Mana;

	public List<Item> Equipments = [];

	public Dictionary<Item, int> Buffs = [];

	public List<Magic> Magics = [];

	public Dictionary<string, int> Items = [];

	[Export(PropertyHint.Range, "0,3")] public int Team = 0;

	public BaseAttributes Attributes = new();
	public List<ElementData> Elements = [];
	protected void TargetableReady()
	{
		//CollisionLayer |= 0b_0000_1;
	}

	protected (int,bool) BaseTakeDamage(DamageDate data)
	{
		var damage = (from element in Element
        			let source = Elements.Find(x => x.Element == element)
        			let defense = source.Def * (1 + source.DefMul)
        			let attack = data.Elements[element] 
        			select attack * defense / (defense + 600)
        			into finalDamage 
        			select (int)finalDamage).Sum();
        		Health -= damage;
		        return (damage,data.Crit);
	}

	public virtual void TakeDamage(DamageDate data)
	{
		BaseTakeDamage(data);
	}

	public virtual float GetDir()
	{
		return GetGlobalRotation();
	}
}