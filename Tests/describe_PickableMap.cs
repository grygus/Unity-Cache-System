//BDD-Style Tests

using System;
using Moq;
using NUnit.Framework;
using Should;
using SpecsFor;
using UnityEngine;

namespace Grygus.Preset.Tests
{
    public class describe_PickableMap
    {
        public class when_creating_a_map : SpecsFor<PickableMap>
        {

            protected override void Given()
            {
                base.BeforeEachTest();
                SUT = new PickableMap();
            }

            protected override void When()
            {
                SUT.AddNode(0, 0, new Vector3(1, 1, 1));
            }

            [Test]
            public void then_it_should_have_one_element()
            {
                SUT.Count().ShouldEqual(1);
            }

        }

        public class when_removeing_item : SpecsFor<PickableMap>
        {
            protected override void Given()
            {
                base.BeforeEachTest();
                SUT = new PickableMap();
            }

            protected override void When()
            {
                SUT.AddNode(0, 0, new Vector3(1, 1, 1));
                SUT.AddNode(0, 1, new Vector3(1, 1, 2));
                SUT.AddNode(0, 2, new Vector3(1, 1, 3));

                SUT.RemoveNode(0, 2);
            }

            [Test]
            public void then_it_should_have_two_elements()
            {
                SUT.Count().ShouldEqual(2);
            }

            [Test]
            public void then_second_element()
            {
                SUT.GetNode(0, 1).z.ShouldEqual(2);
            }

            [Test]
            public void then_third_element_should_not_exist()
            {
                Action act = () => SUT.GetNode(0, 2);
                act.ShouldThrow<NullReferenceException>();
            }
        }
    }
}