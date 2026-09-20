using UnityEngine;

public static class Utility 
{
    // AI brukt for å finne ut hvordan man presenterer en float verdi over til sting i from av tid
    public static string ConvertTime(float time)
    {
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        int ms = Mathf.FloorToInt((time * 100) % 100);

        string timeString = string.Format("{0:00}:{1:00}:{2:00}", min, sec, ms);

        return timeString;
    }

}
