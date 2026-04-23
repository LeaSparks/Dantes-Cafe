using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Scriptable Objects/RoomData")]
public class RoomData : ScriptableObject
{
    public int FloorNumber;
    public int PointsToWin;
    public float EnemyIntelligenceThreshold;
    public Color WallColor;
}
