using System.Collections.Generic;
using UnityEngine;
public enum FixGroup
{
    None = 0,       
    House = 1,      
    Box = 2,        
    LpPlayer = 3,   
    Bed = 4         
}
public class FixedBuilding : MonoBehaviour
{
        [Header("고유 식별 번호 (예: 1번 자리 집, 2번 자리 헛간)")]
        public int LocationID;

        [Header("현재 적용된 아이템 ID")]
        public int CurrentItemID;

        // 초기화용 메서드
        public void Setup(int locationId, int itemId)
        {
            LocationID = locationId;
            CurrentItemID = itemId;
        }
    public void OpenSwapMenu(int clickedItemId)
    {
        var database = DataManager.Instance.DecorationDatabase.InteriorData;

        // 1. 클릭한 건물의 데이터 가져오기
        var clickedData = database[clickedItemId];
        //FixGroup currentGroup = clickedData.FixGroup; // (SO에 FixGroup 필드가 있다고 가정)

        // 만약 그룹이 없는 녀석이라면 교체 UI를 띄우지 않음
        //if (currentGroup == FixGroup.None) return;

        // 2. [핵심] 데이터베이스 전체를 뒤져서 '같은 FixGroup'을 가진 애들만 뽑아오기
        ////List<InteriorDataSO> swapOptions = database.Values
        //    .Where(data => data.FixGroup == currentGroup) // 같은 그룹(예: House)만 필터링
        //    .OrderBy(data => data.InteriorID)             // ID 순서대로 예쁘게 정렬
        //    .ToList();

        // 3. 뽑아온 리스트(swapOptions)를 바탕으로 UI 버튼들 생성!
        //foreach (var option in swapOptions)
        {
            //Debug.Log($"UI 버튼 생성 대상: {option.InteriorName_String} (ID: {option.InteriorID})");
            // TODO: 실제 화면에 버튼 프리팹을 하나씩 생성하고, 
            // 그 버튼을 누르면 BuildingManager.SwapFixBuilding(..., option.InteriorID) 가 실행되게 연결
        }

        // UI 창 켜기
        gameObject.SetActive(true);
    }
}
