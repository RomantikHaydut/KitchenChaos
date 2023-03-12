using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text soundEffectText;
    [SerializeField] private TMP_Text musicText;
    #region Key Rebinding Texts
    [SerializeField] private TMP_Text moveUpText;
    [SerializeField] private TMP_Text moveDownText;
    [SerializeField] private TMP_Text moveLeftText;
    [SerializeField] private TMP_Text moveRightText;
    [SerializeField] private TMP_Text interactText;
    [SerializeField] private TMP_Text interactAlternateText;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text gamePadInteractText;
    [SerializeField] private TMP_Text gamePadInteractAlternateText;
    [SerializeField] private TMP_Text gamePadPauseText;
    #endregion
    #region Key Rebinding Buttons
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternateButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamePadInteractButton;
    [SerializeField] private Button gamePadInteractAlternateButton;
    [SerializeField] private Button gamePadPauseButton;
    #endregion
    [SerializeField] private Transform pressToRebindKeyTransform;

    private Action onCloseButtonAction;

    private void Awake()
    {
        Instance = this;

        soundEffectButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisual();
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
            onCloseButtonAction();
        });

        moveUpButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Move_Up);
        });

        moveDownButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Move_Down);
        });

        moveLeftButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Move_Left);
        });

        moveRightButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Move_Right);
        });

        interactButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Interact);
        });

        interactAlternateButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Interact_Alternate);
        });

        pauseButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Pause);
        });

        gamePadInteractButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Game_Pad_Interact);
        });

        gamePadInteractAlternateButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Game_Pad_Interact_Alternate);
        });

        gamePadPauseButton.onClick.AddListener(() =>
        {
            RebindBinding(GameInput.Binding.Game_Pad_Pause);
        });

    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;

        UpdateVisual();

        Hide();

        HidePressPressToRebindKey();
    }

    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void UpdateVisual()
    {
        soundEffectText.text = "Sound Effects : " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f).ToString();
        musicText.text = "Music : " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f).ToString();

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
        gamePadInteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Interact);
        gamePadInteractAlternateText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Interact_Alternate);
        gamePadPauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Game_Pad_Pause);
    }

    public void Show(Action onCloseButtonAction)
    {
        this.onCloseButtonAction = onCloseButtonAction;
        gameObject.SetActive(true);
        soundEffectButton.Select();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }

    private void HidePressPressToRebindKey()
    {
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding)
    {
        ShowPressPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressPressToRebindKey();
            UpdateVisual();
        });
    }

}
