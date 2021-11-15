using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Monsters", menuName = "Monsters/Create new monster")]
public class MonsterBase : ScriptableObject
{
   
    [SerializeField] string _name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField]  MonsterAttribute attribute1;
    [SerializeField]  MonsterAttribute attribute2;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] int expYield;

    [SerializeField] List<LearnableMove> learnableMoves;

    public static int MaxNumOfMoves { get; set; } = 4;
    public int GetExpForLevel(int level)
    {
        return level * level * level;
    }
    public string Name { get { return _name; } }
    public string Description { get { return description; } }

    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public MonsterAttribute Attribute1{ get { return attribute1; } }
    public MonsterAttribute Attribute2 { get { return attribute2; } }
    public int MaxHp { get { return maxHp; } }
    public int Attack { get { return attack; } }
    public int Defense { get { return defense; } }
    public int SpAttack { get { return spAttack; } }
    public int SpDefense { get { return spDefense; } }
    public int Speed { get { return speed; } }

    public int ExpYield { get { return expYield; } }

    public List<LearnableMove> LearnableMoves { get { return learnableMoves; } }
}
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;
    public MoveBase Base { get { return moveBase; } }
    public int Level { get { return level; } }
}
public enum MonsterAttribute
{
    None,
    Normal,
    Fuego,
    Agua,
    Electrico,
    Manga,
    Hielo,
    veneno,
    Volador,
    Psiquico,
    Insecto,
    Roca,
    Fantasma,
    Dragon
}
public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,
    //Not actual stats, used to boost accuracy
    Accuracy,
    Evasion
}
public class TypeChart
{
    static float[][] chart =
    { //                   NOR FIR WAT ELE GRA ICE POI FLY PSY BUG RCK GHT DRG
       /*NOR*/ new float[]{ 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f},
       /*Fir*/ new float[]{ 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f},
       /*Wat*/ new float[]{ 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f},
       /*Ele*/ new float[]{ 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f},
       /*Gra*/ new float[]{ 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f},
       /*Ice*/ new float[]{ 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 2f},
       /*Poi*/ new float[]{ 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f},
       /*Fly*/ new float[]{ 1f, 1f, 1f, 0.5f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f},
       /*Psy*/ new float[]{ 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f, 1f },
       /*bug*/ new float[]{ 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 2f, 1f, 1f, 0.5f,1f},
       /*Rck*/ new float[]{ 1f, 2f, 1f, 1f, 1f, 2f, 1f, 2f, 1f, 2f, 1f, 1f, 1f},
       /*Ght*/ new float[]{ 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f},
       /*Drg*/ new float[]{ 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f},
    };

    public static float GetEffectiveness(MonsterAttribute attackType, MonsterAttribute defenseType)
    {
        if(attackType== MonsterAttribute.None || defenseType ==MonsterAttribute.None)
        {
            return 1;
        }
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;
        return chart[row][col];

    }
}
