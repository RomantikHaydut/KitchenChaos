using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManagerUI : MonoBehaviour
{
    private const string SCORE_ADDING = "Score_Add";

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text addingScoreText;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        ScoreManager.Instance.OnScoreChanged += ScoreManager_OnScoreChanged;
    }

    private void ScoreManager_OnScoreChanged(object sender, int e)
    {
        addingScoreText.text = "+" + e.ToString();
        animator.SetTrigger(SCORE_ADDING);
    }

    private void UpdateScoreText() // Animation Event
    {
        if (scoreText != null)
        {
            scoreText.text = "Score\n" + ScoreManager.Instance.GetScore();
        }
    }
}
