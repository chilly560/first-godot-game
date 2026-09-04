namespace GdUnit4.Tests 
{
    using static GdUnit4.Assertions;

    using Game.Drops;
    using Game.Enemies;
    using Game.StatusModifier;
    using Game.Weapons;

    [TestSuite]
    public class DotnetObjectsTest 
    {
        [TestCase]
        public void TestHelloWorld() {
            AssertThat("Hello World").Equals("Hello World");
        }

        [TestCase]
        public void DropFactoryFactoryConstruct()
        {
            AbstractDropFactory weaponFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Weapon)
;
            AssertThat(weaponFactory).IsNotNull();
            AssertThat(weaponFactory.GetFactoryType()).IsEqual(AbstractDropFactory.WEAPON);

            AbstractDropFactory statusFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.StatusModifier);
            AssertThat(statusFactory).IsNotNull();
            AssertThat(statusFactory.GetFactoryType()).IsEqual(AbstractDropFactory.STATUS);

            AbstractDropFactory nullFactory = DropFactoryFactory.GetFactory(DropFactoryFactoryType.Null);
            AssertThat(nullFactory).IsNotNull();
        }
    }
}