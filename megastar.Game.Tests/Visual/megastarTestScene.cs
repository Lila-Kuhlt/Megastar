using osu.Framework.Testing;

namespace megastar.Game.Tests.Visual
{
    public abstract partial class megastarTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new megastarTestSceneTestRunner();

        private partial class megastarTestSceneTestRunner : MegastarGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
