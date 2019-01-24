using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
    
[DebuggerTypeProxy(typeof(Mscorlib_CollectionDebugView<>))]
[Serializable]
public unsafe struct PerformanceList<T> : IList<T> where T : unmanaged  {
    private const int _defaultCapacity = 4;
    //#region Events
    //public event Action<PerformanceList<T>, T, int> Added;
    //public event Action<PerformanceList<T>, T, int> Removed;
    //public event Action<PerformanceList<T>> Changed;
    //#endregion

    public static readonly T[] EmptyArray = new T[_defaultCapacity];
    public T[] Items;

    //Properties
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

    private int Capacity;

    //Constructor

    //private PerformanceList() {
    //    Count = 0;
    //    Capcity = 1024;
    //    Items = new T[Capcity];
    //    IsReadOnly = false;
    //}
    public PerformanceList(int count = 0) : this() {
        Count = count;
        Capacity = count > _defaultCapacity ? Count : _defaultCapacity;
        Items = new T[Capacity];
        IsReadOnly = false;
    }
    public PerformanceList(T[] items) {
        Count = items.Length;
        Capacity = items.Length;
        Items = items;
        IsReadOnly = false;
    }
    

    //Add
    [__DynamicallyInvokable]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item) {
        if (Count == Capacity) {
            T[] newItems = new T[Capacity *= 2];
            Array.Copy(Items, 0, newItems, 0, Count);
            Items = newItems;
        }
        fixed (T* itemPtr = Items) {
            *(itemPtr + Count++) = item;
        }
    }

    public void AddRange(params T[] source) => AddRange(Count, source);
    public void AddRange(int index, T[] source) {
        if (source == null)
            throw new ArgumentNullException();

        if ((uint)index > (uint)Count)
            throw new ArgumentOutOfRangeException();

        fixed (T* sourcePtr = source) {
            for (int iElements = 0, nElements = source.Length; iElements < nElements; iElements++) {
                Insert(index, *(sourcePtr + iElements));
                index += 1;
            }
        }

        /*if ((uint)index > (uint)Count) throw new ArgumentOutOfRangeException();

        if (Count == Items.Length) {
            T[] newItems = new T[Count + 1];
            Array.Copy(Items, 0, newItems, 0, Count);
            Items = newItems;
        }

        if (index < Count) {
            Array.Copy(Items, index, Items, index + 1, Count - index);
        }

        Count += 1;
        Items[index] = item;*/
        //Changed?.Invoke(this);
    }

    //Contains
    public bool Contains(T item) => Contains(item, 0, Count);
    public bool Contains(T item, int start) => Contains(item, start, Count);
    public bool Contains(T item, int start, int end) {
        fixed (T* itemsPtr = Items) {
            if (Equals(item, null)) {
                for (int i = start; i < end; i += 1) {
                    if (Equals(*(itemsPtr + i), null)) {
                        return true;
                    }
                }
            } else {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < Count; i++) {
                    if (c.Equals(*(itemsPtr + i), item)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //Remove
    public void RemoveAt(int index) => Remove(index);

    public bool Remove(T item) {
        int itemIndex = IndexOf(item);
        if (itemIndex < 0) return false;
        Remove(itemIndex);
        return true;
    }
    public void Remove(int index) {
        if ((uint)index >= (uint)Count) throw new ArgumentOutOfRangeException();
        Count -= 1;
        T item = Items[index];
        if (index < Count) {
            Array.Copy(Items, index + 1, Items, index, Count - index);
            T[] target = new T[Count];
            CopyTo(target, 0, Count);
            Items = target;
        }
        //Removed?.Invoke(this, item, index);
        //Changed?.Invoke(this);
    }

    //CopyTo
    public void CopyTo(T[] targetArray) => CopyTo(targetArray, 0, 0);
    public void CopyTo(T[] targetArray, int arrayIndex) => CopyTo(targetArray, arrayIndex, 0);
    public void CopyTo(T[] targetArray, int arrayIndex, int count) => Array.Copy(Items, 0, targetArray, arrayIndex, Count);

    //IndexOf
    public int IndexOf(T item) => IndexOf(item, 0, Count);
    public int IndexOf(T item, int start) => IndexOf(item, start, Count);
    public unsafe int IndexOf(T item, int start, int end) {
        int range = Count;
        if (!Contains(item, start, end))
            return -1;

        if (start != 0 && end != range)
            range = start > end
                 ? start - end
                 : end - start;
        EqualityComparer<T> c = EqualityComparer<T>.Default;
        fixed (T* itemsPtr = Items) {
            for (int i = 0; i < Count; i++) {
                if (c.Equals(*(itemsPtr + i), item)) return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item) {
        if ((uint)index > (uint)Count) throw new ArgumentOutOfRangeException();

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
    public void Insert(int index, T[] item) {
        if ((uint)index > (uint)Count) throw new ArgumentOutOfRangeException();

        if (Count == Items.Length) {
            T[] newItems = new T[Count += item.Length];
            Array.Copy(Items, 0, newItems, 0, Count);
            Items = newItems;
        }

        if (index < Count) {
            Array.Copy(Items, index, Items, index + item.Length, Count - index - item.Length);
        }

        Count += 1;
        Array.Copy(item, Items, item.Length);
    }
    public void Clear() {
        if (Items != EmptyArray)
            Items = EmptyArray;

        Count = 0;
        //Changed?.Invoke(this);
    }

    public void Revers() { //bubble 
        fixed (T* itemsPtr = Items) {
            for (int iElement = 0, nElement = Count / 2; iElement < nElement; iElement += 1) {
                T t1 = *(itemsPtr + iElement);
                *(itemsPtr + iElement) = *(itemsPtr + Count - 1 - iElement);
                *(itemsPtr + Count - 1 - iElement) = t1;
            }
        }
        //Changed?.Invoke(this);
    }

    public IEnumerator<T> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    public static implicit operator PerformanceList<T>(T[] source) => new PerformanceList<T>(source);

}
