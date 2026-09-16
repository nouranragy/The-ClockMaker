using System;
using UnityEngine;

[System.Serializable]
public class LevelResult
{
    public string playerNames;
    public float puzzle1TargetTime;
    public float puzzle2TargetTime;
    public float puzzle1CompletedTime;
    public float puzzle2CompletedTime;

    public float TotalTime => puzzle1CompletedTime + puzzle2CompletedTime;
    public float AverageTime => TotalTime / 2.0f;

    public int Puzzle1Stars => CalculateStars(puzzle1CompletedTime, puzzle1TargetTime);
    public int Puzzle2Stars => CalculateStars(puzzle2CompletedTime, puzzle2TargetTime);
    public int AverageStars => Mathf.RoundToInt((Puzzle1Stars + Puzzle2Stars) / 2f);

    private int CalculateStars(float completedTime, float targetTime)
    {
        float third = targetTime / 3.0f;
        float twoThirds = (targetTime / 3.0f) * 2.0f;

        if (completedTime <= third) return 3;
        if (completedTime <= twoThirds) return 2;
        return 1;
    }
}