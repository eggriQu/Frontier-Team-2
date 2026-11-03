using System.Collections;
using UnityEngine;
/// <summary>
/// super basic game manager script.
/// Currently only handles player's money and income from placeables
///
/// Income is increased when a placeable is placed. This will need
/// to be changed.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private float money;
    [SerializeField] private float income;

    [SerializeField] private float incomeInterval;

    public float Money { get => money; set => money = value; }
    public float Income { get => income; set => income = value; }

    private void Start()
    {
        StartCoroutine(RegularIncome());
    }

    private IEnumerator RegularIncome()
    {
        while (true)
        {
            yield return new WaitForSeconds(incomeInterval);
            money = Mathf.Lerp(Money, Money + Income, incomeInterval); // lerp no lerpy :/
        }
    }
}
