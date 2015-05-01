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
            var input = Observable.Range(1, 10);

            Random random = new Random();
            var output = input.SelectMany(async i =>
            {
                await Task.Delay(random.Next(0, 1000));
                return i;
            });

            input.Buffer(10).Subscribe(l => Console.WriteLine("Input:  " + string.Join(",", l)));
            output.Buffer(10).Subscribe(l => Console.WriteLine("Output: " + string.Join(",", l)));
            Console.ReadKey();
        }
    }
}
