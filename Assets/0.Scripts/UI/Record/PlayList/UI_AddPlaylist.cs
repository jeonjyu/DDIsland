using System.Collections.Generic;
using UnityEngine;

public class UI_AddPlaylist : MonoBehaviour
{
    [SerializeField] private UI_Playlist playlist;
    [SerializeField] private UI_AddPlaylistSlot slot;
    [SerializeField] private Transform slotTrans;

    private List<UI_AddPlaylistSlot> slotList = new List<UI_AddPlaylistSlot>();

    public RecordDataSO record;

    public void CreatePlaylistSlot(RecordDataSO record)
    {
        ClearSlotList();

        this.record = record;

        foreach (var slot in DataManager.Instance.RecordDatabase.PlaylistDatas)
        {
            UI_AddPlaylistSlot addSlot = Instantiate(this.slot, slotTrans);
            addSlot.PlaylistSlotInit(slot, record, playlist);
            slotList.Add(addSlot);
        }
    }

    private void ClearSlotList()
    {
        if (slotList.Count > 0)
        {
            foreach (var slot in slotList)
            {
                Destroy(slot.gameObject);
            }
        }
        slotList.Clear();
    }
}
