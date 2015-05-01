using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absolom.Reactive.Output
{
    class Program
    {
        static void Main(string[] args)
        {

            var range = Observable.Range(1, 10);

            

            var observable = range.SelectManyEx(async i =>
            {
                await Task.Delay(1000 - (i * 100));
                Console.WriteLine(i);
                return i;
            });

            observable.Subscribe(Console.WriteLine, () => Console.WriteLine("Completed!"));
            Console.ReadKey();
        }
    }
}
