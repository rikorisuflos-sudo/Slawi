using System.Collections.Generic;
using UnityEngine;

public class BoundedQueue<T>
{
    private Queue<T> queue = new Queue<T>();
    private int capacity;

    public BoundedQueue(int capacity)
    {
        this.capacity = capacity;
    }

    public void Enqueue(T item)
    {
        if (queue.Count == capacity)
        {
            queue.Dequeue();
        }
        queue.Enqueue(item);
    }

    public T Dequeue() => queue.Dequeue();
    public int Count => queue.Count;
    public T Peek() => queue.Peek();
}
