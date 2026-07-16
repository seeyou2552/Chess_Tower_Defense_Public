using System;
using System.Collections;
using System.Collections.Generic;
// using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Tilemaps;


public class StageManager : Singleton<StageManager>
{
    [Header("Stage Data")]
    public List<StageData> Stages = new List<StageData>();
    public int CurrentStage = 0;
    public int CurrentWave = 0;
    public int CurrentHP = 0;
    public int StageGold;

    [Header("Tilemap")]
    public Tilemap MinionTilemap;

    private int _aliveEnemyCount = 0;
    private bool _isSpawnFinished = false;
    private Coroutine _spawnCoroutine;


    void OnEnable()
    {
        EventBus.Subscribe<WaveStartRequestEvent>(WaveStart);
        EventBus.Subscribe<EnemyDeathEvent>(EnemyDeath);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartRequestEvent>(WaveStart);
        EventBus.Unsubscribe<EnemyDeathEvent>(EnemyDeath);
    }

    public void StartStage()
    {
        GameManager.Instance.ChangeGameState(GameState.Intermission);
        CurrentWave = 0;
        CurrentHP = PlayerManager.Instance.Data.MaxHP;
        StageGold = PlayerManager.Instance.Data.StartGold;
        _aliveEnemyCount = 0;
        
        EventBus.Publish(new UIGoldChangedEvent(StageGold));
        EventBus.Publish(new UIHPChangedEvent(CurrentHP, PlayerManager.Instance.Data.MaxHP));
        
        SoundManager.Instance.PlayBGM("StageBGM");
    }

    private void StageClear()
    {
        // FirebaseManager.Instance.LogEvent(
        //         "stage_clear",
        //         new Parameter("stage_id", CurrentStage)
        //     );

        PlayerManager.Instance.AddGold(Stages[CurrentStage-1].ClearGold);
        GameManager.Instance.ChangeGameState(GameState.GameClear);
    }

    public void StageOver()
    {
        if(_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        // FirebaseManager.Instance.LogEvent(
        //         "stage_fail",
        //         new Parameter("stage_id", CurrentStage)
        //     );

        GameManager.Instance.ChangeGameState(GameState.GameOver);
    }

    /// <summary>
    /// Wave 진행 중 씬 전환 시 필요한 정리 작업
    /// </summary>
    public void CleanupStage()
    {
        // SpawnCoroutine 정지
        if(_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    public bool TryUseGold(int value)
    {
        if(StageGold - value < 0)
        {
            EventBus.Publish(new UIAlertEvent("골드가 부족합니다."));
            return false;
        }
        else
        {
            StageGold -= value;
            EventBus.Publish(new UIGoldChangedEvent(StageGold));
        }
        return true;
        
    }

    public void AddGold(int value)
    {
        StageGold += value;
        EventBus.Publish(new UIGoldChangedEvent(StageGold));
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        _aliveEnemyCount--;
        EventBus.Publish(new UIHPChangedEvent(CurrentHP, PlayerManager.Instance.Data.MaxHP));

        if(CurrentHP <= 0)
            StageOver();
        
        CheckEndWave();
    }

    IEnumerator SpawnCoroutine()
    {
        foreach(WaveEnemy wave in Stages[CurrentStage-1].Wave[CurrentWave].EnemyList)
        {
            for (int i = 0; i < wave.Count; i++)
            {
                yield return YieldCache.GetWaitForSeconds(wave.SpawnDelay);
                SpawnManager.Instance.SpawnEnemy(wave.EnemyData);
                _aliveEnemyCount++;
                
            }
        }

        _isSpawnFinished = true;
        _spawnCoroutine = null;
    }

    public void CheckEndWave()
    {
        if(_isSpawnFinished && _aliveEnemyCount == 0)
        {
            CurrentWave++;
            EndWave();
        }
    }

    public void EndWave()
    {   
        if(CurrentWave >= Stages[CurrentStage-1].Wave.Count)
        {
            StageClear();
        }
        else
        {
            GameManager.Instance.ChangeGameState(GameState.Intermission);
            EventBus.Publish(new WaveEndEvent());
        }
    }

    public Dictionary<EnemyData, int> GetCurrentWaveEnemyList() 
    {
        Dictionary<EnemyData, int> enemyDict = new Dictionary<EnemyData, int>();

        foreach(var enemyList in Stages[CurrentStage-1].Wave[CurrentWave].EnemyList)
        {
            if (!enemyDict.ContainsKey(enemyList.EnemyData))
                enemyDict[enemyList.EnemyData] = 0;
            
            enemyDict[enemyList.EnemyData] += enemyList.Count;
        }

        return enemyDict;
    }

    

#region Event Handlers

    private void WaveStart(WaveStartRequestEvent e)
    {
        if(GameManager.Instance.GameState != GameState.Intermission)
            return;

        GameManager.Instance.ChangeGameState(GameState.Wave);
        EventBus.Publish(new WaveStartEvent());

        _isSpawnFinished = false;
        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    
    }

    private void EnemyDeath(EnemyDeathEvent e)
    {
        AddGold(e.EnemyState.Data.Reward);

        _aliveEnemyCount--;
        CheckEndWave();
    }

#endregion

}

#region EventBus

 /// <summary>
 /// 웨이브 시작 요청
 /// </summary>
public readonly struct WaveStartRequestEvent {}

/// <summary>
///  웨이브 시작 시 발생할 이벤트
/// </summary>
public readonly struct WaveStartEvent {}

/// <summary>
///  웨이브 끝날 시 발생할 이벤트
/// </summary>
public readonly struct WaveEndEvent {}

#endregion