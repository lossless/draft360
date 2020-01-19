namespace NRKernal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool
    {
        private Dictionary<Type, Queue<object>> m_CachePoolDict = new Dictionary<Type, Queue<object>>();
        public int InitCount = 100;

        public void Expansion<T>() where T : new()
        {
            var queue = GetQueue<T>();
            for (int i = 0; i < InitCount; i++)
            {
                T data = new T();
                queue.Enqueue(data);
            }
        }

        public T Get<T>() where T : new()
        {
            var queue = GetQueue<T>();
            if (queue.Count == 0)
            {
                Expansion<T>();
            }

            return (T)queue.Dequeue();
        }

        public void Put<T>(T data) where T : new()
        {
            var queue = GetQueue<T>();
            queue.Enqueue(data);
        }

        private Queue<object> GetQueue<T>() where T : new()
        {
            Queue<object> queue = null;
            m_CachePoolDict.TryGetValue(typeof(T), out queue);
            if (queue == null)
            {
                queue = new Queue<object>();
                m_CachePoolDict.Add(typeof(T), queue);
            }
            return queue;
        }
    }

    public class BytesPool
    {
        public Dictionary<int, Queue<byte[]>> BytesDict = new Dictionary<int, Queue<byte[]>>();

        public byte[] Get(int len)
        {
            if (len <= 0)
            {
                Debug.Log("BytesPool get len is not valid :" + len);
                return null;
            }
            Queue<byte[]> que = null;
            BytesDict.TryGetValue(len, out que);
            if (que == null)
            {
                que = new Queue<byte[]>();
                byte[] temp = new byte[len];
                que.Enqueue(temp);

                BytesDict.Add(len, que);
            }
            else if (que.Count == 0)
            {
                byte[] temp = new byte[len];
                que.Enqueue(temp);
            }

            return que.Dequeue();
        }

        public void Put(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.Log("BytesPool retrieve data is null.");
                return;
            }

            Queue<byte[]> que = null;
            BytesDict.TryGetValue(data.Length, out que);
            if (que == null)
            {
                que = new Queue<byte[]>();
                BytesDict.Add(data.Length, que);
            }
            que.Enqueue(data);
        }
    }
}
