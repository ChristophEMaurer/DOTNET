using System.Collections.Generic;

namespace BitcoinLib
{
    public class OpItems : List<OpItem>
    {
        public OpItems()
        {
        }

        public OpItems(OpItems other)
        {
            AddRange(other);
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
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public OpItem Pop(int n)
        {
            OpItem item = base[n];
            base.RemoveAt(n);

            return item;
        }

        public OpItem Peek()
        {
            OpItem item = base[this.Count - 1];
            return item;
        }
    }
}
