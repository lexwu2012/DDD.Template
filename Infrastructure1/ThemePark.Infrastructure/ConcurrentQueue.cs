using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ThemePark.Infrastructure
{
    /// <summary>
    /// Ϊ���������ṩ���й���
    /// </summary>
    public class ConcurrentQueue
    {
        private readonly ConcurrentDictionary<object, QueueItem>
            _concurrentQueue = new ConcurrentDictionary<object, QueueItem>();

        private readonly object defaultKey = new object();

        /// <summary>
        /// Ĭ��ʵ��
        /// </summary>
        public static ConcurrentQueue Default { get; } = new ConcurrentQueue();

        /// <summary>
        /// �����������ִ������
        /// </summary>
        /// <param name="key">��������</param>
        public async Task<IDisposable> QueueAsync(object key)
        {
            key = key ?? defaultKey;

            var taskQueue = _concurrentQueue.GetOrAdd(key, _ => new QueueItem(key));

            Interlocked.Increment(ref taskQueue.TaskCount);
            var task = new QueueTask(this, taskQueue);

            taskQueue.Tasks.Enqueue(task);
            QueueTask wateTask = null;

            while (taskQueue.Tasks.TryPeek(out wateTask) && wateTask != task)
            {
                await wateTask.TaskCompletion.Task;
            }

            return task;
        }

        class QueueItem
        {
            public readonly ConcurrentQueue<QueueTask> Tasks = new ConcurrentQueue<QueueTask>();

            public readonly object Key;

            public QueueItem(object key)
            {
                Key = key;
            }

            public int TaskCount = 0;
        }

        class QueueTask : IDisposable
        {
            private readonly ConcurrentQueue _queue;
            private readonly QueueItem _item;

            public QueueTask(ConcurrentQueue queue, QueueItem item)
            {
                _queue = queue;
                _item = item;
            }

            public readonly TaskCompletionSource<bool> TaskCompletion = new TaskCompletionSource<bool>();

            /// <summary>
            /// ��Ƕ������������
            /// </summary>
            public void Dispose()
            {
                QueueTask wateTask = null;
                if (_item.Tasks.TryPeek(out wateTask) && wateTask == this)
                    _item.Tasks.TryDequeue(out wateTask);

                TaskCompletion.TrySetResult(true);

                QueueItem wateItem;
                if (Interlocked.Decrement(ref _item.TaskCount) == 0)
                    _queue._concurrentQueue.TryRemove(_item.Key, out wateItem);
            }
        }
    }
}