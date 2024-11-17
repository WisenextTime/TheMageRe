namespace TheMage.Scripts.Enemies;

public partial class ZeroSlime : Enemy
{
	public override void _Ready()
	{
		EnemyReady();
		Elements.Add(new ElementData
		{
			Element = "Zero",
			Def = 600
		});
	}
}