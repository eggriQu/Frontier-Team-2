using UnityEngine;

public abstract class BasePlaceable : ScriptableObject
{
    [SerializeField] private string placeableName;
    [SerializeField] private int id;
    [SerializeField] private GameObject prefab;
    [SerializeField] private int cost;

    public string PlaceableName { get => placeableName; set => placeableName = value; }
    public int Id { get => id; set => id = value; }
    public GameObject Prefab { get => prefab; set => prefab = value; }
    public int Cost { get => cost; set => cost = value; }
}