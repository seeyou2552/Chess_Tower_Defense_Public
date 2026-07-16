using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerData Data { get; private set;}

    void Awake()
    {
        base.Awake();
        Data = SaveSystem.LoadPlayerData();
    }

    public void AddGold(int value)
    {
        Data.Gold += value;

        if(Data.Gold >= 9999)
            Data.Gold = 9999;

        EventBus.Publish(new UIGoldChangedEvent(Data.Gold));
        SaveSystem.Save(Data);
    }

    public bool TryUseGold(int value)
    {
        if(value > Data.Gold)
        {
            EventBus.Publish(new UIAlertEvent("골드가 부족합니다."));
            return false;
        }
        Data.Gold -= value;
        EventBus.Publish(new UIGoldChangedEvent(Data.Gold));

        SaveSystem.Save(Data);
        return true;
    }

    public void AddMinion(int minionIndex)
    {
        if (!Data.MinionList.Contains(minionIndex))
        {
            Data.MinionList.Add(minionIndex);
            Data.MinionList.Sort();
            SaveSystem.Save(Data);
        }
    }

    public void IncreaseMaxHP(int amount)
    {
        Data.MaxHP += amount;
        SaveSystem.Save(Data);
    }

    public void IncreaseStartGold(int amount)
    {
        Data.StartGold += amount;
        SaveSystem.Save(Data);
    }
}
