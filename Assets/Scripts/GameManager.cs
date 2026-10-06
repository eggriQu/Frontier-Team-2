using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float money;
    [SerializeField] private float income;

    [SerializeField] private float IncomeInterval;

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
            yield return new WaitForSeconds(IncomeInterval);
            money = Mathf.Lerp(Money, Money + Income, IncomeInterval); // lerp no lerpy :/
        }
    }
}
