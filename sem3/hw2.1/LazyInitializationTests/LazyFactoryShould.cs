using LazyInitialization;
using NUnit.Framework;

namespace LazyInitializationTests
{
    public class LazyFactoryShould
    {
        [Test]
        public void CreateLazy()
        {
            var lazy = LazyFactory.CreateLazy(() => new int());

            Assert.That(lazy, Is.TypeOf<LazyInitialization.Lazy<int>>());
        }

        [Test]
        public void CreateThreadSafeLazy()
        {
            var threadSafeLazy = LazyFactory.CreateThreadSafeLazy(() => new int());

            Assert.That(threadSafeLazy, Is.TypeOf<ThreadSafeLazy<int>>());
        }
    }
}