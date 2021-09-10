using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Answer", menuName = "Monsters/Create new Answer")]
public class AnswerBase : ScriptableObject
{
    [SerializeField] string _name;
    public string Name { get { return _name; } }
}
