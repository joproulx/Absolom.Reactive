using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Absolom.Reactive.Tests
{
    [TestClass]
    public class SelectManyExTests
    {
        [TestMethod]
        public async Task TestOutputSequenceOrder()
        {
            var range = Observable.Range(1, 10);

            var observable = range.SelectManyEx(async i =>
            {
                await Task.Delay(1000 - (i*100));
                return i;
            });

            var result = await observable.Buffer(10).ToTask();

            for (int i = 1; i < 10; i++)
            {
                Assert.AreEqual(i, result[i]);
            }

        }
    }
}
