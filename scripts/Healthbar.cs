using Godot;
using System;

/// <summary>
/// Visual representation of Enemy/Player remaining health.
/// </summary>
public partial class Healthbar : ProgressBar
{
	private ProgressBar progressBar;
	/// <summary>
	/// This timer controls the delay between the "background" Progressbar updating to match
	/// the value of the "main" healthbar, creating a "damage" animation on the healthbar.
	/// 
	/// Time is configured in the engine editor.
	/// </summary>
	private Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (progressBar is null)
			progressBar = GetNode<ProgressBar>("./ProgressBar");

		timer = GetNode<Timer>("./Timer");
	}
	/// <summary>
	/// Initializes values for the healthbar
	/// </summary>
	/// <param name="hp">Health points to start with</param>
    public void Setup(int hp)
	{
		MaxValue = hp;
		Value = hp;

		if (progressBar is null)
			progressBar = GetNode<ProgressBar>("./ProgressBar");

		progressBar.MaxValue = hp;
		progressBar.Value = hp;
	}
	/// <summary>
	/// Signal handler for the internal timer that causes the 'delay' effect when
	/// taking damage.
	/// 
	/// Does not interact with the GameData signal bus because this is internal to this
	/// Node.
	/// </summary>
	public void OnHealthbarTimerTimeout()
	{
		progressBar.Value = Value;
	}
	/// <summary>
	/// Sets value of ProgressBar 
	/// </summary>
	/// <param name="hp">HP to set Healthbar to</param>
	/// <param name="startTimer">True if taking damage</param>
	public void SetHealth(int hp, bool startTimer = false)
	{
		Value = hp;
		if (startTimer)
		    timer.Start();
		else progressBar.Value = hp;
	}
}
