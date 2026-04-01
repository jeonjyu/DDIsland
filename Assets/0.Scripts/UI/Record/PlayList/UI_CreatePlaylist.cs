using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class UI_CreatePlaylist : MonoBehaviour
{
    // 정규식 사용, 1. 특수문자 제외 / 2. 한글 1 ~ 10글자 / 3. 영어 1 ~ 20글자
    private const string NamePattern = @"^[a-zA-Z0-9가-힣]*$";

    [Header("플레이리스트 제목 인풋필드")]
    [SerializeField] private TMP_InputField titleInputField;

    [Header("플레이리스트 생성 불가 안내 오브젝트")]
    [SerializeField] private GameObject failureObj;

    public void OnClick_CreatePlaylist()
    {
        if(IsValidPlaylistName(titleInputField.text))
        {
            
        }
        else
        {
            failureObj.SetActive(true);
        }
    }

    private bool IsValidPlaylistName(string input)
    {
        if (!Regex.IsMatch(input, NamePattern)) return false;

        int length = 0;

        foreach(var c in input)
        {
            length += (c >= '\uAC00' && c <= '\uD7A3') ? 2 : 1;
        }

        return length <= 20;
    }
}
