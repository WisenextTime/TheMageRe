using System.Collections.Generic;

namespace TheMage.Scripts;

public partial class Game : Node2D
{
	//Attributes
	//public int PlayerLevel = 1;
	public int Coins = 0;
	//Buffs
	
	public Player Player;
	public override void _Ready()
	{
		Player = GetNode<Player>("GameObjects/Player");
	}
}