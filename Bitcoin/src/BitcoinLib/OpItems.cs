using System.Collections.Generic;

namespace BitcoinLib
{
    public class OpItems : List<OpItem>
    {
        public OpItems()
        {
        }

        /// <summary>
        /// Create a deep copy of all items. When processing the script in Evaluate(), sometime elements are popped off, but
        /// we must not change the original arrays
        /// </summary>
        /// <param name="other"></param>
        public OpItems(OpItems other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                OpItem item = new OpItem(other[i]);
                this.Add(item);
            }
        }

        public void Push(byte[] item)
        {
            base.Add(new OpItem(item));
        }

        public void Push(OpItem item)
        {
            base.Add(item);
        }

        public OpItem Pop()
        {
            OpItem item = base[this.Count - 1];
            base.RemoveAt(this.Count - 1);

            return item;
        }

        /// <summary>
        /// OpItem Pop TODO: change a list to a Queue or Stack as removing the first element and shifting all others is expensive!
        /// The last item in the array is removed and returned. The list of items changes.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public OpItem Pop(int n)
        {
            OpItem item = base[n];
            base.RemoveAt(n);

            return item;
        }

        /// <summary>
        /// The last item in the array is returned without changing the array.
        /// </summary>
        /// <returns></returns>
        public OpItem Peek()
        {
            OpItem item = base[this.Count - 1];
            return item;
        }
    }
}
