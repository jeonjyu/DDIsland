using System;
using System.Collections.Generic;
using System.Linq;

// 정렬 기준 체이닝하는 클래스
public static class EnumerableSorterBuilder
{
    // 오름차순
    public static IOrderedEnumerable<T> AppendOrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> func)
    {
        // 이전에 정렬된 건지 확인
        if (source is IOrderedEnumerable<T>)
            return ((IOrderedEnumerable<T>)source).ThenBy(func);
        else
            return source.OrderBy(func);
    }

    // 내림차순
    public static IOrderedEnumerable<T> AppendOrderByDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> func)
    {
        // 이전에 정렬된 건지 확인
        if (source is IOrderedEnumerable<T>)
            return ((IOrderedEnumerable<T>)source).ThenByDescending(func);
        else
            return source.OrderByDescending(func);
    }
}