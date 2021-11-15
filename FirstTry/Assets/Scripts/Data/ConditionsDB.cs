using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name= "Poison",
                StartMessage = "ha sido envenenado",
                OnAfterTurn= (Monster monster) =>
                {
                    monster.DecreaseHP(monster.MaxHp/8);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} ha sido herido por el veneno");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name= "Burn",
                StartMessage = "ha sido quemado",
                OnAfterTurn= (Monster monster) =>
                {
                    int damage= monster.MaxHp/16;
                    if(damage<=0)
                    {

                        damage=1;
                    }
                    monster.DecreaseHP(damage);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} ha sido herido por el fuego");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name= "Paralyzed",
                StartMessage = "ha sido paralizado",
                OnBeforeMove = (Monster monster) =>
                {

                   if( Random.Range(1,5)==1)
                    {
                        
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} esta paralizado y no se puede mover");
                        return false;
                    }
                    return  true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name= "Freeze",
                StartMessage = "ha sido congelado",
                OnBeforeMove = (Monster monster) =>
                {
                   if( Random.Range(1,5)==1)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} se descongelo");
                        return true;
                    }
                   monster.StatusChanges.Enqueue($"{monster.Base.Name} esta congelado y no se puede mover");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name= "Sleep",
                StartMessage = "se ha dormido",
                OnStart = (Monster monster) =>
                {
                    //Sleep for 1-3 turns
                    monster.StatusTime=Random.Range(1,4);
                    Debug.Log($"will be asleep for {monster.StatusTime} moves");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.StatusTime<=0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} se desperto");
                        return true;
                    }
                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} esta durmiendo");
                    return false;
                }
            }
        },

        //Volatile Status Conditions
        {
            ConditionID.confusion,
            new Condition()
            {
                Name= "Confusion",
                StartMessage = "ha sido confundido",
                OnStart = (Monster monster) =>
                {
                    //Be confused for 1-4 turns
                    monster.VolatileStatusTime=Random.Range(1,5);
                    Debug.Log($"will be confused for {monster.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Monster monster) =>
                {
                    if(monster.VolatileStatusTime<=0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} ya no esta confundido");
                        return true;
                    }
                    monster.VolatileStatusTime--;
                    //50% chance to do a move
                    if(Random.Range(1,100)==1)
                    {
                        return true;
                    }
                    
                    //Hurt by confusion
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} esta confundido");
                    monster.DecreaseHP(monster.MaxHp/4);
                    
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} se hirio a si mismo por la confusion");
                    return false;
                }
            }
        }

    };
}
public enum ConditionID
{
     none,psn,brn,slp,par,frz,confusion
}