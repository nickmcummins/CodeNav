using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeNav.Languages.Collections
{
    public class IntervalTree<TKey, TValue> : IIntervalTree<TKey, TValue>
    {
        private IntervalTreeNode<TKey, TValue> _root;
        private IList<RangeValuePair<TKey, TValue>> _rangeValuePairs;
        private readonly IComparer<TKey> _comparer;
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

        public IEnumerable<TValue> Values => _rangeValuePairs.Select(i => i.Value);

        public int Count => _rangeValuePairs.Count;

        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree() : this(Comparer<TKey>.Default) { }
        public IntervalTree(IList<RangeValuePair<TKey, TValue>> rangeValuePairs) : this(Comparer<TKey>.Default, rangeValuePairs) { }
        /// <summary>
        /// Initializes an empty tree.
        /// </summary>
        public IntervalTree(IComparer<TKey> comparer, IList<RangeValuePair<TKey, TValue>> rangeValuePairs = null)
        {
            _comparer = comparer ?? Comparer<TKey>.Default;
            _isInSync = true;
            _root = new IntervalTreeNode<TKey, TValue>(this._comparer);
            _rangeValuePairs = rangeValuePairs ?? new List<RangeValuePair<TKey, TValue>>();
            if (_rangeValuePairs.Count > 0)
            {
                _isInSync = false;
            }

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
            if (_comparer.Compare(from, to) > 0)
                throw new ArgumentOutOfRangeException($"{nameof(from)} cannot be larger than {nameof(to)}");

            _isInSync = false;
            _rangeValuePairs.Add(new RangeValuePair<TKey, TValue>(from, to, value));
        }

        public void Remove(TValue value)
        {
            _isInSync = false;
            _rangeValuePairs = _rangeValuePairs.Where(l => !l.Value.Equals(value)).ToList();
        }

        public void Remove(IEnumerable<TValue> items)
        {
            _isInSync = false;
            this._rangeValuePairs = this._rangeValuePairs.Where(l => !items.Contains(l.Value)).ToList();
        }

        public void Clear()
        {
            _root = new IntervalTreeNode<TKey, TValue>(_comparer);
            _rangeValuePairs = new List<RangeValuePair<TKey, TValue>>();
            _isInSync = true;
        }

        public IEnumerator<RangeValuePair<TKey, TValue>> GetEnumerator()
        {
            if (!_isInSync)
                Rebuild();

            return _rangeValuePairs.GetEnumerator();
        }

        private void Rebuild()
        {
            if (_isInSync)
                return;

            if (_rangeValuePairs.Count > 0)
                _root = new IntervalTreeNode<TKey, TValue>(_rangeValuePairs, _comparer);
            else
                _root = new IntervalTreeNode<TKey, TValue>(_comparer);
            _isInSync = true;
        }
    }
}
