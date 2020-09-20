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

            Assert.That(lazy, Is.TypeOf<Lazy<int>>());
        }

        [Test]
        public void CreateThreadSafeLazy()
        {
            var threadSafeLazy = LazyFactory.CreateThreadSafeLazy(() => new int());

            Assert.That(threadSafeLazy, Is.TypeOf<ThreadSafeLazy<int>>());
        }

        [Test]
        public void CreateNotSingletonLazy()
        {
            var firstLazy = LazyFactory.CreateLazy(() => new int());
            var secondLazy = LazyFactory.CreateLazy(() => new int());
            
            Assert.That(firstLazy, Is.Not.EqualTo(secondLazy));
        }

        [Test]
        public void CreateNotSingletonThreadSafeLazy()
        {
            var firstLazy = LazyFactory.CreateThreadSafeLazy(() => new int());
            var secondLazy = LazyFactory.CreateThreadSafeLazy(() => new int());
            
            Assert.That(firstLazy, Is.Not.EqualTo(secondLazy));
        }
    }
}