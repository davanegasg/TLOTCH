using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;
    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    [Header("Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    public override bool Use(Monster monster)
    {
        if(restoreMaxHP||hpAmount>0)
        {
            if(monster.HP == monster.MaxHp)
                return false;
            if (restoreMaxHP)
                monster.IncreaseHP(monster.MaxHp);
            else
                monster.IncreaseHP(hpAmount);
        }
        //Recover status
        if (status != ConditionID.none)
        {
            if (monster.Status == null && monster.VolatileStatus == null)
                return false;
            if (recoverAllStatus)
            {
                monster.CureStatus();
                monster.CureVolatileStatus();
            }
            else
            {
                if (monster.Status.Id == status)
                    monster.CureStatus();
                else if (monster.VolatileStatus.Id == status)
                    monster.CureVolatileStatus();
                else
                    return false;

            }
        }
        //Restore PP
        if(restoreMaxPP)
        {
            monster.Moves.ForEach(m => m.IncreasePP(m.Base.PP));

        }
        else if (ppAmount>0)
        {
            monster.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }
        return true;
    }

}
