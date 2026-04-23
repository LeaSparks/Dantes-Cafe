using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Dialogue/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters;
}