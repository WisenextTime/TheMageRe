using System;
using System.Linq;
using TheMage.Scripts.ExceptionHanding.Exceptions;
using Environment = Godot.Environment;

namespace TheMage.Scripts.ItemServices;

public static class ItemLoader
{
	public static Item LoadItemFromFile(string dataSource)
	{
		try
		{
			var data = new ConfigFile();
			var newItem = new Item();
			data.Load(dataSource);
			if (!data.HasSection("Info")) throw new ConfigErrorSectionException(dataSource, "Info");
			if (!data.HasSectionKey("Info", "Name")) throw new ConfigErrorKeyException(dataSource, "Name", "Info");
			newItem.Name = (string)data.GetValue("Info", "Name");
			newItem.Description = (string)data.GetValue("Info", "Description", "");
			newItem.Equippable = (bool)data.GetValue("Info", "Equippable", false);
			newItem.Hidden = (bool)data.GetValue("Info", "Hidden", false);
			if (!newItem.Equippable || !data.HasSection("EquipmentData")) return newItem;
			if (data.HasSection("BaseAttributes"))
				newItem.BaseAttributes = new BaseAttributes
				{
					MaxMp = (float)data.GetValue("Base", "MaxMp", 0),
					MaxHp = (float)data.GetValue("Base", "MaxHp", 0),
					MpReg = (float)data.GetValue("Base", "MpReg", 0),
					HpReg = (float)data.GetValue("Base", "HpReg", 0),
					MaxHpMul = (float)data.GetValue("Base", "MaxHpMul", 0),
					MaxMpMul = (float)data.GetValue("Base", "MaxMpMul", 0),
					MovSpd = (float)data.GetValue("Base", "MovSpd", 0),
					AtkSpd = (float)data.GetValue("Base", "AtkSpd", 0),
					CritDmg = (float)data.GetValue("Base", "CritDmg", 0),
					CritCnc = (float)data.GetValue("Base", "CritCnc", 0),
				};
			foreach (var element in Element)
			{
				if (!data.HasSection("ElementData")) continue;
				newItem.Elements.Add(new ElementData
				{
					Element = element,
					Atk = (float)data.GetValue(element, "Atk", 0f),
					AtkMul = (float)data.GetValue(element, "AtkMul", 0f),
					Def = (float)data.GetValue(element, "Def", 0f),
					DefMul = (float)data.GetValue(element, "DefMul", 0f),
				});
			}
			return newItem;
		}
		catch (Exception e)
		{
			GD.Print(e);
			return GetGlobal().MissingItem;
		}
	}
}

public static class WeaponLoader
{
	public static Weapon LoadWeaponFromFile(string dataSource)
	{
		try
		{
			var data = new ConfigFile();
			data.Load(dataSource);
			var newWeapon = new Weapon();
			if (!data.HasSection("Info")) throw new ConfigErrorSectionException(dataSource, "Info");
			if (!data.HasSectionKey("Info", "Source")) throw new ConfigErrorKeyException(dataSource, "Source", "Info");
			newWeapon.ItemSource = (string)data.GetValue("Info", "Source");
			newWeapon.Hidden = (bool)data.GetValue("Info", "Hidden", false);
			return newWeapon;
		}
		catch (Exception e)
		{
			GD.Print(e);
			return null;
		}
	}
}