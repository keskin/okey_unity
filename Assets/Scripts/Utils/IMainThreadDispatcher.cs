using System;
namespace Utils
{
    public interface IMainThreadDispatcher
    {
        void Enqueue(Action action);
    }
}