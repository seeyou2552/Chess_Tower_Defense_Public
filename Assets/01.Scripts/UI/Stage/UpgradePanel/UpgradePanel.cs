using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private UpgradeBtn[] _btns= new UpgradeBtn[3];
    [SerializeField] private Button _allUpgradeBtn;

    public void SetBtn(MinionState minionState)
    {
        for(int i=0; i<3; i++)
        {
            _btns[i].Init(minionState.Data.Upgrades[i], minionState);
        }

        _allUpgradeBtn.onClick.RemoveAllListeners();
        _allUpgradeBtn.onClick.AddListener(AllUpgradeBtn);
    }

    private void AllUpgradeBtn()
    {
        for(int i=0; i<_btns.Length; i++)
        {            
            while (!_btns[i].MaxLevelCheck())
            {
                // 업그레이드가 불가능해지면 해당 업그레이드 중단
                if (!_btns[i].TryUpgrade())
                    break;
            }
            
        }
    }
}
