[Serializable]
    public struct PerformanceList<T> : IList<T> {

        static readonly T[] EmptyArray = new T[0];
        private T[] Items;

        public T this[int index] { get {
                if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();
                return Items[index];
            }
            set {
                if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();
                Items[index] = value;
            }
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; }

        public PerformanceList(int count = 0)  {
            Items = count == 0 ? EmptyArray : new T[count];
            Count = count;
            IsReadOnly = false;
        }

        public PerformanceList(T[] items, int count) {
            Items = items;
            Count = count;
            IsReadOnly = false;
        }

        public void Add(T item) {
            if (Count == Items.Length) {
                T[] newItems = new T[Count++];
                Array.Copy(Items, 0, newItems, 0, Count);
                Items = newItems;
            }

            Count += 1;
            Items[Count] = item;
        }

        public void Clear() {
            if (Items != EmptyArray)
                Items = EmptyArray;
        }

        public bool Contains(T item) {
            if (item == null) {
                for (int i = 0; i < Count; i+=1)
                    if (Items[i] == null)  return true;
            } else {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < Count; i+=1)
                    if (c.Equals(Items[i], item)) return true;
            }
            return false;
        }
        public void CopyTo(T[] targetArray) => CopyTo(targetArray, 0, 0);
        public void CopyTo(T[] targetArray, int arrayIndex) => CopyTo(targetArray, arrayIndex, 0);
        public void CopyTo(T[] targetArray, int arrayIndex, int count) {
            Array.Copy(Items, 0, targetArray, arrayIndex, Count);
        }

        public int IndexOf(T item) => IndexOf(item, 0, Count);
        public int IndexOf(T item, int start) => IndexOf(item, start, Count);
        public int IndexOf(T item, int startIndex, int endIndex) {
            int range = Count;

            if(startIndex != 0 && endIndex != range)
               range = startIndex > endIndex 
                    ? startIndex - endIndex 
                    : endIndex - startIndex;

            for (int i = 0; i < range; i++) {
                if (Equals(Items[i], item)) return i;
            }

            return -1;
        }

        public void Insert(int index, T item) {
            if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();

            if (Count == Items.Length) {
                T[] newItems = new T[Count++];
                Array.Copy(Items, 0, newItems, 0, Count);
                Items = newItems;
            }

            if (index < Count) {
                Array.Copy(Items, index, Items, index + 1, Count - index);
            }

            Count += 1;
            Items[index] = item;
        }

        public bool Remove(T item) {
            int itemIndex = IndexOf(item);
            if (itemIndex < 0) return false;

            return true;
        }
        public void RemoveAt(int index) {
            if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();
            if (index < Count) {
                Array.Copy(Items, index + 1, Items, index, Count - index);
                Count -= 1;
                T[] target = new T[Count];
                CopyTo(target, 0, Count);
                Items = target;
            }
        }
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);


        public struct Enumerator : IEnumerator<T>, IEnumerator {

            private PerformanceList<T> list;
            private int index;
            private T current;

            public T Current { get => current; }
            object IEnumerator.Current { get => current; }


            public Enumerator(PerformanceList<T> list) {
                this.list = list;
                index = 0;
                current = default;
            }

            public void Dispose() { }
            public bool MoveNext() {

                PerformanceList<T> localList = list;

                if(((uint)index < (uint)localList.Count)) {
                    current = localList.Items[index];
                    index++;
                    return true;
                }

                index = list.Count + 1;
                current = default;
                return false;
            }
            public void Reset() {
                index = 0;
                current = default(T);
            }
        }
    }
