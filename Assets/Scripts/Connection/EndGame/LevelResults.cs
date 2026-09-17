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
    public float AverageTime => (puzzle1CompletedTime + puzzle2CompletedTime) / 2.0f;
    public int Puzzle1Stars => CalculateStars(puzzle1CompletedTime, puzzle1TargetTime);
    public int Puzzle2Stars => CalculateStars(puzzle2CompletedTime, puzzle2TargetTime);
    public int AverageStars => Mathf.RoundToInt((Puzzle1Stars + Puzzle2Stars) / 2f);
    private int CalculateStars(float completedTime, float target)
    {
        if (target <= 0) return 3;
        if (completedTime <= target) return 3;
        if (completedTime <= target * 1.5f) return 2;
        return 1;
    }
}
