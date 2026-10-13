using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public int TotalSeconds = 0; //This number constantly counts up for others to calculate off of.
    public int Seconds = 0; //Current seconds in HH:MM:SS.
    public int Minutes = 0; //Current minutes in HH:MM:SS.
    public int Hours = 0; //Current hours in HH:MM:SS.
    public int Days = 0; //Current amount of days spent.
    public string DayOfWeek = string.Empty;
    public string TimeName = string.Empty;
    [SerializeField] public float SpeedScale = 1; //Multiplier for speed of time so it is fast enough to experience time change.
    
    private IEnumerator UpdateTime()
    {
        while (true)
        {
            TotalSeconds += 1;

            Days = TotalSeconds / 86400;
            int DaysRemainder = TotalSeconds % 86400;
            Hours = DaysRemainder / 3600;
            int HoursRemainder = TotalSeconds % 3600;
            Minutes = HoursRemainder / 60;
            Seconds = HoursRemainder % 60;

            GetDayOfWeek();
            GetTimeName();

            float WaitSpeed = 1 / SpeedScale;
            yield return new WaitForSeconds(WaitSpeed);
        }
    }

    private void GetDayOfWeek()
    {
        int DayInWeek = Days % 7;
        switch (DayInWeek)
        {
            case 0:
                DayOfWeek = "Monday";
                break;
            case 1:
                DayOfWeek = "Tuesday";
                break;
            case 2:
                DayOfWeek = "Wednesday";
                break;
            case 3:
                DayOfWeek = "Thursday";
                break;
            case 4:
                DayOfWeek = "Friday";
                break;
            case 5:
                DayOfWeek = "Saturday";
                break;
            case 6:
                DayOfWeek = "Sunday";
                break;
        }
    }

    private void GetTimeName()
    {
        if (Hours >= 5 && Hours < 12)
        {
            TimeName = "Morning";
        }
        else if (Hours >= 12 && Hours < 17)
        {
            TimeName = "Afternoon";
        }
        else if (Hours >= 17 && Hours < 21)
        {
            TimeName = "Evening";
        }
        else
        {
            TimeName = "Night";
        }
    }

    void Start()
    {
        StartCoroutine(UpdateTime());
    }
}
