using System;
using System.Collections;
using UnityEngine;

public enum CoroutineState
{
    RUNNING,
    EXHAUSTED,
    ERROR
}

public class InvalidCoroutineState : Exception
{
    public InvalidCoroutineState(CoroutineState state) : base($"Invalid CoroutineState: {state.ToString()}") { }
}

public static partial class Extensions
{
    private static CoroutineState ExecuteCoroutineStep(IEnumerator coroutine)
    {
        try
        {
            return coroutine.MoveNext() ? CoroutineState.RUNNING : CoroutineState.EXHAUSTED;
        }
        catch
        {
            return CoroutineState.ERROR;
        }
    }

    private static IEnumerator ResilientCoroutine(Func<object[], IEnumerator> coroutineFactory, object[] args)
    {
        IEnumerator coroutine = coroutineFactory(args);

        while (true)
        {
            CoroutineState state = ExecuteCoroutineStep(coroutine);

            switch (state)
            {
                case CoroutineState.RUNNING:
                    yield return coroutine.Current;
                    break;

                case CoroutineState.ERROR:
                    coroutine = coroutineFactory(args);
                    yield return new WaitForSeconds(1.0f);
                    break;

                case CoroutineState.EXHAUSTED:
                    yield break;

                default:
                    throw new InvalidCoroutineState(state);
            }
        }
    }

    public static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory, params object[] args)
    {
        return self.StartCoroutine(ResilientCoroutine(coroutineFactory, args));
    }

    public static Coroutine StartResilientCoroutine(this MonoBehaviour self, Func<object[], IEnumerator> coroutineFactory)
    {
        return self.StartResilientCoroutine(coroutineFactory, new object[] { });
    }
}
