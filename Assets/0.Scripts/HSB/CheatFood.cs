#if TESTMODE
using UnityEngine;
using UnityEngine.InputSystem;

public class CheatFood : MonoBehaviour
{
    [SerializeField] private int _foodId = 50004;
    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.f12Key.wasPressedThisFrame)
        {
            FoodInstance FoodDescGrilledGoldenMandarinFish = new() { FoodId = _foodId };
            FoodStorageManager.Instance.TryAddToFoodStorage(FoodDescGrilledGoldenMandarinFish);
        }
        
    }
}
#endif
