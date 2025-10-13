using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.XPath;
using NewGameProject.dotnet_objects.Game.Drops;

namespace Game.Drops
{
    public static class DropFactoryFactory
    {

        private class WeaponDropFactory : AbstractDropFactory
        {


            public override ICollectable MakeDrop(int type)
            {
                DropType.Weapon weaponDropType; ;

                try
                {
                    weaponDropType = (DropType.Weapon)type;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Invalid value for WeaponDropFactory");
                }

                switch (weaponDropType)
                {
                    case DropType.Weapon.Pistol:
                        throw new NotImplementedException("PistolDrop not implemented yet");
                    case DropType.Weapon.Shotgun:
                        throw new NotImplementedException("ShotgunDrop not implemented yet");
                    default:
                        throw new ArgumentException("Invalid value for DropType.Weapon in WeaponDropFactory");
                }
                throw new NotImplementedException("MakeDrop method not implemented yet");
            }

            public override string ToString()
            {
                return "DropFactory Type: WeaponDropFactory";
            }
        }

        private class StatusModifierDropFactory : AbstractDropFactory
        {
            public override ICollectable MakeDrop(int type)
            {
                try
                {
                    DropType.StatusModifier statusModifierDropType = (DropType.StatusModifier)type;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Invalid value for StatusModifierDropFactory");
                }
                throw new NotImplementedException("MakeDrop method not implemented yet");
            }

            public override string ToString()
            {
                return "DropFactory Type: StatusModifierDropFactory";
            }
        }

        public static AbstractDropFactory GetFactory(DropFactoryFactoryType type)
        {
            switch (type)
            {
                case DropFactoryFactoryType.Weapon:
                    return new WeaponDropFactory();
                case DropFactoryFactoryType.StatusModifier:
                    return new StatusModifierDropFactory();
                default:
                    throw new ArgumentException("Invalid DropFactoryFactoryType");
            }
        }

        /// <summary>
        /// Simulates a 'random drop' based on the enemyDropChance and chances provided (currently up to 2)
        /// </summary>
        /// <param name="enemyDropChance"></param>
        /// <param name="chances"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static AbstractDropFactory GetFactoryChance(float enemyDropChance, List<float> chances)
        {
            if (chances.Count != 2)
                throw new ArgumentException("chances must be of Length 2");
                
            Random rand = new Random();
            int baseLine = rand.Next(1, 11);
            if (baseLine * enemyDropChance > (1 - enemyDropChance))
            {
                float total = 0;
                foreach (float chance in chances)
                {
                    total += chance;
                }

                if (Math.Abs(total - 1.0f) > 0.01f)
                {
                    throw new ArgumentException("ERROR: Chance list must sum to 1");
                }

                // randomly generate factory based on chances provided
                baseLine = rand.Next(1, 101);
                int a = (int)chances[0];
                int b = (int)chances[1];
                if (baseLine <= a * 100)
                {
                    return GetFactory(DropFactoryFactoryType.Weapon);
                }
                else if (baseLine <= (a + b) * 100)
                {
                    return GetFactory(DropFactoryFactoryType.StatusModifier);
                }
            }

            throw new NotImplementedException("GetFactoryChance method not implemented yet");
        }
    }
}