#define Experimental
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace EndLessBrick.PerformanceX {
    [Serializable]
    public struct PerformanceList<T> {

        #region Events
        public event Action<PerformanceList<T>, T, int> Added;
        public event Action<PerformanceList<T>, T, int> Removed;
        public event Action<PerformanceList<T>> Changed;
        #endregion

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
            Added = null;
            Removed = null;
            Changed = null;
        }

        public PerformanceList(T[] items, int count) {
            Items = items;
            Count = count;
            IsReadOnly = false;
            Added = null;
            Removed = null;
            Changed = null;
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
            Added?.Invoke(this, item, Count);
            Changed?.Invoke(this);
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
            Changed?.Invoke(this);
        }
        #endregion

        #region Contains
        public bool Contains(T item) => Contains(item, 0, Count);
        public bool Contains(T item, int start) => Contains(item, start, Count);
        public bool Contains(T item, int start, int end) {
            if (item == null) {
                for (int i = start; i < end; i += 1) {
                    if (Items[i] == null) return true;
                }
            } else {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int i = 0; i < Count; i++) {
                    if (c.Equals(Items[i], item)) return true;
                }
            }
            return false;
        }
        #endregion

        #region Remove
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
            Removed?.Invoke(this, item, index);
            Changed?.Invoke(this);
        }
        #endregion

        #region CopyTo
        public void CopyTo(T[] targetArray) => CopyTo(targetArray, 0, 0);
        public void CopyTo(T[] targetArray, int arrayIndex) => CopyTo(targetArray, arrayIndex, 0);
        public void CopyTo(T[] targetArray, int arrayIndex, int count) => Array.Copy(Items, 0, targetArray, arrayIndex, Count);
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
            for (int i = 0; i < Count; i++) {
                T current = Items[i];
                if (c.Equals(current, item))
                    return i;
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
            Changed?.Invoke(this);
        }

        public void Clear() {
            if (Items != EmptyArray)
                Items = EmptyArray;

            Count = 0;
            Changed?.Invoke(this);
        }
        public void Revers() { //bubble 
            for (int i = 0; i < Count / 2; i += 1) {
                T t1 = Items[i];
                T t2 = Items[Count - 1 - i];
                Items[i] = t2;
                Items[Count - 1 - i] = t1;
            }
            Changed?.Invoke(this);
        }

        #region Lambda/Linq
        #region ForEach
        public void ForEach(Action<T> action) {
            for (int i = 0; i < Count; i++) {
                action(Items[i]);
            }
        }
        public void ForEach(Action<T, int> action) {
            for (int i = 0; i < Count; i++) {
                action(Items[i], i);
            }
        }
        #endregion
        #region Select
        public IEnumerable<T1> Select<T1>(Func<T, T1> action) {
            for (int i = 0; i < Count; i++) {
                yield return action(Items[i]);
            }
        }
        public IEnumerable<T1> Select<T1>(Func<T, int, T1> action) {
            for (int i = 0; i < Count; i++) {
                yield return action(Items[i], i);
            }
        }
        #endregion
        #region Where
        public IEnumerable<T> Where(Func<T, bool> action) {
            for (int i = 0; i < Count; i++) {
                T current = Items[i];
                if (action(current))
                    yield return current;
            }
        }
        public IEnumerable<T> Where(Func<T, int, bool> action) {
            for (int i = 0; i < Count; i++) {
                T current = Items[i];
                if (action(current, i))
                    yield return current;
            }
        }
        #endregion
        #region FindLast
        public T WhereLast(Func<T, T, bool> compare) {
            T tmpItem = default;
            for (int i = 0; i < Count; i += 1) {
                T current = Items[i];
                if (tmpItem == null) {
                    tmpItem = current;
                } else {
                    tmpItem = Equals(tmpItem, current) ? current : tmpItem;
                    for (int subi = i + 1; subi < Count; subi += 1) {
                        tmpItem = Equals(tmpItem, current) ? current : tmpItem;
                    }
                }
            }
            return tmpItem;
        }
        public T FindLastOrDefault(Func<T, T, bool> compare) {
            T tmpItem = default;
            for (int i = 0; i < Count; i += 1) {
                T current = Items[i];
                if (compare(tmpItem, current)) {
                    tmpItem = current;
                }
                for (int subi = i + 1; subi < Count; subi += 1) {
                    if (compare(tmpItem, current)) {
                        tmpItem = current;
                    }
                }
            }
            return tmpItem;
        }
        #endregion

        #region Sum


