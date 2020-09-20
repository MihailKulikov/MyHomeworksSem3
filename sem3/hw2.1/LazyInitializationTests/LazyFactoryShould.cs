using LazyInitialization;
using NUnit.Framework;

namespace LazyInitializationTests
{
    public class LazyFactoryShould
    {
        [Test]
        public void Throw_ArgumentNullException_When_Creating_Lazy_With_Null_Supplier()
        {
            Assert.That(() => LazyFactory.CreateLazy<object>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Throw_ArgumentNullException_When_Creating_ThreadSafeLazy_With_Null_Supplier()
        {
            Assert.That(() => LazyFactory.CreateThreadSafeLazy<object>(null), Throws.ArgumentNullException);
        }
        
        [Test]
        public void Create_Lazy()
        {
            var lazy = LazyFactory.CreateLazy(() => new int());

            Assert.That(lazy, Is.TypeOf<Lazy<int>>());
        }

        [Test]
        public void Create_ThreadSafeLazy()
        {
            var threadSafeLazy = LazyFactory.CreateThreadSafeLazy(() => new int());

            Assert.That(threadSafeLazy, Is.TypeOf<ThreadSafeLazy<int>>());
        }

        [Test]
        public void Create_Not_Singleton_Lazy()
        {
            var firstLazy = LazyFactory.CreateLazy(() => new int());
            var secondLazy = LazyFactory.CreateLazy(() => new int());
            
            Assert.That(firstLazy, Is.Not.EqualTo(secondLazy));
            Assert.That(firstLazy, Is.TypeOf<Lazy<int>>());
            Assert.That(secondLazy, Is.TypeOf<Lazy<int>>());
        }

        [Test]
        public void Create_Not_Singleton_ThreadSafeLazy()
        {
            var firstLazy = LazyFactory.CreateThreadSafeLazy(() => new int());
            var secondLazy = LazyFactory.CreateThreadSafeLazy(() => new int());
            
            Assert.That(firstLazy, Is.Not.EqualTo(secondLazy));
            Assert.That(firstLazy, Is.TypeOf<ThreadSafeLazy<int>>());
            Assert.That(secondLazy, Is.TypeOf<ThreadSafeLazy<int>>());
        }
    }
}