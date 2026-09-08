namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;
    using Game;
    using Game.StatusModifier;

    [TestSuite]
    public class HealthModifierTest
    {
        [TestCase]
        public void GetHealAmount_ReturnsAmountPassedToConstructor()
        {
            HealthModifier modifier = new HealthModifier(15);
            AssertThat(modifier.GetHealAmount()).IsEqual(15);
        }

        [TestCase]
        public void GetHealAmount_ZeroAmount_ReturnsZero()
        {
            HealthModifier modifier = new HealthModifier(0);
            AssertThat(modifier.GetHealAmount()).IsEqual(0);
        }

        [TestCase]
        public void GetHealAmount_NegativeAmount_ReturnsNegativeValue()
        {
            HealthModifier modifier = new HealthModifier(-10);
            AssertThat(modifier.GetHealAmount()).IsEqual(-10);
        }

        [TestCase]
        public void SetParent_NullParent_DoesNotThrow()
        {
            try
            {
                HealthModifier modifier = new HealthModifier(5);
                modifier.SetParent(null);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void SetParent_ValidParent_DoesNotThrow()
        {
            Node2D parent = null;
            try
            {
                HealthModifier modifier = new HealthModifier(5);
                parent = new Node2D();
                modifier.SetParent(parent);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                parent?.Free();
            }
        }

        [TestCase]
        public void GetHealAmount_DoesNotChangeAfterSetParent()
        {
            HealthModifier modifier = new HealthModifier(20);
            modifier.SetParent(null);
            AssertThat(modifier.GetHealAmount()).IsEqual(20);
        }
    }
}
