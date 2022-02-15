// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionProviderCollection : ICollection<Type>
{
    private readonly HashSet<Type> _items = new();

    public RequestCompressionProviderCollection()
    {
    }

    public void Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
        where T : IRequestCompressionProvider
    {
        _items.Add(typeof(T));
    }

    public void Add([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (!typeof(IRequestCompressionProvider).IsAssignableFrom(item))
        {
            throw new ArgumentException($"The provider must implement {nameof(IRequestCompressionProvider)}.", nameof(item));
        }

        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains<T>()
        where T : IRequestCompressionProvider
    {
        return _items.Contains(typeof(T));
    }

    public bool Contains(Type item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _items.Contains(item);
    }

    void ICollection<Type>.CopyTo(Type[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        _items.CopyTo(array, arrayIndex);
    }

    public HashSet<Type>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public bool Remove<T>()
        where T : IRequestCompressionProvider
    {
        return _items.Remove(typeof(T));
    }

    public bool Remove(Type item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _items.Remove(item);
    }

    public void TrimExcess()
    {
        _items.TrimExcess();
    }

    public int Count
    {
        get
        {
            return _items.Count;
        }
    }

    bool ICollection<Type>.IsReadOnly
    {
        get
        {
            return false;
        }
    }
}
