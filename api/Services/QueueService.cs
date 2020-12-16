// David Wahid
using System;
using System.Collections.Concurrent;

namespace api.Services
{
    public interface IQueueService<T>
    {
        void Enqueue(T item);
        T Dequeue();
    }

    public class QueueService<T> : IQueueService<T> where T : class
    {
        private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        public T Dequeue()
        {
            var success = _items.TryDequeue(out var workItem);

            return success
                ? workItem
                : null;
        }

        public void Enqueue(T item)
        {
            if (item == null) throw new NotImplementedException();

            _items.Enqueue(item);
        }
    }
}
