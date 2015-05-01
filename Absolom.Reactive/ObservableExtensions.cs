using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Absolom.Reactive
{
    public static class ObservableExtensions
    {

        public static IObservable<T> SelectManyEx<T>(this IObservable<T> source, Func<T, Task<T>> selectorAsync)
        {
            return Observable.Create<T>(o =>
            {
                Task<T> current = null;

                var sub = source.Subscribe(u =>
                {
                    try
                    {
                        if (current == null)
                        {
                            current = selectorAsync(u);
                        }
                        else
                        {
                            var next = selectorAsync(u);
                            var last = ContinueNext(current, o);
                            current = last.Then(next);
                        }
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                },
                o.OnError,
                () =>
                {
                    if (current == null)
                        o.OnCompleted();
                    else
                        Complete(current, o);
                });

                return sub.Dispose;
            });
        }

        private static Task ContinueNext<T>(Task<T> task, IObserver<T> observable)
        {
            return Continue(task, observable, t => observable.OnNext(t.Result));
        }

        private static void Complete<T>(Task<T> task, IObserver<T> observable)
        {
            Continue(task, observable, t =>
            {
                observable.OnNext(t.Result);
                observable.OnCompleted();
            });
        }

        private static Task Continue<T>(Task<T> task, IObserver<T> observable, Action<Task<T>> onResult)
        {
            var last = task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    observable.OnError(t.Exception ?? new Exception("Exception occurred while executing asynchonous operation"));
                    return;
                }

                if (t.IsCanceled)
                {
                    observable.OnCompleted();
                    return;
                }

                onResult(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return last;
        }
    }
}
