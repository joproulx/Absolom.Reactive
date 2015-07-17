using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Absolom.Reactive
{
    public static class ObservableExtensions
    {
        public static IObservable<TOut> Select<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> asyncSelector)
        {
            return Observable.Create<TOut>(observer =>
            {
                Task task = Task.FromResult(default(object));

                return source.Subscribe(value =>
                    {
                        var asyncProjection = asyncSelector(value);

                        task = Task.WhenAll(task, asyncProjection)
                                   .ContinueWith(_ =>
                                   {
                                       try
                                       {
                                           observer.OnNext(asyncProjection.Result);
                                       }
                                       catch (Exception ex)
                                       {
                                           observer.OnError(ex);
                                       }
                                   }, TaskContinuationOptions.ExecuteSynchronously);
                    },
                    error => task.ContinueWith(_ => observer.OnError(error), TaskContinuationOptions.ExecuteSynchronously),
                    () => task.ContinueWith(_ => observer.OnCompleted(), TaskContinuationOptions.ExecuteSynchronously));
            });
        }

    }
}
