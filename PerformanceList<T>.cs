using System;
using System.Collections;
using System.Collections.Generic;

namespace FabmPerformance {
    #region PerformanceList 
    [Serializable]
    public struct PerformanceList<T> : IPerformanceList<T>, ICollection<T> {

        static readonly T[] EmptyArray = new T[0];
        private T[] Items;

        #region Properties
        public T this[int index] {
            get {
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
        #endregion

        #region Constructor
        public PerformanceList(int count = 0) {
            Items = new T[count];
            Count = count;
            IsReadOnly = false;
        }

        public PerformanceList(T[] items, int count) {
            Items = items;
            Count = count;
            IsReadOnly = false;
        }
        #endregion

        #region Add
        public void Add(T item) {
            if (Items == null)
                Items = EmptyArray;

            if (Count == Items.Length) {
                T[] newItems = new T[Count + 1];
                Array.Copy(Items, 0, newItems, 0, Count);
                Items = newItems;
            }

            Items[Count] = item;
            Count += 1;
        }
        public void AddRange(params T[] source) => AddRange(Count, source);
        public void AddRange(IEnumerable<T> source) => AddRange(Count, source);
        public void AddRange(int index, IEnumerable<T> source) {
            if (source == null)
                throw new ArgumentNullException();

            if ((uint)index > (uint)Count)
                throw new ArgumentOutOfRangeException();


            using (IEnumerator<T> en = source.GetEnumerator()) {
                while (en.MoveNext()) {
                    Insert(index, en.Current);
                    index += 1;
                }
            }

        }
        #endregion

        #region Contains
        public bool Contains(T item) => Contains(item, 0, Count);
        public bool Contains(T item, int start) => Contains(item, start, Count);
        public bool Contains(T item, int start, int end) {
            if (item == null) {
                for (int i = start; i < end; i += 1)
                    if (Items[i] == null) return true;
            } else {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                using (Enumerator en = new Enumerator(this)) {
                    while (en.MoveNext()) {
                        if (c.Equals(en.Current, item)) return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Remove
        private void RemoveAT(int index) => Remove(index);

        public bool Remove(T item) {
            int itemIndex = IndexOf(item);
            if (itemIndex < 0) return false;
            Remove(itemIndex);
            return true;
        }
        public void Remove(int index) {
            if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();
            Count -= 1;
            if (index < Count) {
                Array.Copy(Items, index + 1, Items, index, Count - index);
                T[] target = new T[Count];
                CopyTo(target, 0, Count);
                Items = target;
            }
        }
        #endregion

        #region CopyTo
        public void CopyTo(T[] targetArray) => CopyTo(targetArray, 0, 0);
        public void CopyTo(T[] targetArray, int arrayIndex) => CopyTo(targetArray, arrayIndex, 0);
        public void CopyTo(T[] targetArray, int arrayIndex, int count) {
            Array.Copy(Items, 0, targetArray, arrayIndex, Count);
        }
        #endregion

        #region IndexOf
        public int IndexOf(T item) => IndexOf(item, 0, Count);
        public int IndexOf(T item, int start) => IndexOf(item, start, Count);
        public int IndexOf(T item, int start, int end) {
            int range = Count;
            if (!Contains(item, start, end))
                return -1;

            if (start != 0 && end != range)
                range = start > end
                     ? start - end
                     : end - start;
            EqualityComparer<T> c = EqualityComparer<T>.Default;
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext()) {
                    if (c.Equals(en.Current, item)) return en.CurrentIndex;
                }
            }

            return -1;
        }
        #endregion

        public void Insert(int index, T item) {
            if ((uint)index > (uint)Count) throw new ArgumentOutOfRangeException();

            if (Count == Items.Length) {
                T[] newItems = new T[Count + 1];
                Array.Copy(Items, 0, newItems, 0, Count);
                Items = newItems;
            }

            if (index < Count) {
                Array.Copy(Items, index, Items, index + 1, Count - index);
            }

            Count += 1;
            Items[index] = item;
        }

        public void Clear() {
            if (Items != EmptyArray)
                Items = EmptyArray;

            Count = 0;
        }
        public void Revers() { //bubble 
            for (int i = 0; i < Count / 2; i += 1) {
                T t1 = Items[i];
                T t2 = Items[Count - 1 - i];
                Items[i] = t2;
                Items[Count - 1 - i] = t1;
            }
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #region Lambda/Linq
        public void ForEach(Action<T> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    action(en.Current);
            }
        }

        public void ForEach(Action<T, int> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    action(en.Current, en.CurrentIndex);
            }
        }

        public IEnumerator<T1> Select<T1>(Func<T, T1> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    yield return action(en.Current);
            }
        }

        public IEnumerator<T1> Select<T1>(Func<T, int ,T1> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    yield return action(en.Current, en.CurrentIndex);
            }
        }
        public IEnumerator<T> Where(Func<T, bool> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    if (action(en.Current))
                        yield return en.Current;
            }
        }
        public IEnumerator<T> Where(Func<T, int, bool> action) {
            using (Enumerator en = new Enumerator(this)) {
                while (en.MoveNext())
                    if (action(en.Current, en.CurrentIndex))
                        yield return en.Current;
            }
        }
        #endregion

        public T[] ToArray() => Items;

        public struct Enumerator : IEnumerator<T>, IEnumerator {

            private PerformanceList<T> list;
            private int index;
            private T current;

            public T Current { get => current; }
            public int CurrentIndex { get => index; }
            object IEnumerator.Current { get => current; }

            public Enumerator(PerformanceList<T> list) {
                this.list = list;
                index = 0;
                current = default;
            }

            public void Dispose() { }
            public bool MoveNext() {
                PerformanceList<T> localList = list;

                if (((uint)index < (uint)localList.Count)) {
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
}
#endregion
