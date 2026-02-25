using System;

public class StoreItemBaseSO<T> : StoreItem where T : System.Enum
{
    public T filter;

    public override Enum GetFilter() => filter;
}
