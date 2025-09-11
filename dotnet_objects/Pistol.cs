using System;

public class Pistol : Weapon
{

    public Pistol()
    {
        BulletType = Bullet.Standard;
    }

    public override void Reload()
    {
        throw new NotImplementedException();
    }

    public override void Shoot()
    {
        throw new NotImplementedException();
    }
}