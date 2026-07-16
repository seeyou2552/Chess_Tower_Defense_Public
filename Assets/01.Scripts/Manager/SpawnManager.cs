using System.Collections;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [Header("Minion")]
    [SerializeField] private GameObject _minionPrefab;
    [SerializeField] private GameObject _minionPool;

    [Header("Enemy")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyPool;

    [Header("Enemy HPBar")]
    [SerializeField] private HPBarUI _hpBarPrefab;
    [SerializeField] private GameObject _hpBarPool;

    [Header("Skill")]
    [SerializeField] private SkillEffect _skillPrefab;
    [SerializeField] private GameObject _skillPool;

    [Header("Effect")]
    [SerializeField] private EffectObject _effectPrefab;
    [SerializeField] private GameObject _effectPool;

    


    void Start()
    {
        if(_enemyPrefab == null)
            _enemyPrefab = Resources.Load<GameObject>("Enemy/DefaultEnemy");
        if(_minionPrefab == null)
            _minionPrefab = Resources.Load<GameObject>("Minion/DefaultMinion");
        if(_skillPrefab == null)
            _skillPrefab = Resources.Load<SkillEffect>("Skill/DefaultSkillObject");
        if(_effectPrefab == null)
            _effectPrefab = Resources.Load<EffectObject>("Skill/DefaultAfterObject");
        if(_hpBarPrefab == null)
            _hpBarPrefab = Resources.Load<HPBarUI>("UI/HPBar");
    }


    public GameObject MinionSpawn(MinionData minionData)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        
        GameObject createMinion = ObjectPoolManager.Instance.Get(_minionPrefab.name, _minionPrefab, pos, _minionPool);

        Minion minion = createMinion.GetComponent<Minion>();
        minion.Init(minionData);

        return createMinion;
    }


    public void SpawnEnemy(EnemyData enemyData)
    {
        int randomRouteIndex = Random.Range(0, PathManager.Instance.Paths.Count); // 무작위 경로 선택

        GameObject enemyObj = ObjectPoolManager.Instance.Get(
            _enemyPrefab.name,
            _enemyPrefab,
            PathManager.Instance.GetPoint(randomRouteIndex, 0).position,
            _enemyPool
            );
        
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Movement.SetTarget(randomRouteIndex);
        enemy.Init(enemyData);
    }

    public HPBarUI GetHPBar()
    {
        HPBarUI hPBar = Instantiate(_hpBarPrefab, _hpBarPool.transform);

        return hPBar;
    }


    public SkillEffect GetSkillObject(Vector3 pos)
    {
        SkillEffect skillObj = ObjectPoolManager.Instance.GetSkillEffect(_skillPrefab, _skillPool);
        skillObj.transform.position = pos;

        return skillObj;
    }


    public EffectObject GetEffect(Vector3 pos, EffectObject effectObjectPrefab = null)
    {
        EffectObject effectObj;
        
        if (effectObjectPrefab != null)
            effectObj = ObjectPoolManager.Instance.GetEffect(effectObjectPrefab.name, effectObjectPrefab, _effectPool);
        
        else
            effectObj = ObjectPoolManager.Instance.GetEffect(_effectPrefab.name, _effectPrefab, _effectPool);
        
        effectObj.transform.position = pos;
        
        return effectObj;
    }
    
}