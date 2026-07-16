using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TD/Shop/StartGoldItemData", fileName = "StartGoldItem")]
public class IncreaseStartGoldItemData : ItemData
{
    public int IncreaseAmount;

    public override bool TryBuy()
    {
        PlayerManager.Instance.IncreaseStartGold(IncreaseAmount);
        return true;
    }
}
