# Absolom.Reactive
Extension methods for Reactive Extension (Rx, C#). 

###Description

One of the many ```Observable.SelectMany()``` overloads has the following signature:

```c#
IObservable<TResult> SelectMany<TSource, TResult>(Func<TSource, Task<TResult>> selector)
``` 

This operator basically lets you generate an observable sequence by applying an async transformation on each element of an input sequence. The catch is that each element in the output sequence is inserted when the async transformation is completed. Meaning that the output sequence order can be different than the input one.

This is an example of the behavior:  

```c#
var input = Observable.Range(1, 10);

Random random = new Random();
var output = input.SelectMany(async i =>
{
    await Task.Delay(random.Next(0, 1000));
    return i;
});

input.Buffer(10).Subscribe(l => Console.WriteLine("Input:  " + string.Join(",", l)));
output.Buffer(10).Subscribe(l => Console.WriteLine("Output: " + string.Join(",", l)));
``` 
You can have a result like this:
```
Input:  1,2,3,4,5,6,7,8,9,10
Output: 9,4,10,6,8,2,3,7,1,5
```
In some cases, you don't mind about the order, so the behavior is perfectly valid. But what if you want to keep the same order as the input sequence no matter how long each async transformation takes?
```
Input:  1,2,3,4,5,6,7,8,9,10
Output: 1,2,3,4,5,6,7,8,9,10
```
#####SelectManyEx
```Absolom.Reactive``` library offers a ```SelectManyEx()``` method that lets you do just that. 
```c#
IObservable<TResult> SelectManyEx<TSource, TResult>(Func<TSource, Task<TResult>> selector)
``` 
