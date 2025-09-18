using UnityEngine;
using System;
using System.Collections.Generic;

namespace Utils
{
    public class MainThreadDispatcher : MonoBehaviour, IMainThreadDispatcher
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void Enqueue(Action action)
        {
            lock (ExecutionQueue)
            {
                ExecutionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0)
                {
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}
