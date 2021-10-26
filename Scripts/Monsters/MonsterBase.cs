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
    [SerializeField] MonsterAttribute attribute1;
    [SerializeField] MonsterAttribute attribute2;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;
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
    Water,
    Fire,
    Earth,
    Normal,
    Air,
    Electric,
    Ice,
    Poison
}