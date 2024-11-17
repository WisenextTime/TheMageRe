using System.Collections.Generic;
using System.Diagnostics;
using TheMage.Scripts.ItemServices;

namespace TheMage.Scripts;

public partial class ItemScene : Node
{
	[Export] public string DataSource; 
	public Item Data;

	protected void BaseReady()
	{
		Data = GetGlobal().Items[DataSource];
	}

	public override void _Ready()
	{
		BaseReady();
	}
	
	public virtual void OnAttack(Player player) { }
	public virtual void OnUse(Player player) { }
	public virtual void OnMove(Player player) { }
	public virtual void OnPerSec(Player player) { }
	public virtual void OnPerFrame(Player player) { }
}

public class Item
{
	public string Name = null;
	public string Description = string.Empty;
	public bool Equippable = false;
	public string Weapon = string.Empty;
	public BaseAttributes BaseAttributes = new ();
	public List<ElementData> Elements = [];
	public bool Hidden = false;
}