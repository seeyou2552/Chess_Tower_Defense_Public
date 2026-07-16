using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [Header("참조 Component")]
    private SpriteRenderer _sr;
    private LayerMask _enemyLayer;
    private LayerMask _minionLayer;
    private Renderer[] _rangeRenderers;
    private CircleCollider2D _circle;

    [Header("Color")]
    [SerializeField] private Color _originalColor;
    [SerializeField] private Color _impossibleColor;

    private List<Enemy> _enemies = new List<Enemy>(); // 적을 관리하는 리스트

    void Awake()
    {
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();

        if (_circle == null)
            _circle = GetComponent<CircleCollider2D>();

        // 오브젝트의 모든 Renderer 컴포넌트를 수집
        _rangeRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    void Start()
    {       
        _enemyLayer = LayerMask.GetMask("Enemy");
        _minionLayer = LayerMask.GetMask("Minion");

        // 시작시 가시성 설정
        SetOriginalColor();
        ApplyVisibility();
        SetImposibleColor();
    }

    private void ApplyVisibility() 
    {
        bool shouldShow = true;

        // 게임 설정에 따라 AttackRange의 가시성을 적용
        if (GameManager.Instance != null && GameManager.Instance.SettingData != null)
            shouldShow = GameManager.Instance.SettingData.IsAtkRangeOn;

        SetVisible(shouldShow);
    }

    private List<Minion> FindMinionsInRange()
    {
        float radius = _circle.radius * transform.lossyScale.x;

        // radius 범위 내의 모든 minionLayer에 속한 Collider2D를 수집
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            _minionLayer
        );

        List<Minion> result = new();

        foreach (var hit in hits)
        {
            // 오류를 방지하기 위해 Minion 컴포넌트를 가진 오브젝트만 필터링
            if (hit.TryGetComponent<Minion>(out var minion))
            {
                result.Add(minion);
            }
        }

        return result;
    }

    private bool IsEnemy(Collider2D other)
    {
        return ((1 << other.gameObject.layer) & _enemyLayer) != 0;
    }

    public void RangeIncrease(float range)
    {
        transform.localScale += new Vector3(range, range, 0);
    }

    public void SetRange(float newRange)
    {
        transform.localScale = new Vector3(newRange, newRange, 1);
        _enemies.Clear();
        SetOriginalColor();
    }

    public List<Enemy> GetEnemiesInRange()
    {
        return _enemies;
    }

    public List<ChessPiece> SearchTargets(Minion minion, SearchScope searchType, int maxTarget = 0)
    {
        List<ChessPiece> targets = new List<ChessPiece>();
        
        // searchType에 따라 타겟 추가 후 반환
        switch (searchType)
        {
            case SearchScope.SingleTarget:

                targets.Add(_enemies[0]);
                break;

            case SearchScope.MultiTarget:

                for(int i=0; i < Mathf.Min(maxTarget, _enemies.Count); i++)
                {
                    if (_enemies[i].IsDead)
                    {
                        _enemies.RemoveAt(i);
                        i--;
                    }
                    
                    else
                        targets.Add(_enemies[i]);
                    
                }
                break;

            case SearchScope.Area:
                for (int i = _enemies.Count - 1; i >= 0; i--)
                {
                    if (_enemies[i].IsDead)
                        _enemies.RemoveAt(i);
                    else
                    {
                        targets.Add(_enemies[i]);
                    }
                }
                break;

            case SearchScope.Self:
                targets.Add(minion);
                break;

            case SearchScope.AreaMinion:
                FindMinionsInRange().ForEach(m => targets.Add(m));
                break;
                
        }

        return targets;
    }

    /// <summary>
    ///  적이 한명이라도 존재하는지 확인
    /// </summary>
    public bool EnemyCheck()
    {
        if (_enemies.Count > 0)
        {
            for (int i=0; i< _enemies.Count; i++)
            {
                if (_enemies[i].IsDead)
                {
                    _enemies.RemoveAt(i);
                    i--;
                }
                else
                    return true;
            }
        }

        return false;
    }


#region OnTriggerEvent

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsEnemy(other))
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                _enemies.Add(enemy);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsEnemy(other))
        {
            if (other.TryGetComponent<Enemy>(out var enemy))
            {
                _enemies.Remove(enemy);
            }
        }
    }

#endregion


#region Visual

    /// <summary>
    /// 모든 AttackRange 오브젝트의 가시성을 설정
    /// </summary>
    public static void SetVisibleForAll(bool visible)
    {
        var allRanges = FindObjectsOfType<AttackRange>();
        for (int i = 0; i < allRanges.Length; i++)
        {
            allRanges[i].SetVisible(visible);
        }
    }

    public void SetVisible(bool visible)
    {
        if (_rangeRenderers == null || _rangeRenderers.Length == 0)
            _rangeRenderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        for (int i = 0; i < _rangeRenderers.Length; i++)
        {
            if (_rangeRenderers[i] != null)
                _rangeRenderers[i].enabled = visible;
        }
    }

    public void SetOriginalColor()
    {
        _sr.color = _originalColor;
    }

    public void SetImposibleColor()
    {

        _impossibleColor.a = _originalColor.a;
        _sr.color = _impossibleColor;
        
    }

#endregion
}