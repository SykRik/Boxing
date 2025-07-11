using System.Collections.Generic;
using UnityEngine;

namespace Boxing
{
    public class InputQueue
    {
        private class InputEvent
        {
            public ConditionType Type;
            public float ExpireTime;
        }

        private readonly LinkedList<InputEvent> queue = new();
        private readonly float defaultExpire = 0.2f;

        public void Enqueue(ConditionType type)
        {
            float expireAt = Time.time + defaultExpire;

            if (queue.Count > 0)
            {
                var last = queue.Last.Value;
                if (last.Type == type)
                {
                    last.ExpireTime = expireAt;
                    return;
                }
            }

            queue.AddLast(new InputEvent
            {
                Type = type,
                ExpireTime = expireAt
            });
        }

        public bool PeekAndConsume(ConditionType type)
        {
            while (queue.Count > 0)
            {
                var input = queue.First.Value;

                if (Time.time > input.ExpireTime)
                {
                    queue.RemoveFirst();
                    continue;
                }

                if (input.Type == type)
                {
                    queue.RemoveFirst();
                    return true;
                }

                break;
            }

            return false;
        }

        public void ClearAll()
        {
            queue.Clear();
        }
    }
}
