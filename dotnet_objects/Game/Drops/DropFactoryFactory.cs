using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewGameProject.dotnet_objects.Game.Drops;

namespace Game.Drops
{
    public static class DropFactoryFactory
    {

        private class WeaponDropFactory : AbstractDropFactory
        {
            protected override IDrop MakeDrop(int type)
            {
                try
                {
                    DropType.Weapon weaponDropType = (DropType.Weapon)type;  
                } catch (InvalidCastException)
                {
                    throw new ArgumentException("Invalid value for WeaponDropFactory");
                }
                throw new NotImplementedException("MakeDrop method not implemented yet");
            }
        }

        private class StatusModifierDropFactory : AbstractDropFactory
        {
            protected override IDrop MakeDrop(int type)
            {
                try
                {
                    DropType.StatusModifier statusModifierDropType = (DropType.StatusModifier)type;  
                } catch (InvalidCastException)
                {
                    throw new ArgumentException("Invalid value for StatusModifierDropFactory");
                }
                throw new NotImplementedException("MakeDrop method not implemented yet");
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

        public static AbstractDropFactory GetFactoryChance(List<float> chances)
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

            throw new NotImplementedException("GetFactoryChance method not implemented yet");
        }
    }
}