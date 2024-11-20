using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheMage.Scripts;

public partial class Npc : ItemObject
{
	[Export] public bool RandomMove;
	[Export] public int RandomMoveRange = 5;
	[Export] public int RandomMoveSpeed = 1;
	[Export(PropertyHint.File, "*.json")] public string SpeakContent;
	[Export] public string NpcName = "";
	[Export(PropertyHint.Expression)] public string DangerInfo;

	public bool Idle = true;
	public bool Safe = true;
	
	protected Area2D _safeArea;
	protected RichTextLabel _danger;

	public Json SpeakContentFile => ResourceLoader.Load<Json>(SpeakContent);

	protected void NpcBaseReady()
	{
		GameObjectReady();
		BaseReady();
		_safeArea = GetNode<Area2D>("SafeArea");
		_danger = GetNode<RichTextLabel>("Danger");
		_danger.ParseBbcode(DangerInfo);
	}

	public override void _Ready()
	{
		NpcBaseReady();
	}

	public override void _PhysicsProcess(double delta)
	{
		TestSafe();
	}

	protected void TestSafe()
	{
		Safe = !_safeArea.GetOverlappingBodies()
			.Where(body => body is Targetable target && target.Team != 0)
			.Select(body => body).Any();
	}

	public override void OnUse()
	{
		if (!Idle) return;
		if (!Safe) PopupDanger();
		else _ = Speak(SpeakContentFile);
	}

	protected void PopupDanger()
	{
		var _dangerTween = GetTree().CreateTween();
		_dangerTween.TweenCallback(Callable.From(_danger.Show));
		_dangerTween.TweenProperty(_danger, new NodePath(CanvasItem.PropertyName.Modulate),new Color(1,1,1), 0);
		_dangerTween.TweenProperty(_danger, new NodePath(CanvasItem.PropertyName.Modulate),new Color(1,1,1), 1);
		_dangerTween.TweenProperty(_danger, new NodePath(CanvasItem.PropertyName.Modulate),new Color(1,1,1,0), 2);
		_dangerTween.TweenCallback(Callable.From(_danger.Hide));
	}

	protected async Task Speak(Json json)
	{
		Idle = false;
		await GetGame(this).GetNode<Ui>("Ui").Dialogue(json);
		Idle = true;
	}
}