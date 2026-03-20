using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] private GameObject _slotPrefab;

    [SerializeField] private GameObject _UpgradePan;
    [SerializeField] private TextMeshProUGUI _storageLevelText;
    [SerializeField] private TextMeshProUGUI _systemMsgText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TextMeshProUGUI _upgradeButtonLabel;
    private Coroutine _msgRoutine;


    SortMode _currentSort;      // 현재 정렬 모드
    List<int> _viewIndices = new List<int>();   // 화면에 보여줄 realIndex 목록
    int _selectedRealIndex = -1;   // 마지막으로 선택한 realIndex(삭제 버튼 처리 등에 사용)

    // 상세 패널 UI
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _gradeText;
    //[SerializeField] private TextMeshProUGUI _bestPriceText;
    [SerializeField] private TextMeshProUGUI _DescText;
    [SerializeField] private Image _detailIcon;

    [SerializeField] private Transform _slotParent;  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

    [SerializeField] private TMP_Dropdown _sortDropdown;

    [SerializeField] private GameObject _fishStorageUI;
    [SerializeField] private GameObject _foodStorageUI;

    [SerializeField] private Button _ofFishFishButton;
    [SerializeField] private Button _ofFishFoodButton;
    [SerializeField] private Button _ofFoodFishButton;
    [SerializeField] private Button _ofFoodFoodButton;

    private void Start()
    {
        _slotPrefab.SetActive(false);  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

        SlotPool();
        if (_sortDropdown != null)
        {
            _sortDropdown.onValueChanged.RemoveListener(OnSortDropdownChanged);
            _sortDropdown.onValueChanged.AddListener(OnSortDropdownChanged);

            // 초기값 반영 (드롭다운 현재 선택 기준)
            OnSortDropdownChanged(_sortDropdown.value);
        }
        else
        {
            RefreshAll();
        }
    }

    private void OnEnable()
    {
        if (FishStorageManager.Instance != null)
        {
            FishStorageManager.Instance.OnSlotChanged += UpdateSlot; // Storage 데이터가 바뀔 때마다 UI를 갱신하기 위해 이벤트 구독
            RefreshAll();
            RefreshUpgradeUI();
        }
    }

    private void OnDisable()
    {
        if (FishStorageManager.Instance != null)
        {
            FishStorageManager.Instance.OnSlotChanged -= UpdateSlot;
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
        for (int real = 0; real < FishStorageManager.Instance.Capacity; real++)
        {
            var slot = FishStorageManager.Instance.GetSlot(real);
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
                foreach (Transform child in _fishSlots[ui].transform)
                {
                   child.gameObject.SetActive(true);
                }
                _fishSlots[ui].BindRealIndex(realIndex); // 이 UI real 슬롯 저장
                _fishSlots[ui].Refresh(FishStorageManager.Instance.GetSlot(realIndex));  // 실제 슬롯 데이터를 UI에 반영
            }
            else
            {
                // 남는 UI칸은 빈칸으로
                _fishSlots[ui].BindRealIndex(-1);
                _fishSlots[ui].gameObject.SetActive(true);
                _fishSlots[ui].SetEmpty();
            }
        }
        RefreshSelectedDetail();
    }

    public int CompareByCurrentSort(int a, int b)    // 현재 SortMode에 따른 비교 함수, a,b는 realIndex임!
    {
        var sa = FishStorageManager.Instance.GetSlot(a);
        var sb = FishStorageManager.Instance.GetSlot(b);

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

        var slot = FishStorageManager.Instance.GetSlot(realIndex);
        if (!slot.HasValue) return;

        var def = DataManager.Instance.FishingDatabase.FishData[slot.Value.FishId];
        if (def == null) return;

        RefreshDetailFish(def, slot);
    }

    public void RefreshDetailFish(FishDataSO def, FishStackSlot? slot)  //슬롯 내용물없으면 디테일패널을 비우고 있으면 넣기
    {
        if (!slot.HasValue || def == null)
        {
            ClearDetailFish();
            return;
        }

        var sp = def.FishImgPath_Sprite;

        if (_detailIcon != null)
        {
            _detailIcon.gameObject.SetActive(true);
            _detailIcon.enabled = (sp != null);
            _detailIcon.sprite = sp;
        }

        _nameText.text = def.FishName_String;
        _gradeText.text = def.gradeType.ToString();
        _DescText.text = def.FishDesc_String;
    }
    private void RefreshSelectedDetail()
    {
        if (_selectedRealIndex < 0)
        {
            ClearDetailFish();
            return;
        }

        var slot = FishStorageManager.Instance.GetSlot(_selectedRealIndex);
        if (!slot.HasValue)
        {
            _selectedRealIndex = -1;
            ClearDetailFish();
            return;
        }

        var def = DataManager.Instance.FishingDatabase.FishData[slot.Value.FishId];
        if (def == null)
        {
            _selectedRealIndex = -1;
            ClearDetailFish();
            return;
        }

        RefreshDetailFish(def, slot);
    }
    private void ClearDetailFish()
    {
        if (_detailIcon != null)
        {
            _detailIcon.sprite = null;
            _detailIcon.enabled = false;
        }

        if (_nameText != null)
            _nameText.text = string.Empty;

        if (_gradeText != null)
            _gradeText.text = string.Empty;

        if (_DescText != null)
            _DescText.text = string.Empty;
    }
    public void OnRemoveClicked()
    {
        if (_selectedRealIndex < 0) return;

        FishStorageManager.Instance.TryRemoveAt(_selectedRealIndex);
        _selectedRealIndex = -1;
        ClearDetailFish();
    }
    public void UpgradeStorage()
    {
        //이미 최대치면: 버튼/텍스트 최신상태로 갱신 + 안내 메시지 띄우고 끝
        if (FishStorageManager.Instance.StorageLevel >= FishStorageManager.MaxLevel ||
        FishStorageManager.Instance.Capacity >= FishStorageManager.MaxCapacity)
        {
            RefreshUpgradeUI();
            ShowSystemMessage("최대 확장 완료 상태입니다.");
            return;
        }
        if (!FishStorageManager.Instance.PayStorageUpgrade())  //돈체크
        {
            ShowSystemMessage("코인이 부족합니다");
            return;
        }
        FishStorageManager.Instance.UpgradeStorageindex(); // 실제 데이터(슬롯 배열) 확장
        SlotPool();  
        RefreshUpgradeUI();
        ShowSystemMessage($"창고가 Lv.{FishStorageManager.Instance.StorageLevel}로 확장되었습니다! (총 {FishStorageManager.Instance.Capacity}칸)");
        RefreshAll();
    }
    private void SlotPool()  //UI 슬롯 풀(프리팹 복제)도 데이터 Capacity 만큼 늘려줌
    {
        int cap = FishStorageManager.Instance.Capacity;

        // 이미 충분하면 끝
        if (_fishSlots != null && _fishSlots.Length >= cap) return;

        // 기존 슬롯 유지하면서 더 큰 배열로 확장
        int old = _fishSlots == null ? 0 : _fishSlots.Length;
        Array.Resize(ref _fishSlots, cap);

        for (int i = old; i < cap; i++) 
        {
            var go = Instantiate(_slotPrefab, _slotParent, false);
            var slot = go.GetComponent<UI_StorageSlot>();
            _fishSlots[i] = slot;
            slot.Init(this);    //슬롯이 클릭했을 때 UI_Storage로 콜백할 수 있게 초기화
        }
        _slotCount = cap;  //RefreshAll() 루프에서 사용할 UI 슬롯 개수 최신화
    }
    private void RefreshUpgradeUI()  //업그레이드 UI 텍스트/버튼 상태 갱신
    {
        var sm = FishStorageManager.Instance;
        if (sm == null) return;

        if (_storageLevelText != null)
            _storageLevelText.text = $"Lv.{sm.StorageLevel} / Lv.{FishStorageManager.MaxLevel}";

        bool isMax = false;     //최대치인지 판단

        if (sm.StorageLevel >= FishStorageManager.MaxLevel || sm.Capacity >= FishStorageManager.MaxCapacity)
        {
            isMax = true;
        }

        if (_upgradeButton != null)
            _upgradeButton.interactable = !isMax;

        if (_upgradeButtonLabel != null)     //최대치면 버튼 글자 변경
            _upgradeButtonLabel.text = isMax ? "최대 확장 완료" : "확장하기";
    }
    private void ShowSystemMessage(string msg)    //시스템 알림 출력
    {
        if (_systemMsgText == null) return;

        _systemMsgText.gameObject.SetActive(true);
        _systemMsgText.text = msg;

        if (_msgRoutine != null) StopCoroutine(_msgRoutine);   
        _msgRoutine = StartCoroutine(HideMsgAfterSeconds(2f));
    }

    private IEnumerator HideMsgAfterSeconds(float sec)   
    {
        yield return new WaitForSeconds(sec);     //지정 시간 후 메시지 숨김
        if (_systemMsgText != null) _systemMsgText.gameObject.SetActive(false);
        _msgRoutine = null;
    }

    public void OpenUpgradeUI()
    {
        _UpgradePan.gameObject.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        _UpgradePan.gameObject.SetActive(false);
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
    private void OnDestroy()
    {
        if (_sortDropdown != null)
            _sortDropdown.onValueChanged.RemoveListener(OnSortDropdownChanged);
    }
    public void OnSortDropdownChanged(int value)
    {
        _currentSort = (SortMode)value;
        RefreshAll();
    }
    public void ExitStorageUI()
    {
        gameObject.SetActive(false);
    }
    public void OpenStorageUI()
    {
        gameObject.SetActive(true);
    }
    public void OpenFishStorage() 
    {
        Debug.Log("물고기창고 버튼눌림" + EventSystem.current.currentSelectedGameObject?.name);
        _fishStorageUI.SetActive(true);
        _foodStorageUI.SetActive(false);
        
        _ofFishFishButton.interactable = false;
        _ofFishFoodButton.interactable = true;
    }
    public void OpenFoodStorage()
    {
        Debug.Log("음식창고 버튼눌림" + EventSystem.current.currentSelectedGameObject?.name);
        _fishStorageUI.SetActive(false);
        _foodStorageUI.SetActive(true);

        _ofFoodFishButton.interactable = true;
        _ofFoodFoodButton.interactable = false;
    }
}