#if Experimental
        private static Func<T[], T> expressionsum;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Sum() {
            if (expressionsum == null) {
                expressionsum = (items) => {
                    ParameterExpression arrayExpr = Expression.Parameter(typeof(T[]), "array");
                    ParameterExpression iExpr = Expression.Variable(typeof(int), "i");
                    ParameterExpression rExpr = Expression.Variable(typeof(T), "result");
                    LabelTarget breakLabel = Expression.Label(typeof(T));
                    Expression block = Expression.Block(
                        new[] { rExpr, iExpr },
                        Expression.Loop(
                            Expression.IfThenElse(
                                Expression.LessThan(iExpr, Expression.ArrayLength(arrayExpr)),
                                Expression.Block(
                                    Expression.Assign(rExpr, Expression.Add(rExpr, Expression.ArrayAccess(arrayExpr, iExpr))),
                                    Expression.Assign(iExpr, Expression.Increment(iExpr))),
                                Expression.Break(breakLabel, rExpr)
                            ),
                            breakLabel));
                    return Expression.Lambda<Func<T[], T>>(block, arrayExpr).Compile(false)(items);
                };
            }

            return expressionsum(Items); 
        }
#else
        public T Sum() {
            TypeCode code = Type.GetTypeCode(typeof(T));
            switch (code) {
                case TypeCode.Char:
                    Char valueChar = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueChar += (Char)(object)Items[i];
                    }
                    return (T)(object)valueChar;
                case TypeCode.SByte:
                    SByte valueSByte = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueSByte += (SByte)(object)Items[i];
                    }
                    return (T)(object)valueSByte;
                case TypeCode.Byte:
                    Byte valueByte = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueByte += (Byte)(object)Items[i];
                    }
                    return (T)(object)valueByte;
                case TypeCode.Int16:
                    Int16 valueInt16 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueInt16 += (Int16)(object)Items[i];
                    }
                    return (T)(object)valueInt16;
                case TypeCode.UInt16:
                    UInt16 valueUInt16 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueUInt16 += (UInt16)(object)Items[i];
                    }
                    return (T)(object)valueUInt16;
                case TypeCode.Int32:
                    Int32 valueInt32 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueInt32 += (Int32)(object)Items[i];
                    }
                    return (T)(object)valueInt32;
                case TypeCode.UInt32:
                    UInt32 valueUInt32 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueUInt32 += (UInt32)(object)Items[i];
                    }
                    return (T)(object)valueUInt32;
                case TypeCode.Int64:
                    Int64 valueInt64 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueInt64 += (Int64)(object)Items[i];
                    }
                    return (T)(object)valueInt64;

                case TypeCode.UInt64:
                    UInt64 valueUInt64 = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueUInt64 += (UInt64)(object)Items[i];
                    }
                    return (T)(object)valueUInt64;
                case TypeCode.Single:
                    Single valueSingle = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueSingle += (Single)(object)Items[i];
                    }
                    return (T)(object)valueSingle;
                case TypeCode.Double:
                    Double valueDouble = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueDouble += (Double)(object)Items[i];
                    }
                    return (T)(object)valueDouble;
                case TypeCode.Decimal:
                    Decimal valueDecimal = default;
                    for (int i = 0; i < Count; i += 1) {
                        valueDecimal += (Decimal)(object)Items[i];
                    }
                    return (T)(object)valueDecimal;
                default:
                    throw new NotSupportedException();
            }
        }
#endif
        #endregion

        #region Sort
        public void Sort() => Array.Sort(Items);
        public void Sort(IComparer<T> comparer) => Array.Sort(Items, comparer);
        #endregion
        #endregion

        #region Serialize
        public void SafeToFile(string path) => SafeToFile(new FileInfo(path));
        public void SafeToFile(FileInfo file) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file.Open(FileMode.CreateNew), this);
        }

        public void LoadFromFile(string path) => LoadFromFile(new FileInfo(path));
        public void LoadFromFile(FileInfo file) {
            if (!file.Exists)
                throw new FileNotFoundException();

            BinaryFormatter formatter = new BinaryFormatter();
            this = (PerformanceList<T>)formatter.Deserialize(file.OpenRead());
        }
        #endregion

        public T[] ToArray() => Items;

    }
}
