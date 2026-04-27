using System;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int Score { get; private set; }
    
    public Action<int> OnScoreChanged;
    
    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);
    }
}
