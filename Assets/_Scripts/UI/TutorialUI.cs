using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TMP_Text keyMoveUpText;
    [SerializeField] private TMP_Text keyMoveDownText;
    [SerializeField] private TMP_Text keyMoveLeftText;
    [SerializeField] private TMP_Text keyMoveRightText;
    [SerializeField] private TMP_Text keyInteractText;
    [SerializeField] private TMP_Text keyInteractAlternateText;
    [SerializeField] private TMP_Text keyPauseText;
    [SerializeField] private TMP_Text keyGamePadInteractText;
    [SerializeField] private TMP_Text keyGamePadInteractAlternateText;
    [SerializeField] private TMP_Text keyGamePadPauseText;

    private void Start()
    {
        GameInput.Instance.OnRebind += GameInput_OnRebind;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        UpdateVisual();

        Show();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void GameInput_OnRebind(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        keyMoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        keyMoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        keyMoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        keyInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        keyPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        keyGamePadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Interact);
        keyGamePadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Interact_Alternate);
        keyGamePadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Pause);
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
