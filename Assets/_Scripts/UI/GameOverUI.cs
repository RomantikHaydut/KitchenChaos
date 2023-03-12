using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text recipeDeliveredText;

    [SerializeField] private TMP_Text score_Text;

    [SerializeField] private TMP_Text best_Score_Text;

    [SerializeField] private Button restartButton;


    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        restartButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.GameScene);
        });

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();
            recipeDeliveredText.text = DeliveryManager.Instance.SuccessfullDeliverAmount().ToString();
            score_Text.text = ScoreManager.Instance.GetScore().ToString();
            ScoreManager.Instance.SaveBestScore();
            best_Score_Text.text = ScoreManager.Instance.GetBestScore().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);

    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
