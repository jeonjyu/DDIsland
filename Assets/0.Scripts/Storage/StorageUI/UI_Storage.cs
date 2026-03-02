using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

enum SortMode
{
    Recent,
    GradeHigh,
    GradeLow,
    Name
}

public class UI_Storage : MonoBehaviour
{
    int _slotCount;    // 슬롯 총 개수(= StorageManager의 Capacity)
    UI_StorageSlot[] _fishSlots;  // 화면에 실제로 표시되는 슬롯 UI 배열 (풀링)

    [SerializeField] GameObject _slotPrefab;

    SortMode _currentSort;      // 현재 정렬 모드
    List<int> _viewIndices = new List<int>();   // 화면에 보여줄 realIndex 목록
    int _selectedRealIndex = -1;   // 마지막으로 선택한 realIndex(삭제 버튼 처리 등에 사용)

    // 상세 패널 UI
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private TextMeshProUGUI _bestPriceText;
    [SerializeField] private Image _detailIcon;

    [SerializeField] private Transform _slotParent;  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

    private void Start()
    {
        gameObject.SetActive(false);
        _slotCount = StorageManager.Instance.Capacity;
        _fishSlots = new UI_StorageSlot[_slotCount];

        _slotPrefab.SetActive(false);  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

        for (int i = 0; i < _slotCount; i++)
        {
            var hi = Instantiate(_slotPrefab, _slotParent, false);
            var slot = hi.GetComponent<UI_StorageSlot>();
            _fishSlots[i] = slot;  
            slot.Init(this);// 슬롯이 자기 부모(UI_Storage)
        }

        RefreshAll();
    }

    private void OnEnable()
    {
        if (StorageManager.Instance != null)
        {
            StorageManager.Instance.OnSlotChanged += UpdateSlot; // Storage 데이터가 바뀔 때마다 UI를 갱신하기 위해 이벤트 구독
            RefreshAll();
        }
    }

    private void OnDisable()
    {
        if (StorageManager.Instance != null)
        {
            StorageManager.Instance.OnSlotChanged -= UpdateSlot;
        }
    }

    public void FishSlotLayout()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            _fishSlots[i].gameObject.SetActive(true);
        }
    }

    void UpdateSlot(int index)
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        _viewIndices.Clear();

        // 1 실제 데이터 슬롯 중 값 있는 슬롯만 모음
        for (int real = 0; real < StorageManager.Instance.Capacity; real++)
        {
            var slot = StorageManager.Instance.GetSlot(real);
            if (slot.HasValue)
                _viewIndices.Add(real);
        }

        // 2 모아둔 realIndex 목록을 정렬
        _viewIndices.Sort(CompareByCurrentSort);

        // 3 UI 슬롯에 매핑 후 표시
        for (int ui = 0; ui < _slotCount; ui++)
        {
            if (ui < _viewIndices.Count)
            {
                int realIndex = _viewIndices[ui];

                _fishSlots[ui].gameObject.SetActive(true);
                _fishSlots[ui].BindRealIndex(realIndex); // 이 UI real 슬롯 저장
                _fishSlots[ui].Refresh(StorageManager.Instance.GetSlot(realIndex));  // 실제 슬롯 데이터를 UI에 반영
            }
            else
            {
                // 남는 UI칸은 빈칸으로
                _fishSlots[ui].BindRealIndex(-1);
                _fishSlots[ui].gameObject.SetActive(false);
            }
        }
    }

    public int CompareByCurrentSort(int a, int b)    // 현재 SortMode에 따른 비교 함수, a,b는 realIndex임!
    {
        var sa = StorageManager.Instance.GetSlot(a);
        var sb = StorageManager.Instance.GetSlot(b);

        if (!sa.HasValue && !sb.HasValue) return 0;
        if (!sa.HasValue) return 1;
        if (!sb.HasValue) return -1;

        // fishId로 정의 데이터(등급/이름 등)를 찾는다
        var defA = DataManager.Instance.FishingDatabase.FishData[sa.Value.FishId];
        var defB = DataManager.Instance.FishingDatabase.FishData[sb.Value.FishId];

        // def가 없으면 뒤로
        if (defA == null && defB == null) return 0;
        if (defA == null) return 1;
        if (defB == null) return -1;

        int primary = 0;

        switch (_currentSort)
        {
            case SortMode.Recent:  // 최근 획득
                primary = sb.Value.LastAcquiredOrder.CompareTo(sa.Value.LastAcquiredOrder);
                break;

            case SortMode.GradeHigh:
                {
                    primary = defB.gradeType.CompareTo(defA.gradeType); // 높은 등급 먼저
                }
                break;

            case SortMode.GradeLow:
                {
                   primary = defA.gradeType.CompareTo(defB.gradeType); // 낮은 등급 먼저
                }
                break;

            case SortMode.Name:  // 이름 오름차순
                primary = string.Compare(defA.FishName_String, defB.FishName_String, StringComparison.Ordinal);
                break;
        }

        if (primary != 0)
            return primary;

        // 기본 정렬(최근 획득)
        return sb.Value.LastAcquiredOrder.CompareTo(sa.Value.LastAcquiredOrder);
    }
   // public int SeasonToBit(Grade fish) // 등급 enum 플래그
   // {
   //     return fish switch 
   //     {
   //         fish. => 1,
   //         Season.Summer => 2,
   //         Season.Autumn => 4,
   //         Season.Winter => 8,
   //         _ => 0 
   //     }; 
   // }
    public void OnSlotClicked(int realIndex) // 슬롯 클릭(= realIndex를 전달받음)
    {
        _selectedRealIndex = realIndex;

        var slot = StorageManager.Instance.GetSlot(realIndex);
        if (!slot.HasValue) return;

        var def = DataManager.Instance.FishingDatabase.FishData[slot.Value.FishId];
        if (def == null) return;

        var sp = def.FishImgPath_Sprite;

        if (_detailIcon != null)
        {
            _detailIcon.gameObject.SetActive(true);
            _detailIcon.enabled = (sp != null);
            _detailIcon.sprite = sp;
        }

        string name = def.FishName_String;
        _nameText.text = name;

        var grade = def.gradeType;
        _gradeText.text = grade.ToString();

        int bestPrice = slot.Value.MaxPrice;
        _bestPriceText.text = bestPrice.ToString();
    }

    public void OnRemoveClicked()
    {
        if (_selectedRealIndex < 0) return;

        StorageManager.Instance.TryRemoveAt(_selectedRealIndex);
        _selectedRealIndex = -1;
    }

    // 정렬 버튼
    public void SetSortRecent()
    {
        _currentSort = SortMode.Recent;
        RefreshAll();
    }

    public void SetSortGradeHigh()
    {
        _currentSort = SortMode.GradeHigh;
        RefreshAll();
    }

    public void SetSortGradeLow()
    {
        _currentSort = SortMode.GradeLow;
        RefreshAll();
    }

    public void SetSortName()
    {
        _currentSort = SortMode.Name;
        RefreshAll();
    }

    public void ExitStorageUI()
    {
        gameObject.SetActive(false);
    }
}