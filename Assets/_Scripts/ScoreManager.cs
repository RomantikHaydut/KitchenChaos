using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string PLAYER_PREFS_BEST_SCORE_KEY = "Best_Score";

    public static ScoreManager Instance;

    public event EventHandler<int> OnScoreChanged;

    private int score;

    private int bestScore;

    private void Awake()
    {
        Instance = this;

        bestScore = PlayerPrefs.GetInt(PLAYER_PREFS_BEST_SCORE_KEY, 0);
    }

    void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, RecipeSO e)
    {
        score += e.scoreFactor;
        OnScoreChanged?.Invoke(this, e.scoreFactor);
    }

    public void SaveBestScore()
    {
        if (score > bestScore)
        {
            bestScore = score;
        }

        PlayerPrefs.SetInt(PLAYER_PREFS_BEST_SCORE_KEY, bestScore);
        PlayerPrefs.Save();

    }

    public int GetScore()
    {
        return score;
    }

    public int GetBestScore()
    {
        return bestScore;
    }
}
