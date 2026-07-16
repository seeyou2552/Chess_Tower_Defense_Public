using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeController
{
    private MinionState _state;
    private readonly Dictionary<UpgradeData, int> _upgradeLevels = new();

    public IReadOnlyDictionary<UpgradeData, int> UpgradeLevels => _upgradeLevels;

    public UpgradeController(MinionState state)
    {
        _state = state;

        _upgradeLevels = new();
    }
    public void Init()
    {
        CleanupUpgrade();
        
        foreach(var upgrade in _state.Data.Upgrades)
        {
            _upgradeLevels.Add(upgrade.UpgradeData, 0);
        }
    }

    public void UpgradeLevelUp(UpgradeData upgradeData)
    {
        if (!_upgradeLevels.ContainsKey(upgradeData))
            return;

        _upgradeLevels[upgradeData] += 1;
    }

    public bool MaxUpgradeCheck()
    {
        foreach(var upgrade in _state.Data.Upgrades)
        {
            if (_upgradeLevels[upgrade.UpgradeData] < upgrade.MaxLevel)
                return false;
        }

        return true;
    }

    public void CleanupUpgrade()
    {
        _upgradeLevels.Clear();
    }
}
