using System;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor;
using UnityEngine;

namespace Grygus.Utilities.Pool
{
    public class describe_Cache
    {
        public class given
        {
            public class the_sample_class_cache : SpecsFor<SampleClass>
            {
                protected override void Given()
                {
                    SUT = new SampleClass();
                    base.BeforeEachTest();
                    Cache<SampleClass>.DefaultPool.Clear();
                    var myCache = Cache<SampleClass>.Caches["MyCache"];
                    myCache.Clear();
                }

            }
        }

        public class when_generating_objects : given.the_sample_class_cache
        {

            protected override void When()
            {
                Cache<SampleClass>.Generate(2);
                Cache<SampleClass>.Caches["MyCache"].Generate(2);
            }

            [Test]
            public void then_default_pool_contains_objects()
            {

                Cache<SampleClass>.Count.ShouldEqual(2);
            }

            [Test]
            public void then_named_pool_contains_objects()
            {

                Cache<SampleClass>.Caches["MyCache"].Count.ShouldEqual(2);
            }
        }

        public class when_pulling_from_cache : given.the_sample_class_cache
        {
            protected override void When()
            {
                Cache<SampleClass>.Generate(3);
                var myCache = Cache<SampleClass>.DefaultPool;
                var defaultPoolInstance = myCache.Pop();
                var cachedInstance = Cache<SampleClass>.Pop();
            }

            [Test]
            public void then_it_should_have_one_element()
            {

                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Count.ShouldEqual(1);
            }
        }

        public class when_pushing_to_cache : given.the_sample_class_cache
        {
            protected override void When()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Push(new SampleClass());
                Cache<SampleClass>.Push(new SampleClass());
            }

            [Test]
            public void then_it_should_have_two_elements()
            {

                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Count.ShouldEqual(2);
            }
        }

        public class when_caching_with_custom_factory : given.the_sample_class_cache
        {
            protected override void When()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.SetFactory(() => new SampleClass() { Name = "Custom" });
                myCache.Generate(1);
            }

            [Test]
            public void then_it_should_have_elements_with_custom_name()
            {

                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Pop().Name.ShouldBeSameAs("Custom");
            }
        }

        public class when_using_custom_reset : given.the_sample_class_cache
        {
            protected override void When()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.SetResetAction((instance) => instance.Name = "Reset");
                myCache.Generate(1);
                var sampleInstance = myCache.Pop();
                sampleInstance.Name = "Popped";
                myCache.Push(sampleInstance);
            }

            [Test]
            public void then_it_should_have_elements_with_reset_name()
            {

                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Pop().Name.ShouldBeSameAs("Reset");
            }
        }

        public class when_Allowing_To_Expand : given.the_sample_class_cache
        {
            protected override void When()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.AllowExpand().Generate(1);
                var instance1 = myCache.Pop();
                var instance2 = myCache.Pop();
                var instance3 = myCache.Pop();
                myCache.Push(instance1);
                myCache.Push(instance2);
                myCache.Push(instance3);
            }

            [Test]
            public void then_it_should_have_elements_with_reset_name()
            {

                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.Count.ShouldEqual(3);
            }
        }

        public class when_allowing_to_recycle_with_one_element_in_cache : given.the_sample_class_cache
        {
            protected override void When()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.AllowRecycle().Generate(1);
            }

            [Test]
            public void then_all_pop_elements_should_be_the_same()
            {

                var myCache = Cache<SampleClass>.DefaultPool;

                myCache.Count.ShouldEqual(1);

                var instance1 = myCache.Pop();
                var instance2 = myCache.Pop();

                instance1.ShouldEqual(instance2);
                myCache.Count.ShouldEqual(0);
            }

            [Test]
            public void then_recycled_element_should_reset()
            {
                var myCache = Cache<SampleClass>.DefaultPool;
                myCache.SetResetAction(instance => instance.count = 0);

                var instance1 = myCache.Pop();
                instance1.count++;
                var instance2 = myCache.Pop();
                instance2.count++;
                var instance3 = myCache.Pop();
                instance3.count.ShouldEqual(0);
            }
        }
    }

    public class SampleClass
    {
        public string Name = string.Empty;
        public int count = 0;
    }
}
