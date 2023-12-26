using NUnit.Framework;
using System;

namespace J2N.Memory
{
    internal class TestInt32Packer
    {
        [Test]
        public void TestSetValueAndGetMinBound()
        {
            ValueInt32Packer packer = new ValueInt32Packer();
            packer.SetValue(0, 0);

            Assert.AreEqual(0, packer.GetValue(0));
        }

        [Test]
        public void TestSetValueAndGetIntermediateValue()
        {
            ValueInt32Packer packer = new ValueInt32Packer();
            int value = 12345;
            packer.SetValue(5, value);

            Assert.AreEqual(value, packer.GetValue(5));
        }

        [Test]
        public void TestSetValueAndGetMaxBound()
        {
            ValueInt32Packer packer = new ValueInt32Packer();
            int value = ValueInt32Packer.MaxValue;
            packer.SetValue(15, value);

            Assert.AreEqual(value, packer.GetValue(15));
        }

        [Test]
        public void TestSetValueWithInvalidSlotIndex()
        {
            ValueInt32Packer packer = new ValueInt32Packer();

            try
            {
                packer.SetValue(-1, 100);
                Assert.Fail("Expected ArgumentOutOfRangeException not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
            try
            {
                packer.SetValue(32, 100);
                Assert.Fail("Expected ArgumentOutOfRangeException not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void TestSetValueWithInvalidValue()
        {
            ValueInt32Packer packer = new ValueInt32Packer();

            try
            {
                packer.SetValue(10, -1);
                Assert.Fail("Expected ArgumentOutOfRangeException not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
            try
            {
                packer.SetValue(20, ValueInt32Packer.MaxValue + 1);
                Assert.Fail("Expected ArgumentOutOfRangeException not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void TestGetSlotMask()
        {
            int slotIndex = 3;
            ulong expectedMask = (ulong)ValueInt32Packer.MaxValue << (slotIndex % ValueInt32Packer.SlotsPerULong) * ValueInt32Packer.BitsPerSlot;

            ulong resultMask = ValueInt32Packer.GetSlotMask(slotIndex);

            Assert.AreEqual(expectedMask, resultMask);
        }
    }
}
