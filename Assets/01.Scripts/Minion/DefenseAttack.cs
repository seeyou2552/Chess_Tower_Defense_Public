using System;
using System.Collections;
using System.Collections.Generic;


public class DefenseAttack
{
    public static void DefaultAttack(float damage, Minion minion, MinionState minionState)
    {
        if (minion.AttackRange.EnemyCheck())
        {
            List<ChessPiece> targets = minion.AttackRange.SearchTargets(
                minion,
                minionState.RuntimeStat.SearchScope,
                minionState.RuntimeStat.CurrentAtkMaxTarget
                );

            SkillEffect skillObj;
            switch (minionState.RuntimeStat.DeliveryType)
            {
                case AttackDeliveryType.Instant:
                    foreach (var target in targets)
                    {
                        if (target is Enemy enemy)
                        {
                            skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                            skillObj.transform.position = target.transform.position;
                            skillObj.DefaultAttack(target as Enemy, minionState);

                            enemy.TakeDamage(damage);
                        }
                        
                    }
                    break;

                case AttackDeliveryType.Projectile:
                    foreach (var target in targets)
                    {
                        if (target is Enemy enemy)
                        {
                            skillObj = SpawnManager.Instance.GetSkillObject(minion.transform.position);
                            skillObj.transform.position = minion.transform.position;
                            skillObj.DefaultAttack(target as Enemy, minionState, true); // isProjectile true로 설정
                        }
                    }
                    break;
            }

            SoundManager.Instance.PlaySFX(minionState.Data.AtkSFX);
        }
    }

    public static void UseSkill(float damage, Minion minion, MinionState minionState)
    {
        if (minion.AttackRange.EnemyCheck())
        {
            List<ChessPiece> targets = minion.AttackRange.SearchTargets(
                minion,
                minionState.Data.Skill.SearchScope,
                minionState.RuntimeStat.SkillMaxTarget
            );

            ISkillStrategy skillStrategy = SkillStrategyFactory.GetSkillStrategy(minionState.Data.Skill.SkillType);
            skillStrategy?.Execute(minion, minionState, targets, damage);

            SoundManager.Instance.PlaySFX(minionState.Data.Skill.SkillSFX);
        }
    }
    
}
