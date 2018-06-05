using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace VirtualizationTest
{
    class ParticalList<T> : 
        IList,
        IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        #region member
        private int _count = -1;
        private int _partialListLength = 100;
        private int _partialListLifeCount = 3000;
        private bool _isPending; 

        private SynchronizationContext _syncCntxt;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<int, IList<T>> _partialLists = new Dictionary<int,IList<T>>();
        private readonly Dictionary<int, DateTime> _partialListCallTimes = new Dictionary<int, DateTime>();

        #endregion


        #region properties
        public IListMaker<T> _listMaker { get; private set; }

        public bool IsPending
        {
            get
            {
                return _isPending;
            }
            set
            {
                if (value != _isPending) _isPending = value;
                NotiPropertyChanged("IsPending");
            }
        }

        //public SynchronizationContext _syncCntxt { get; set; }

        #endregion

        #region constructor
        public ParticalList(IListMaker<T> listMaker)
        {
            _listMaker = listMaker;
            _syncCntxt = SynchronizationContext.Current;
        }

        public ParticalList(IListMaker<T> listMaker, int partialListLength)
        {
            _listMaker = listMaker;
            _partialListLength = partialListLength;
            _syncCntxt = SynchronizationContext.Current;
        }

        public ParticalList(IListMaker<T> listMaker, int partialListLength, int partialListLifeCount)
        {
            _listMaker = listMaker;
            _partialListLength = partialListLength;
            _partialListLifeCount = partialListLifeCount;
            _syncCntxt = SynchronizationContext.Current;
        } 
        #endregion

        #region iLst implementation
        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                int _partialListIdx = index / _partialListLength;
                int _partialListRemain = index % _partialListLength;

                GetPartialList(_partialListIdx);

                if (_partialListRemain > _partialListLength / 2
                    && _partialListIdx < Count / _partialListLength)
                {
                    GetPartialList(_partialListIdx + 1);
                }
                
                if (_partialListRemain < _partialListLength / 2 && _partialListIdx > 0)
                {
                    GetPartialList(_partialListIdx - 1);
                }

                DisposePartialList();

                if (_partialLists[_partialListIdx] == null) return default(T);

                return _partialLists[_partialListIdx][_partialListRemain];
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                if (_count == -1) GetCount();
                return _count;
            }
            protected set { _count = value; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region others

        private void DisposePartialList()
        {
            List<int> _tmpKeys = new List<int>(_partialListCallTimes.Keys);
            foreach (int _tmpKey in _tmpKeys)
            {
                // page 0 is a special case, since WPF ItemsControl access the first item frequently
                if (_tmpKey != 0
                    && (DateTime.Now - _partialListCallTimes[_tmpKey]).TotalMilliseconds > _partialListLifeCount)
                {
                    _partialLists.Remove(_tmpKey);
                    _partialListCallTimes.Remove(_tmpKey);
                    Trace.WriteLine("Removed Page: " + _tmpKey);
                }
            }
        }

        private void GetPartialList(int partialListIdx)
        {
            if (!_partialLists.ContainsKey(partialListIdx))
            {
                _partialLists.Add(partialListIdx, null);
                _partialListCallTimes.Add(partialListIdx, DateTime.Now);
                Trace.WriteLine("appended list : " + partialListIdx);

                IsPending = true;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    _syncCntxt.Send(o =>
                    {
                        int _tmpIdx = (int)o;
                        Trace.WriteLine("List page displayed: " + _tmpIdx);
                        if (_partialLists.ContainsKey(_tmpIdx))
                        { 
                            _partialLists[_tmpIdx] =
                                _listMaker.getAvailableRows( 
                                    _tmpIdx * _partialListLength
                                    , _partialListLength );
                        }
                        IsPending = false;
                        NotiCollectionChanged();
                    }, x
                    );
                }
                , partialListIdx);
            }
            else
            {
                _partialListCallTimes[partialListIdx] = DateTime.Now;
            }
        }

        private void GetCount()
        {
            Count = 0;
            IsPending = true;
            ThreadPool.QueueUserWorkItem(x =>
            {
                int _cnt = _listMaker.getAvailableRowsCount();
                _syncCntxt.Send( o => { 
                    Count = (int) o ;
                    IsPending = false;
                    NotiCollectionChanged(); 
                } 
                , _cnt);
            });
        }

        private void NotiCollectionChanged()
        {
            NotifyCollectionChangedEventArgs e
                = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

            NotifyCollectionChangedEventHandler hdlr = CollectionChanged;
            if ( hdlr != null ) hdlr(this, e);
        }

        private void NotiPropertyChanged(string propName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName: propName);

            PropertyChangedEventHandler hdlr = PropertyChanged;
            if (hdlr != null) hdlr(this, e);
        }

        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
