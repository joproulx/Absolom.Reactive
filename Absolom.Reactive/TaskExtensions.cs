using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Absolom.Reactive
{
    public static class TaskExtensions
    {
        private static bool ValidateTask<T>(Task task, TaskCompletionSource<T> tcs)
        {
            if (task.IsFaulted)
            {
                tcs.TrySetException(task.Exception.InnerExceptions);
                return false;
            }
            else if (task.IsCanceled)
            {
                tcs.TrySetCanceled();
                return false;
            }
            return true;
        }


        /// <summary>
        /// Based on: http://stackoverflow.com/questions/14630770/sequential-processing-of-asynchronous-tasks
        /// </summary>
        public static Task Then(this Task first, Func<Task> next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<object>();
            first.ContinueWith(delegate
            {
                if (!ValidateTask(first, tcs))
                {
                    return;
                }

                try
                {
                    var t = next();
                    if (t == null)
                    {
                        tcs.TrySetCanceled();
                        return;
                    }

                    t.ContinueWith(delegate
                    {
                        if (ValidateTask(t, tcs))
                        {
                            tcs.TrySetResult(null);
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        /// <summary>
        /// Based on: http://stackoverflow.com/questions/14630770/sequential-processing-of-asynchronous-tasks
        /// </summary>
        public static Task<T> Then<T>(this Task first, Func<Task<T>> next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<T>();
            first.ContinueWith(delegate
            {
                if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
                else if (first.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var t = next();
                        if (t == null) tcs.TrySetCanceled();
                        else t.ContinueWith(delegate
                        {
                            if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                            else if (t.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(t.Result);
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        /// <summary>
        /// Based on: http://stackoverflow.com/questions/14630770/sequential-processing-of-asynchronous-tasks
        /// </summary>
        public static Task<T> Then<T>(this Task first, Task<T> next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<T>();
            first.ContinueWith(delegate
            {
                if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
                else if (first.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var t = next;
                        t.ContinueWith(delegate
                        {
                            if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                            else if (t.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(t.Result);
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        /// <summary>
        /// Based on: http://stackoverflow.com/questions/14630770/sequential-processing-of-asynchronous-tasks
        /// </summary>
        public static Task Then(this Task first, Task next)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (next == null) throw new ArgumentNullException("next");

            var tcs = new TaskCompletionSource<object>();
            first.ContinueWith(delegate
            {
                if (first.IsFaulted) tcs.TrySetException(first.Exception.InnerExceptions);
                else if (first.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        var t = next;
                        t.ContinueWith(delegate
                        {
                            if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                            else if (t.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(null);
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                    catch (Exception exc) { tcs.TrySetException(exc); }
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

    }
}
