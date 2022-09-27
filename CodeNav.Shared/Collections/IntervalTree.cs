using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeNav.Shared.Collections.IntervalTree
{
    public class IntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    {
        private IntervalTreeNode<TKey, TValue> _root;
        private IList<RangeValuePair<TKey, TValue>> _items;
        private readonly IComparer<TKey> comparer;
        private bool _isInSync;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TKey Max
        {
            get
            {
                if (!_isInSync)
                    Rebuild();

                return _root.Max;
            }
        }

        public TKey Min
        {
            get
            {
                if (!_isInSync)
                    Rebuild();

                return _root.Min;
            }
        }

        public IEnumerable<TValue> Values => _items.Select(i => i.Value);

        public int Count => _items.Count;

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree() : this(Comparer<TKey>.Default) { }

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree(IComparer<TKey> comparer)
        {
            this.comparer = comparer ?? Comparer<TKey>.Default;
            _isInSync = true;
            _root = new IntervalTreeNode<TKey, TValue>(this.comparer);
            _items = new List<RangeValuePair<TKey, TValue>>();
        }

        public IntervalTree(IList<RangeValuePair<TKey, TValue>> items)
        {
            comparer = Comparer<TKey>.Default;
            _isInSync = false;
            _root = new IntervalTreeNode<TKey, TValue>(comparer);
            _items = items;
        }

        public IEnumerable<TValue> Query(TKey value)
        {
            if (!_isInSync)
                Rebuild();

            return _root.Query(value);
        }

        public IEnumerable<TValue> Query(TKey from, TKey to)
        {
            if (!_isInSync)
                Rebuild();

            return _root.Query(from, to);
        }

        public void Add(TKey from, TKey to, TValue value)
        {
            if (comparer.Compare(from, to) > 0)
                throw new ArgumentOutOfRangeException($"{nameof(from)} cannot be larger than {nameof(to)}");

            _isInSync = false;
            _items.Add(new RangeValuePair<TKey, TValue>(from, to, value));
        }

        public void Remove(TValue value)
        {
            _isInSync = false;
            _items = _items.Where(l => !l.Value.Equals(value)).ToList();
        }

        public void Remove(IEnumerable<TValue> items)
        {
            _isInSync = false;
            this._items = this._items.Where(l => !items.Contains(l.Value)).ToList();
        }

        public void Clear()
        {
            _root = new IntervalTreeNode<TKey, TValue>(comparer);
            _items = new List<RangeValuePair<TKey, TValue>>();
            _isInSync = true;
        }

        public IEnumerator<RangeValuePair<TKey, TValue>> GetEnumerator()
        {
            if (!_isInSync)
                Rebuild();

            return _items.GetEnumerator();
        }

        private void Rebuild()
        {
            if (_isInSync)
                return;

            if (_items.Count > 0)
                _root = new IntervalTreeNode<TKey, TValue>(_items, comparer);
            else
                _root = new IntervalTreeNode<TKey, TValue>(comparer);
            _isInSync = true;
        }
    }
}