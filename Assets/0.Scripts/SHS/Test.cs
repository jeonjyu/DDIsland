using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM("Frozen_Window_Glow");

        Debug.Log(DataManager.Instance.FoodDatabase.FoodInfoData[50003].FoodName_String);
    }
}
