using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionDataManager : Singleton<MinionDataManager>
{
    public List<MinionData> MinionDatas;

    private readonly Dictionary<int, MinionData> _minionIndexDict = new();
    public IReadOnlyDictionary<int, MinionData> MinionIndexDict => _minionIndexDict;

    protected override void Awake()
    {
        base.Awake();
        MinionDatas.ForEach(data => 
        {
            _minionIndexDict.Add(data.MinionIndex, data);
        });
    }

}
