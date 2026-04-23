using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Dialogue/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;

    [Header("Sprites")]
    public List<Sprite> sprites;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public List<string> dialogueLines;

    [Header("Audio")]
    public AudioClip blipSound;

    [Header("Final Dialogue")]
    [TextArea(2,5)]
    public string finalDialogue;

    public float finalTextSpeed = 0.06f;
    public float finalPitch = 0.7f;
}