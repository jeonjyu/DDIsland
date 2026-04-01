using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
enum FoodSortMode
{
    Recent,
    GradeHigh,
    GradeLow,
    Name
}
public class UI_FoodStorage : MonoBehaviour
{
    int _slotCount;
    UI_FoodStorageSlot[] _foodSlots; 

    [SerializeField] private GameObject _slotPrefab;

    [SerializeField] private GameObject _UpgradePan;
    [SerializeField] private TMP_Text _storageLevelText;
    [SerializeField] private TMP_Text _systemMsgText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private TMP_Text _upgradeButtonLabel;
    [SerializeField] private TMP_Text _upgradeCostText;
    private Coroutine _msgRoutine;

    FoodSortMode _currentSort;      // 현재 정렬 모드
    List<int> _viewIndices = new List<int>();   // 화면에 보여줄 realIndex 목록
    public int _selectedRealIndex = -1;   // 마지막으로 선택한 realIndex(삭제 버튼 처리 등에 사용)

    // 상세 패널 UI
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _gradeText;
    [SerializeField] private TMP_Text _DescText;
    [SerializeField] private Image _detailIcon;

    [SerializeField] private Transform _slotParent;  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

    [SerializeField] private TMP_Dropdown _sortDropdown;

    [SerializeField] private AudioClip _TabButtonSFX;
    [SerializeField] private AudioClip _CloseButtonSFX;
    [SerializeField] private AudioClip _UpgradeSFX;
    [SerializeField] private AudioClip _ButtonSFX;

    private void Start()
    {
        _slotPrefab.SetActive(false);  // 슬롯 프리팹(원본은 꺼두고 복제해서 사용)

        FoodSlotPool();
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
        if (FoodStorageManager.Instance != null)
        {
            FoodStorageManager.Instance.OnSlotChanged += UpdateFoodSlot; // Storage 데이터가 바뀔 때마다 UI를 갱신하기 위해 이벤트 구독
            RefreshAll();
            RefreshFoodUpgradeUI();
        }
    }

