using System.Collections.Generic;

namespace FabmPerformance {
    public interface IPerformanceList<T> {
        T this[int index] { get; set; }

        bool IsReadOnly { get; }

        void Add(T item);
        void AddRange(params T[] source);
        void AddRange(IEnumerable<T> source);
        void AddRange(int index, IEnumerable<T> source);

        bool Contains(T item);
        bool Contains(T item, int start);
        bool Contains(T item, int start, int end);

        void Insert(int index, T item);

        bool Remove(T item);
        void Remove(int index);

        void CopyTo(T[] targetArray);
        void CopyTo(T[] targetArray, int arrayIndex);
        void CopyTo(T[] targetArray, int arrayIndex, int count);

        int IndexOf(T item);
        int IndexOf(T item, int start);
        int IndexOf(T item, int start, int end);

        void Clear();
        void Revers();
    }
}
