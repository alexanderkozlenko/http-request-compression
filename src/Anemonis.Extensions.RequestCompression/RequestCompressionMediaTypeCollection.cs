// © Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CS1591

using System.Collections;

namespace Anemonis.Extensions.RequestCompression;

public sealed class RequestCompressionMediaTypeCollection : ICollection<string>
{
    private readonly HashSet<string> _items;

    public RequestCompressionMediaTypeCollection()
    {
        _items = new(StringComparer.OrdinalIgnoreCase);
    }

    public RequestCompressionMediaTypeCollection(IEnumerable<string> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        _items = new(collection, StringComparer.OrdinalIgnoreCase);
    }

    public void Add(string item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(string item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _items.Contains(item);
    }

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        _items.CopyTo(array, arrayIndex);
    }

    public HashSet<string>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public bool Remove(string item)
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

    bool ICollection<string>.IsReadOnly
    {
        get
        {
            return false;
        }
    }
}
