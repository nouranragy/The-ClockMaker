using UnityEngine;

[System.Serializable]
public class LevelResult
{
    public string playerNames;
    public float puzzle1TargetTime = 180f;
    public float puzzle2TargetTime = 180f;
    public float puzzle1CompletedTime;
    public float puzzle2CompletedTime;

    // Averages calculated independently from each puzzle's performance
    public float AverageTime => (puzzle1CompletedTime + puzzle2CompletedTime) / 2f;

    // Independent Star Calculations:
    // 3 Stars: 0s to 60s
    // 2 Stars: 60s to 120s
    // 1 Star:  120s to 180s
    // 0 Stars: > 180s
    public int Puzzle1Stars => CalculateStars(puzzle1CompletedTime, puzzle1TargetTime);
    public int Puzzle2Stars => CalculateStars(puzzle2CompletedTime, puzzle2TargetTime);

    // Rounded average of earned stars (e.g., 1 star + 3 stars = 2 stars average)
    public int AverageStars => Mathf.RoundToInt((Puzzle1Stars + Puzzle2Stars) / 2f);

    private int CalculateStars(float completedTime, float targetTime)
    {
        if (targetTime <= 0f || completedTime <= 0f || completedTime > targetTime)
        {
            return 0;
        }

        float third = targetTime / 3f;

        if (completedTime <= third) return 3;
        if (completedTime <= third * 2f) return 2;
        return 1;
    }
}