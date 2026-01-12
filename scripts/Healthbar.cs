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
	/// Adds points back to this ProgressBar
	/// </summary>
	/// <param name="hp">Health points to add back</param>
	public void Heal(int hp)
	{
		if (hp <= 0)
			throw new ArgumentOutOfRangeException($"{hp} must be positive");

		Value += hp;
	}
	/// <summary>
	/// Removes points from this ProgressBar
	/// </summary>
	/// <param name="hp">Health points to remove</param>
	public void Damage(int hp)
	{
		if (hp >= 0)
			throw new ArgumentOutOfRangeException($"{hp} msut be negative");
		
		Value -= hp;
		timer.Start();
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
}
