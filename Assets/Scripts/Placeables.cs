using UnityEngine;

public abstract class BasePlaceable : ScriptableObject
{
    [SerializeField] private string placeableName;
    [SerializeField] private int id;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 transformOffsets;
    [SerializeField] private int cost;

    public string PlaceableName { get => placeableName; set => placeableName = value; }
    public int Id { get => id; set => id = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
    public Vector3 TransformOffsets { get => transformOffsets; set => transformOffsets = value; }
    public int Cost { get => cost; set => cost = value; }
}


[CreateAssetMenu(fileName = "Placeable", menuName = "Scriptable Objects/EconomyPlaceable")]
public class EconomyPlaceable : BasePlaceable
{
    [SerializeField] private int income;

    public int Income { get => income; set => income = value; }
}


[CreateAssetMenu(fileName = "Placeable", menuName = "Scriptable Objects/GenericPlaceable")]
public class GenericPlaceable : BasePlaceable
{
}