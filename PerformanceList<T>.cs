  [Serializable]
    public struct PerformanceList<T> : IList<T> {

        private T[] Items;
        private int Size;

        static readonly T[] EmptyArray = new T[0];

        public T this[int index] { get {
                if ((uint)index >= (uint)Size) throw new ArgumentOutOfRangeException();
                return Items[index];
            }
            set {
                if ((uint)index >= (uint)Size) throw new ArgumentOutOfRangeException();
                Items[index] = value;
            }
        }

        public int Count { get => Size; }
        public bool IsReadOnly { get; }


        public void Add(T item) {
            if (Size == Items.Length) {
                T[] newItems = new T[Size++];
                Array.Copy(Items, 0, newItems, 0, Size);
                Items = newItems;
            }

            Size += 1;
            Items[Size] = item;
        }

        public void Clear() {
            if (Items != EmptyArray)
                Items = EmptyArray;
        }

        public bool Contains(T item) {
            if (item == null) {
                for (int i = 0; i < Size; i+=1)
                    if (Items[i] == null)  return true;
            } else {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < Size; i+=1)
                    if (c.Equals(Items[i], item)) return true;
            }
            return false;
        }
        public void CopyTo(T[] targetArray) => CopyTo(targetArray, 0, 0);
        public void CopyTo(T[] targetArray, int arrayIndex) => CopyTo(targetArray, arrayIndex, 0);
        public void CopyTo(T[] targetArray, int arrayIndex, int count) {
            Array.Copy(Items, 0, targetArray, arrayIndex, Size);
        }

        public int IndexOf(T item) => IndexOf(item, 0, Size);
        public int IndexOf(T item, int start) => IndexOf(item, start, Size);
        public int IndexOf(T item, int startIndex, int endIndex) {
            int range = Size;

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
            if ((uint)index >= (uint)Size) throw new ArgumentOutOfRangeException();

            if (Size == Items.Length) {
                T[] newItems = new T[Size++];
                Array.Copy(Items, 0, newItems, 0, Size);
                Items = newItems;
            }

            if (index < Size) {
                Array.Copy(Items, index, Items, index + 1, Size - index);
            }

            Size += 1;
            Items[index] = item;
        }

        public bool Remove(T item) {
            int itemIndex = IndexOf(item);
            if (itemIndex < 0) return false;

            return true;
        }
        public void RemoveAt(int index) {
            if ((uint)index >= (uint)Size) throw new ArgumentOutOfRangeException();
            if (index < Size) {
                Array.Copy(Items, index + 1, Items, index, Size - index);
                Size -= 1;
                T[] target = new T[Size];
                CopyTo(target, 0, Size);
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

                if(((uint)index < (uint)localList.Size)) {
                    current = localList.Items[index];
                    index++;
                    return true;
                }

                index = list.Size + 1;
                current = default;
                return false;
            }
            public void Reset() {
                index = 0;
                current = default(T);
            }
        }
    }
