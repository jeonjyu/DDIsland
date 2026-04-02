using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerStatCheat : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
#if TESTMODE

    private void Update()
    {
        if (_player == null) return;

        var kb = Keyboard.current;
        if (kb == null) return;

        // 스태미너 (T, Y, U, I)
        if (kb.tKey.wasPressedThisFrame) TestStamina(28f);  // 가끔 꾸벅
        if (kb.yKey.wasPressedThisFrame) TestStamina(22f);  // 조금 더 자주
        if (kb.uKey.wasPressedThisFrame) TestStamina(17f);  // 꽤나 자주
        if (kb.iKey.wasPressedThisFrame) TestStamina(12f);  // 심해짐

        // 둥둥수치 (G, H, J, K)
        if (kb.gKey.wasPressedThisFrame) TestDoongDoong(0);  // Slim (0~299)
        if (kb.hKey.wasPressedThisFrame) TestDoongDoong(300);  // Normal (300~499)
        if (kb.jKey.wasPressedThisFrame) TestDoongDoong(500);  // Chubby (500~999)
        if (kb.kKey.wasPressedThisFrame) TestDoongDoong(1000); // Round (1000 이상)

        // 배고픔 (V, B, N, M)
        if (kb.vKey.wasPressedThisFrame) TestHunger(22f); // 10초 구간
        if (kb.bKey.wasPressedThisFrame) TestHunger(17f); // 5초 구간
        if (kb.nKey.wasPressedThisFrame) TestHunger(12f); // 3초 구간
        if (kb.mKey.wasPressedThisFrame) TestHunger(5f);  // 1초 구간
    }
    #region 수치 변경 코드
    public void TestStamina(float value)
    {
        _player.PlayerDataOld.SetStamina(value);
    }

    public void TestDoongDoong(int value)
    {
        _player.PlayerDataOld.SetDoongDoongStat(value);
    }

    public void TestHunger(float value)
    {
        _player.PlayerDataOld.SetHunger(value);
    }
    #endregion
#endif
}
