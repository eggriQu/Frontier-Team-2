using UnityEngine;

[CreateAssetMenu(fileName = "Placeable", menuName = "Scriptable Objects/Placeable")]
public class Placeable : ScriptableObject
{
    public string placeableName;
    //public int id;
    public GameObject prefab;
    public Vector3 transformOffsets;
}
