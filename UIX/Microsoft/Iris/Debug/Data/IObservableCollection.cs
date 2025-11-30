using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Iris.Debug.Data;

public interface IObservableCollection<T> : ICollection<T>, INotifyCollectionChanged
{
}