    private void OnDisable()
    {
        if (FoodStorageManager.Instance != null)
        {
            FoodStorageManager.Instance.OnSlotChanged -= UpdateFoodSlot;
        }
    }
    public void FoodSlotLayout()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            _foodSlots[i].gameObject.SetActive(true);
        }
    }

    void UpdateFoodSlot(int index)
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        _viewIndices.Clear();

        // 1 실제 데이터 슬롯 중 값 있는 슬롯만 모음
        for (int real = 0; real < FoodStorageManager.Instance.Capacity; real++)
        {
            var slot = FoodStorageManager.Instance.GetFoodSlot(real);
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

                _foodSlots[ui].gameObject.SetActive(true);
                foreach (Transform child in _foodSlots[ui].transform)
                {
                    child.gameObject.SetActive(true);
                }
                _foodSlots[ui].BindRealIndex(realIndex); // 이 UI real 슬롯 저장
                _foodSlots[ui].FoodRefresh(FoodStorageManager.Instance.GetFoodSlot(realIndex));  // 실제 슬롯 데이터를 UI에 반영
            }
            else
            {
                // 남는 UI칸은 빈칸으로
                _foodSlots[ui].BindRealIndex(-1);
                _foodSlots[ui].gameObject.SetActive(true);
                _foodSlots[ui].SetEmpty();
            }
        }
        RefreshSelectedDetail();
    }

    public int CompareByCurrentSort(int a, int b)  
    {
        var sa = FoodStorageManager.Instance.GetFoodSlot(a);
        var sb = FoodStorageManager.Instance.GetFoodSlot(b);

        if (!sa.HasValue && !sb.HasValue) return 0;
        if (!sa.HasValue) return 1;
        if (!sb.HasValue) return -1;

 
        var defA = DataManager.Instance.FoodDatabase.FoodInfoData[sa.Value.FoodId];
        var defB = DataManager.Instance.FoodDatabase.FoodInfoData[sb.Value.FoodId];

        // def가 없으면 뒤로
        if (defA == null && defB == null) return 0;
        if (defA == null) return 1;
        if (defB == null) return -1;

        int primary = 0;

        switch (_currentSort)
        {
            case FoodSortMode.Recent:  // 최근 획득
                primary = sb.Value.LastAcquiredOrder.CompareTo(sa.Value.LastAcquiredOrder);
                break;

            case FoodSortMode.GradeHigh:
                {
                    primary = defB.foodrateType.CompareTo(defA.foodrateType); // 높은 등급 먼저
                }
                break;

            case FoodSortMode.GradeLow:
                {
                    primary = defA.foodrateType.CompareTo(defB.foodrateType); // 낮은 등급 먼저
                }
                break;

            case FoodSortMode.Name:  // 이름 오름차순
                primary = string.Compare(defA.FoodName_String, defB.FoodName_String, StringComparison.Ordinal);
                break;
        }

        if (primary != 0)
            return primary;

        // 기본 정렬(최근 획득)
        return sb.Value.LastAcquiredOrder.CompareTo(sa.Value.LastAcquiredOrder);
    }

    public void OnSlotClicked(int realIndex) // 슬롯 클릭(= realIndex를 전달받음)
    {
        _selectedRealIndex = realIndex;

        var slot = FoodStorageManager.Instance.GetFoodSlot(realIndex);
        if (!slot.HasValue) return;

        var def = DataManager.Instance.FoodDatabase.FoodInfoData[slot.Value.FoodId];
        if (def == null) return;

        RefreshDetailFood(def, slot);
    }
    public void RefreshDetailFood(FoodDataSO def, FoodStackSlot? slot)
    {
        if (!slot.HasValue || def == null)
        {
            ClearDetailFood();
            return;
        }

        var sp = def.FoodImgPath_Sprite;

        if (_detailIcon != null)
        {
            _detailIcon.gameObject.SetActive(true);
            _detailIcon.enabled = (sp != null);
            _detailIcon.sprite = sp;
        }

        _nameText.text = def.FoodName_String;
        _gradeText.text = def.foodrateType.ToString();
        _DescText.text = def.FoodDesc_String;
    }
    private void RefreshSelectedDetail()
    {
        if (_selectedRealIndex < 0)
        {
            ClearDetailFood();
            return;
        }

        var slot = FoodStorageManager.Instance.GetFoodSlot(_selectedRealIndex);
        if (!slot.HasValue)
        {
            _selectedRealIndex = -1;
            ClearDetailFood();
            return;
        }

        var def = DataManager.Instance.FoodDatabase.FoodInfoData[slot.Value.FoodId];
        if (def == null)
        {
            _selectedRealIndex = -1;
            ClearDetailFood();
            return;
        }

        RefreshDetailFood(def, slot);
    }
    private void ClearDetailFood()
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

        FoodStorageManager.Instance.TryFoodRemoveAt(_selectedRealIndex);
        _selectedRealIndex = -1;
    }
    public void UpgradeFoodStorage()
    {
        //이미 최대치면: 버튼/텍스트 최신상태로 갱신 + 안내 메시지 띄우고 끝
        if (FoodStorageManager.Instance.StorageLevel >= FoodStorageManager.MaxLevel ||
        FoodStorageManager.Instance.Capacity >= FoodStorageManager.MaxCapacity)
        {
            RefreshFoodUpgradeUI();
            ShowSystemMessage(LocalizationManager.Instance.GetString("InteriorBoxMaxExpansion"));
            return;
        }
        if (!FoodStorageManager.Instance.PayFoodStorageUpgrade())  //돈체크
        {
            ShowSystemMessage(LocalizationManager.Instance.GetString("InteriorBoxNotEnoughGold"));
            return;
        }
        SoundManager.Instance.PlaySFX(_UpgradeSFX);
        FoodStorageManager.Instance.UpgradeFoodStorageindex(); // 실제 데이터(슬롯 배열) 확장
        FoodSlotPool();
        RefreshFoodUpgradeUI();
        string message = string.Format(
        LocalizationManager.Instance.GetString("InteriorBoxUpgradeComplete"),
        FoodStorageManager.Instance.StorageLevel,
        FoodStorageManager.Instance.Capacity
         );
        ShowSystemMessage(message);
        RefreshAll();
    }
    private void FoodSlotPool()  //UI 슬롯 풀(프리팹 복제)도 데이터 Capacity 만큼 늘려줌
    {
        int cap = FoodStorageManager.Instance.Capacity;

        // 이미 충분하면 끝
        if (_foodSlots != null && _foodSlots.Length >= cap) return;

        // 기존 슬롯 유지하면서 더 큰 배열로 확장
        int old = _foodSlots == null ? 0 : _foodSlots.Length;
        Array.Resize(ref _foodSlots, cap);

        for (int i = old; i < cap; i++)
        {
            var go = Instantiate(_slotPrefab, _slotParent, false);
            var slot = go.GetComponent<UI_FoodStorageSlot>();
            _foodSlots[i] = slot;
            slot.Init(this);    //슬롯이 클릭했을 때 UI_Storage로 콜백할 수 있게 초기화
        }
        _slotCount = cap;  //RefreshAll() 루프에서 사용할 UI 슬롯 개수 최신화
    }
    private void RefreshFoodUpgradeUI()  //업그레이드 UI 텍스트/버튼 상태 갱신
    {
        var sm = FoodStorageManager.Instance;
        if (sm == null) return;

        if (_storageLevelText != null)
            _storageLevelText.text = $"Lv.{sm.StorageLevel} / Lv.{FoodStorageManager.MaxLevel}";

        if (_upgradeCostText != null)
        {
            string message = string.Format(
             LocalizationManager.Instance.GetString("InteriorBoxUpgradeCostGold"),
             FoodStorageManager.Instance.FoodGetUpgradeCost().ToString()
             );
            _upgradeCostText.text = message;
        }
        bool isMax = false;     //최대치인지 판단

        if (sm.StorageLevel >= FoodStorageManager.MaxLevel || sm.Capacity >= FoodStorageManager.MaxCapacity)
        {
            isMax = true;
        }

        if (_upgradeButton != null)
            _upgradeButton.interactable = !isMax;

        if (_upgradeButtonLabel != null)     //최대치면 버튼 글자 변경
        {
            string maxText = LocalizationManager.Instance.GetString("Interior_Box_Upgrade_Expand_Text ");
            string upgradeText = LocalizationManager.Instance.GetString("Interior_Box_MaxExpansion_Text");
            _upgradeButtonLabel.text = isMax ? maxText : upgradeText;
        }
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

    public void OpenFoodUpgradeUI()
    {
        SoundManager.Instance.PlaySFX(_ButtonSFX);
        _UpgradePan.gameObject.SetActive(true);
    }

    public void CloseFoodUpgradeUI()
    {
        SoundManager.Instance.PlaySFX(_CloseButtonSFX);
        _UpgradePan.gameObject.SetActive(false);
    }

    // 정렬 버튼
    public void SetSortRecent()
    {
        _currentSort = FoodSortMode.Recent;
        RefreshAll();
    }

    public void SetSortGradeHigh()
    {
        _currentSort = FoodSortMode.GradeHigh;
        RefreshAll();
    }

    public void SetSortGradeLow()
    {
        _currentSort = FoodSortMode.GradeLow;
        RefreshAll();
    }

    public void SetSortName()
    {
        _currentSort = FoodSortMode.Name;
        RefreshAll();
    }
    private void OnDestroy()
    {
        if (_sortDropdown != null)
            _sortDropdown.onValueChanged.RemoveListener(OnSortDropdownChanged);
    }
    public void ResetSelection()
    {
        _selectedRealIndex = -1;
        ClearDetailFood();
    }
    public void OnSortDropdownChanged(int value)
    {
        SoundManager.Instance.PlaySFX(_ButtonSFX);
        _currentSort = (FoodSortMode)value;
        RefreshAll();
    }
    public void ExitFoodStorageUI()
    {
        gameObject.SetActive(false);
    }

}
