
using Godot;

public partial class EnemyBullet : Bullet 
{
    public override void _Ready()
    {
        base._Ready();
    }
    public override void OnAreaEnteredBullet(Node body)
    {
        if (body is Player p)
        {
            p.TakeDamage(Damage);
            QueueFree();
        }
    }
}
