using UnityEngine;

[CreateAssetMenu(fileName = "Placeable", menuName = "Scriptable Objects/EconomyPlaceable")]
public class EconomyPlaceable : BasePlaceable
{
    [SerializeField] private float income;

    public float Income { get => income; set => income = value; }
}