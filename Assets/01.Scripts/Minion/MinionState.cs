using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionState
{
    public MinionData Data { get; private set; }
    public MinionRuntimeStat RuntimeStat { get; private set; }
    public BuffController BuffController { get; private set; }
    public UpgradeController UpgradeController { get; private set; }

    public int SellGold { get; private set; }
    public bool IsFirstWave { get; set; }
    public bool IsSelected { get; set; }
    public bool IsSiege { get; set; }

    // 상태 변화를 알려줄 이벤트들
    public event Action OnAttackReady;
    public event Action OnSkillReady;
    public event Action OnStatChanged;

    private float _attackTimer;

    // 풀에서 꺼내어 재사용할 때 호출할 함수
    public void Setup(MinionData data)
    {
        Data = data;

        RuntimeStat = RuntimeStat ?? new MinionRuntimeStat();

        RuntimeStat.ResetRuntimeStat(); // 기존 데이터 청소
        RuntimeStat.Init(data);         // 새 데이터로 덮어쓰기

        BuffController = BuffController ?? new BuffController(this);

        UpgradeController = UpgradeController ?? new UpgradeController(this);
        UpgradeController.Init();

        SellGold = data.Cost;
        IsFirstWave = true;
        IsSelected = false;
        IsSiege = false;
    }

    // 매 프레임 타이머 계산 로직 수행
    public void UpdateTimer(float deltaTime, bool hasEnemyInRange)
    {
        _attackTimer += deltaTime;

        if (_attackTimer >= RuntimeStat.CurrentAttackCooldown && hasEnemyInRange)
        {
            _attackTimer = 0f;

            // 현재 AC가 MaxAC와 같으면 스킬 시전
            if (RuntimeStat.CurrentAC >= RuntimeStat.CurrentMaxAC)
            {
                OnSkillReady?.Invoke();
                RuntimeStat.ResetCurrentAC();
                OnStatChanged?.Invoke();
                return;
            }

            // 현재 AC가 MaxAC보다 작으면 기본 공격 시전
            if (RuntimeStat.CurrentAC < RuntimeStat.CurrentMaxAC)
                RuntimeStat.AddAC(1);

            OnAttackReady?.Invoke();
            OnStatChanged?.Invoke();
        }
    }

    public void ResetAttackTimer()
    {
        _attackTimer = RuntimeStat.BaseAttackCooldown;
    }

    public void WaveEndReset()
    {
        RuntimeStat.ResetCurrentAC();
        ResetAttackTimer();
        OnStatChanged?.Invoke();
    }

    public void SellGoldUpdate(int goldValue)
    {
        SellGold += goldValue;
        OnStatChanged?.Invoke();
    }

    public void NotifyStatChanged()
    {
        OnStatChanged?.Invoke();
    }
}
