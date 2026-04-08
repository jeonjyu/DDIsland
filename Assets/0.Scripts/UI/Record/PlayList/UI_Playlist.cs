using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Playlist : MonoBehaviour
{
    // 정규식 사용, 1. 특수문자 제외 / 2. 한글 1 ~ 10글자 / 3. 영어 1 ~ 20글자
    private const string NamePattern = @"^[a-zA-Z0-9가-힣]*$";

    [Header("플레이리스트 제목 인풋필드")]
    [SerializeField] private TMP_InputField titleInputField;    // 플레이리스트 생성 인풋필드
    [SerializeField] private TMP_InputField titleChangeInputField;    // 플레이리스트 이름 변경 인풋필드

    [Header("플레이리스트 생성 불가 안내 오브젝트")]
    [SerializeField] private GameObject failureObj;

    [Header("생성할 플레이리스트 프리팹")]
    [SerializeField] private UI_PlaylistSlot playlistSlot;
    [SerializeField] private Transform slotTrans;
    [SerializeField] private UI_AddPlaylist addPlaylist;

    [SerializeField] private UI_CurrentPlaylist currentPlaylist;
    [SerializeField] private UI_EditPlaylist editPlaylist;

    public List<UI_PlaylistSlot> slotList = new List<UI_PlaylistSlot>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        List<PlaylistData> datas = DataManager.Instance.RecordDatabase.PlaylistDatas;

        if(datas.Count > 0)
        {
            for(int i = 0; i < datas.Count; i++)
            {
                UI_PlaylistSlot slot = Instantiate(playlistSlot, slotTrans);
                slot.PlaylistSlotInit(datas[i], this);
                slotList.Add(slot);
            }
        }
    }

    public void OnClick_CreatePlaylist()
    {
        if (IsValidPlaylistName(titleInputField.text))
        {
            UI_PlaylistSlot slot = Instantiate(playlistSlot, slotTrans);
            slot.PlaylistSlotInit(titleInputField.text, this);
            slotList.Add(slot);

            if(addPlaylist.record != null)
            {
                addPlaylist.CreatePlaylistSlot(addPlaylist.record);
            }
        }
        else
        {
            failureObj.SetActive(true);
        }

        titleInputField.text = string.Empty;
    }

    private bool IsValidPlaylistName(string input)
    {
        if (!Regex.IsMatch(input, NamePattern)) return false;

        int length = 0;

        foreach (var c in input)
        {
            length += (c >= '\uAC00' && c <= '\uD7A3') ? 2 : 1;
        }

        return length <= 20;
    }

    public void ShowCurrentPlaylist(PlaylistData data)
    {
        currentPlaylist.gameObject.SetActive(true);
        currentPlaylist.ShowCurrentPlaylist(data);
    }

    public void ChangePlaylistName(PlaylistData data)
    {
        if(IsValidPlaylistName(titleChangeInputField.text))
        {
            data.Name = titleChangeInputField.text;
            slotList[slotList.FindIndex(x => x.Playlist == data)].PlaylistSlotInit(data, this);
            editPlaylist.SetEditInit(data);
        }
        else
        {
            failureObj.SetActive(true);
        }

        titleChangeInputField.text = string.Empty;
    }

    public void OnClick_EditPlaylist(PlaylistData data)
    {
        editPlaylist.SetEditInit(data);
        editPlaylist.gameObject.SetActive(true);
    }
}
