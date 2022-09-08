using System;
using System.Collections;
#if !OPENSILVER
using System.Collections.ObjectModel;
#endif

namespace OpenSilver.Internal
{
    /// <summary>
    ///    This allows callers to "foreach" through a WeakReferenceList.
    ///    Each weakreference is checked for liveness and "current"
    ///    actually returns a strong reference to the current element.
    /// </summary>
    /// <remarks>
    ///    Due to the way enumerators function, this enumerator often
    ///    holds a cached strong reference to the "Current" element.
    ///    This should not be a problem unless the caller stops enumerating
    ///    before the end of the list AND holds the enumerator alive forever.
    /// </remarks>
    internal struct WeakReferenceListEnumerator : IEnumerator
    {
#if OPENSILVER
        public WeakReferenceListEnumerator(ArrayList List) 
#else //BRIDGE
        public WeakReferenceListEnumerator(ReadOnlyCollection<object> List)
#endif
        {
            _i = 0;
            _List = List;
            _StrongReference = null;
        }

        object IEnumerator.Current
        { get { return Current; } }

        public object Current
        {
            get
            {
                if (null == _StrongReference)
                {
#pragma warning suppress 6503
                    throw new InvalidOperationException("No current object to return.");
                }
                return _StrongReference;
            }
        }

        public bool MoveNext()
        {
            object obj = null;
            while (_i < _List.Count)
            {
                WeakReference weakRef = (WeakReference)_List[_i++];
                obj = weakRef.Target;
                if (null != obj)
                    break;
            }
            _StrongReference = obj;
            return (null != obj);
        }

        public void Reset()
        {
            _i = 0;
            _StrongReference = null;
        }

        int _i;
        ReadOnlyCollection<object> _List;
        object _StrongReference;
    }
}