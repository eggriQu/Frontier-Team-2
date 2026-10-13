using UnityEngine;
using System.Collections;

public class NeedsManager : MonoBehaviour
{
    public int Hunger = 100;
    public int Energy = 100;
    public int Fun = 100;
    [SerializeField] public int DeteriorationInterval = 1;
    [SerializeField] public int DeteriorationTick = 1;

    private void Deteriorate()
    {
        Hunger -= DeteriorationTick;
        Energy -= DeteriorationTick;
        Fun -= DeteriorationTick;

        ClampAll();
    }

    private IEnumerator ReDeteriorate()
    {
        while (true)
        {
            Deteriorate();
            yield return new WaitForSeconds(DeteriorationInterval);
        }
    }

    private void ClampAll()
    {
        Hunger = Mathf.Clamp(Hunger, 0, 100);
        Energy = Mathf.Clamp(Energy, 0, 100);
        Fun = Mathf.Clamp(Fun, 0, 100);
    }

    private void Start()
    {
        StartCoroutine(ReDeteriorate());
    }

    public void Feed(int FoodValue)
    {
        Hunger += FoodValue;
        ClampAll();
    }

    public void Rest(int SleepValue)
    {
        Energy += SleepValue;
        ClampAll();
    }

    public void Play(int JoyValue)
    {
        Fun += JoyValue;
        ClampAll();
    }
}
