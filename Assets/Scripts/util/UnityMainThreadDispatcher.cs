using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> actions = new Queue<Action>();
    private static readonly Queue<Func<TaskCompletionSource<bool>, Task>> taskActions = new Queue<Func<TaskCompletionSource<bool>, Task>>();

    private static UnityMainThreadDispatcher instance = null;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!instance)
        {
            var obj = new GameObject("MainThreadDispatcher");
            instance = obj.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(obj);
        }
        return instance;
    }

    void Update()
    {
        lock (actions)
        {
            while (actions.Count > 0)
            {
                actions.Dequeue()?.Invoke();
            }
        }

        lock (taskActions)
        {
            while (taskActions.Count > 0)
            {
                var taskFunc = taskActions.Dequeue();
                var tcs = new TaskCompletionSource<bool>();

                try
                {
                    taskFunc(tcs).ContinueWith(task =>
                    {
                        if (task.IsFaulted && task.Exception != null)
                        {
                            tcs.TrySetException(task.Exception);
                        }
                        else
                        {
                            tcs.TrySetResult(true); // Ensure TrySetResult to avoid multiple calls.
                        }
                    });
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex); // Use TrySetException to avoid multiple calls.
                }
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (actions)
        {
            actions.Enqueue(action);
        }
    }

    // New EnqueueAsync Method for synchronous tasks
    public Task<T> EnqueueAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();

        lock (actions)
        {
            actions.Enqueue(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex); // Use TrySetException to avoid multiple SetResult calls.
                }
            });
        }

        return tcs.Task;
    }

    // New EnqueueAsync Method for asynchronous tasks
    public Task EnqueueAsync(Func<Task> func)
    {
        var tcs = new TaskCompletionSource<bool>();

        lock (taskActions)
        {
            taskActions.Enqueue(async (tcs) =>
            {
                try
                {
                    await func();
                    tcs.TrySetResult(true); // Ensure TrySetResult to avoid multiple calls.
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex); // Ensure TrySetException to handle any error.
                }
            });
        }

        return tcs.Task;
    }
}
