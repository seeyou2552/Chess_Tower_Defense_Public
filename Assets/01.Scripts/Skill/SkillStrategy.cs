using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillStrategy
{
    void Execute(Minion minion, MinionState minionState, List<ChessPiece> targets, float damage);
}

public class ImpactSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, MinionState minionState, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(enemy.transform.position);
                skillObj.SkillInit(enemy, minionState, minionState.Data.Skill, damage, false);
                skillObj.ApplyDamageTo(enemy);
            }
        }
    }
}

public class ProjectileSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, MinionState minionState, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            Debug.Log(target);
            if (target is Enemy enemy)
            {
                SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                skillObj.SkillInit(enemy, minionState, minionState.Data.Skill, damage, true);
            }
        }
    }
}

public class AOESkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, MinionState minionState, List<ChessPiece> targets, float damage)
    {
        SkillEffect skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);

        foreach (var target in targets)
        {
            if (target is Enemy enemy)
            {
                if (enemy.IsDead)
                    continue;

                skillObj.SkillInit(enemy, minionState, minionState.Data.Skill, damage, false);
                skillObj.ApplyDamageTo(enemy);
            }
        }
        skillObj.transform.localScale = minion.AttackRange.gameObject.transform.localScale;
    }
}

public class BuffSkillStrategy : ISkillStrategy
{
    public void Execute(Minion minion, MinionState minionState, List<ChessPiece> targets, float damage)
    {
        foreach (var target in targets)
        {
            if (target is Minion targetMinion)
            {
                foreach(var buff in minionState.Data.Skill.BuffList)
                    buff.Data.ApplyBuff(targetMinion, buff);
            }
        }

        // 공격 타이머 초기화
        minionState.ResetAttackTimer();
    }
}