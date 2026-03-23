using System.Collections.Generic;
using UnityEngine;

public interface IStoreItemDatabase
{
    // TableDatabase의 datas
    List<IStoreItem> Items { get; set; }

    // TableDatabase의 아이템 get
    IStoreItem this[int id] { get; }
}
