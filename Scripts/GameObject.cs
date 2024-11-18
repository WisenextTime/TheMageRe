using Godot.Collections;

namespace TheMage.Scripts;

[GlobalClass]
public abstract partial class GameObject : CharacterBody2D
{
	[Export] public bool CollisionWall = true;
	[Export] public bool CollisionLand = false;
	[Export] public bool CollisionWater = false;
	[Export] public bool CollisionAir = false;

	protected void GameObjectReady() { }
	
	public virtual void OnSelect() { }
	public virtual void OnUnselect() { }
	public virtual void OnUse() { }
}