
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct CookingContext
{
    public int ID;
    public string foodName;
    public string foodDesc;
    public int PurchasePrice;
    public FoodRateType foodrateType;
    public int MainIngredient;
    public int SubIngredient;
    public FoodEffectType foodeffectType;
    public float HungerBuffRate;
    public int DoongDoongBuffRate;
    public Sprite FoodImgPath_Sprite;
    public GameObject FoodIconPath_GameObject;
}
public class CookingManager : Singleton<CookingManager>
{
    public event System.Action<int> OnFoodCooked; // 도감 연동용 이벤트 
    Dictionary<int, FoodDataSO> _foodById;
    List<FoodDataSO> _allfood;
    private void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _allfood = new List<FoodDataSO>(DataManager.Instance.FoodDatabase.FoodInfoData.datas);
        _foodById = new Dictionary<int, FoodDataSO>(); 
        foreach (var food in _allfood)
        {
            _foodById[food.ID] = food;
        }

    }

    public List<FoodDataSO> BuildCookCandidates(CookingContext ctx)  //현재 보유 물고기 검사,레시피 해금 여부검사
    {
        var db = DataManager.Instance.FoodDatabase.FoodInfoData;
        var result = new List<FoodDataSO>();

        foreach (var food in db.datas)
        {
            if (food == null) continue;

            //1 재료 검사
            if (!HasFish(food.MainIngredient))
                continue;

            if (food.SubIngredient != 0 && !HasFish(food.SubIngredient))
                continue;

            //2 기본 회 처리
            if (IsBasicSashimi(food))
            {
                // 같은 주재료를 쓰는 해금된 일반 레시피가 하나도 없을 때만 기본 회 허용
                if (!HasUnlockedRecipeForMainIngredient(food.MainIngredient))
                {
                    result.Add(food);
                }

                continue;
            }

            //3 일반 음식은 레시피 해금된 것만 허용
            if (!HasRecipeUnlocked(food.ID))
                continue;

            result.Add(food);
        }

        return result;
    }
    public bool HasFish(int fishId)
    {
        var fish = FishStorageManager.Instance.FishSlotData;
        foreach (var slot in fish)
        {
            if (!slot.HasValue) continue;
            if (slot.Value.FishId == fishId) return true;
        }

        return false;
    }
    private bool IsBasicSashimi(FoodDataSO food)
    {
        return food != null && food.ID == 50001;
    }
    private bool HasRecipeUnlocked(int foodId)
    {
        if (ItemManager.Instance == null) return false;

        if (!ItemManager.Instance.playerItemDatas.TryGetValue(StoreCat.recipe, out var recipeDb)) return false;

        if (recipeDb == null || recipeDb.Items == null) return false;

        for (int i = 0; i < recipeDb.Items.Count; i++)
        {
            IStoreItem item = recipeDb.Items[i];
            if (item == null) continue;

            if (item.ID == foodId && item.IsGained) return true;
        }

        return false;
    }
    private bool HasUnlockedRecipeForMainIngredient(int fishId)
    {
        var db = DataManager.Instance.FoodDatabase.FoodInfoData;
        if (db == null || db.datas == null) return false;

        foreach (var food in db.datas)
        {
            if (food == null) continue;
            if (food.MainIngredient != fishId) continue;
            if (IsBasicSashimi(food)) continue; // 기본 회 제외

            if (HasRecipeUnlocked(food.ID)) return true;
        }

        return false;
    }
    public FoodDataSO PickRecipe(List<FoodDataSO> rating, CookingContext ctx)  //등급정렬, 동일등급랜덤
    {
        if (rating == null || rating.Count == 0) return null;
        List<FoodDataSO> result = new List<FoodDataSO>();
        FoodRateType maxRate = FoodRateType.None;
        var fish = FishStorageManager.Instance.FishSlotData;
        for (int i = 0; i < rating.Count; i++)
        {
            if (maxRate < rating[i].foodrateType)
            {
                maxRate = rating[i].foodrateType;
            }
        }
        
        foreach (var item in rating)
        {
            if (maxRate == item.foodrateType)
            {
                result.Add(item);
            }
        }
        if (maxRate == FoodRateType.Normal)  //일반등급이면 기본회보다 일반레시피우선 
        {
            List<FoodDataSO> normalRecipe = new List<FoodDataSO>();

            for (int i = 0;i < result.Count;i++)
            {
                if (result[i].ID != 50001) normalRecipe.Add(result[i]);  //기본회 무시
            }

            if (normalRecipe.Count > 0) return normalRecipe[UnityEngine.Random.Range(0, normalRecipe.Count)];

        }
        return result[UnityEngine.Random.Range(0, result.Count)]; 
    }
    public void CreateCookedFood(FoodDataSO food) //요리보관함에 보관
    {
        //음식생성
        FoodToStorage(food);
        OnFoodCooked?.Invoke(food.ID); 
    }
    public void FoodIngredientsRemove(FoodDataSO food)  //재료차감
    {
        FishStorageManager.Instance.TryRemoveFishById(food.MainIngredient);
        if (food.SubIngredient != 0) FishStorageManager.Instance.TryRemoveFishById(food.SubIngredient);
    }
    public void FoodToStorage(FoodDataSO myFood)
    {
        FoodInstance food = new FoodInstance
        {
            FoodId = myFood.ID,
        };
        EmojiController.Instance.ShowFoodEmoji(myFood);
        Debug.Log($"Name: {myFood.FoodName_String}");
        bool success = FoodStorageManager.Instance.TryAddToFoodStorage(food);
        if (!success)
        {
            // 가득함 처리
        }
    }
}
