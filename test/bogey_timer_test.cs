namespace GdUnit4.Tests
{
    using static GdUnit4.Assertions;

    using System;
    using Godot;

    [TestSuite]
    public class BogeyTimerTest
    {
        // Timer.Start() is a no-op (logged engine error, no exception) unless the timer is
        // inside the live SceneTree, so these tests attach the timer to the real tree.
        private static T CreateAndAttachTimer<T>() where T : Timer, new()
        {
            SceneTree tree = (SceneTree)Engine.GetMainLoop();
            T timer = new T();
            tree.Root.AddChild(timer);
            return timer;
        }

        private static void DetachAndFree(Node node)
        {
            node?.GetParent()?.RemoveChild(node);
            node?.Free();
        }

        // ----- BogeyDelayTimer -----

        [TestCase]
        [RequireGodotRuntime]
        public void StartTimer_SetsWaitTimeAndStartsTimer()
        {
            BogeyDelayTimer timer = null;
            try
            {
                timer = CreateAndAttachTimer<BogeyDelayTimer>();

                timer.StartTimer(2.5f);

                AssertThat(timer.WaitTime).IsEqual(2.5);
                AssertThat(timer.IsStopped()).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                DetachAndFree(timer);
            }
        }

        [TestCase]
        [RequireGodotRuntime]
        public void StartTimer_CalledAgain_ReplacesPreviousWaitTime()
        {
            BogeyDelayTimer timer = null;
            try
            {
                timer = CreateAndAttachTimer<BogeyDelayTimer>();

                timer.StartTimer(1.0f);
                timer.StartTimer(3.0f);

                AssertThat(timer.WaitTime).IsEqual(3.0);
                AssertThat(timer.IsStopped()).IsEqual(false);
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
                AssertThat(false).IsEqual(true);
            }
            finally
            {
                DetachAndFree(timer);
            }
        }

        // ----- BogeyTimer -----
        //
        // BogeyTimer declares only empty _Ready()/_Process() overrides and adds no members
        // of its own, so it has no behavior of its own to assert on. Bogey drives it purely
        // through the inherited Godot Timer API (WaitTime/Start), which is engine behavior
        // rather than project code. No test cases here on purpose.
    }
}
