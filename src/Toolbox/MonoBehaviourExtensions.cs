using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBox.Extensions
{
    internal static class MonoBehaviourExtensions
    {
        public static Coroutine ExecuteDelayed(this MonoBehaviour self, Action action, int frameCount = 1)
        {
            return self.StartCoroutine(ExecuteDelayed_Routine(action, frameCount));
        }

        private static IEnumerator ExecuteDelayed_Routine(Action action, int frameCount = 1)
        {
            for (int i = 0; i < frameCount; i++)
                yield return null;
            action();
        }

        public static Coroutine ExecuteDelayed(this MonoBehaviour self, Action action, float delay, bool timeScaled = true)
        {
            return self.StartCoroutine(ExecuteDelayed_Routine(action, delay, timeScaled));
        }

        private static IEnumerator ExecuteDelayed_Routine(Action action, float delay, bool timeScaled)
        {
            if (timeScaled)
                yield return new WaitForSeconds(delay);
            else
                yield return new WaitForSecondsRealtime(delay);
            action();
        }

        public static Coroutine ExecuteDelayedFixed(this MonoBehaviour self, Action action, int waitCount = 1)
        {
            return self.StartCoroutine(ExecuteDelayedFixed_Routine(action, waitCount));
        }

        private static IEnumerator ExecuteDelayedFixed_Routine(Action action, int waitCount)
        {
            for (int i = 0; i < waitCount; i++)
                yield return new WaitForFixedUpdate();
            action();
        }

        public static Coroutine ExecuteDelayed(this MonoBehaviour self, Func<bool> waitUntil, Action action)
        {
            return self.StartCoroutine(ExecuteDelayed_Routine(waitUntil, action));
        }

        private static IEnumerator ExecuteDelayed_Routine(Func<bool> waitUntil, Action action)
        {
            yield return new WaitUntil(waitUntil);
            action();
        }

        private static readonly LinkedList<Action> _queuedActions = new LinkedList<Action>();
        private static Coroutine _queueingCoroutine;
        public static void QueueAction(this MonoBehaviour self, Action action)
        {
            _queuedActions.AddFirst(action);
            if (_queueingCoroutine == null)
                _queueingCoroutine = self.StartCoroutine(QueueAction_Routine());
        }

#if IPA
	    public static void QueueAction(this IPlugin self, Action action)
	    {
		    _queuedActions.AddFirst(action);
		    if (_queueingCoroutine == null)
			    _queueingCoroutine = self.StartCoroutine(QueueAction_Routine());
	    }
#endif

        private static IEnumerator QueueAction_Routine()
        {
            while (_queuedActions.Count != 0)
            {
                yield return null;
                try
                {
                    _queuedActions.Last.Value?.Invoke();
                    _queuedActions.RemoveLast();
                }
                catch (Exception e)
                {
                    Debug.LogError("Queued action:\n" + e);
                }
            }
            _queueingCoroutine = null;
        }
    }
}