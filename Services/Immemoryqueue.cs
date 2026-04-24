using System.Collections.Concurrent;
using Emergy_report.models;

namespace Emergy_report.Services

{ 
    public class Immemoryqueue 
    {
    
      private readonly ConcurrentQueue<Emerguapp> _queue = new();

        public void Enqueue(Emerguapp item)
        {
            _queue.Enqueue(item);
        }

        public bool TryDequeue(out Emerguapp? item) 
        {
            return _queue.TryDequeue(out item);
        }
        public int Count => _queue.Count;
    }
}

