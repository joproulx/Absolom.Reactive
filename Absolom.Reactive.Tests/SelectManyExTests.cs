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
            var timeouts = new[] {1000, 100, 200, 0, 500, 1000, 0, 300, 100, 700};


            var range = Observable.Range(0, 10);

            var observable = range.Select(async i =>
            {
                await Task.Delay(timeouts[i]);
                return i;
            });

            var result = await observable.Buffer(10).ToTask();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(i, result[i]);
            }

        }
    }
}
