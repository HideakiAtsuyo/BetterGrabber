using System;
using System.Collections.Generic;

namespace FVM.Helpers.MethodBlocks
{
    public static class BlockUtils
    {
        public static void AddListEntry<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
        {
            bool flag = key == null;
            if (flag)
            {
                throw new ArgumentNullException("key");
            }
            List<TValue> list;
            bool flag2 = !self.TryGetValue(key, out list);
            if (flag2)
            {
                list = (self[key] = new List<TValue>());
            }
            list.Add(value);
        }
    }
}
