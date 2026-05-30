using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace megastar.Game.Utils;

public class PeekableEnumerator<T> : IEnumerator<T>
{
    public T Current { get; private set; }
    public bool HasNext { get; private set; }

    private readonly IEnumerator<T> enumerator;

    public PeekableEnumerator(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator;
        MoveNext();
    }


    public bool MoveNext()
    {
        //if (!HasNext) return false;
        Current = enumerator.Current;
        HasNext = enumerator.MoveNext();
        return true;
    }

    public void Reset() => throw new System.NotImplementedException();

    public T Peek => enumerator.Current;

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
        enumerator.Dispose();
    }
}

public static class PeekableEnumeratorExtension
{
    public static PeekableEnumerator<T> AsPeekable<T>(this IEnumerable<T> enumerable) =>
        new(enumerable.GetEnumerator());

    public static PeekableEnumerator<T> AsPeekable<T>(this IEnumerator<T> enumerable) =>
        new(enumerable);
}
