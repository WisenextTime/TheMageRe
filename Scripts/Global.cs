using System;
using System.Collections.Generic;
using TheMage.Scripts.ItemServices;

namespace TheMage.Scripts;

public class Global
{
	public Dictionary<string,Item> Items = [];
	public Dictionary<string,Weapon> Weapons = [];
	private static Global _global;
	public Item MissingItem => Items["Missing"];

	public static Global GetGlobal() => _global??= new Global();

	public Global()
	{
		foreach (var file in DirAccess.GetFilesAt("res://Assets/Data/Items/"))
			Items.Add(file.GetBaseName(), ItemLoader.LoadItemFromFile($"res://Assets/Data/Items/{file}"));
		foreach (var file in DirAccess.GetFilesAt("res://Assets/Data/Weapons/"))
			Weapons.Add(file.GetBaseName(), WeaponLoader.LoadWeaponFromFile($"res://Assets/Data/Weapons/{file}"));
		
	}

	public static readonly string[] EquipmentDataMisc = 
	[
		"MaxHp","HpMul","HpReg","HpRegMul",
		"MaxMp","MpMul","MpReg","MpRegMul",
		"MovSpd","AtkSpd",
		"Crit","CritDmg"
	];

	public static readonly string[] Element =
	[
		"Physical","Air","Fire","Water","Earth","Aether"
	];
	
	public struct BaseAttributes
	{
		public float MovSpd = 0;
		public float AtkSpd = 0;
	
		public float CritCnc = 0;
		public float CritDmg = 0;

		public float MaxHp = 0;
		public float MaxHpMul = 0;
		public float HpReg = 0;

		public float MaxMp = 0;
		public float MaxMpMul = 0;
		public float MpReg = 0;
	
		public BaseAttributes() { }
	}
	
	public struct ElementData
	{
		public string Element = "";
		public float Atk = 0;
		public float AtkMul = 0;
		public float Def = 0;
		public float DefMul = 0;

		public ElementData() { }
	}

	public struct DamageDate
	{
		public Dictionary<string, float> Elements = new();

		public DamageDate() { }
	}
	public static Game GetGame(Node node)
	{
		var root = node.GetTree().GetCurrentScene();
		if(root is Game game) return game;
		throw new Exception("Unable to get Game. Check if node is in game.");
	}
}