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
                taskFunc(tcs);
                tcs.SetResult(true);
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

    // New EnqueueAsync Method
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
                    tcs.SetException(ex);
                }
            });
        }

        return tcs.Task;
    }

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
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
        }

        return tcs.Task;
    }
}
