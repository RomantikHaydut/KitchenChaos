using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "SoundEffectsVolume";

    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipRefSO audioClipRefSO;

    private float volume = 1.0f;

    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickSomething += Instance_OnPickSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = (TrashCounter)sender;
        PlaySound(audioClipRefSO.trash, trashCounter.transform.position, 1f);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = (BaseCounter)sender;
        PlaySound(audioClipRefSO.objectDrop, baseCounter.transform.position, 1f);
    }

    private void Instance_OnPickSomething(object sender, System.EventArgs e)
    {
        Player player = (Player)sender;
        PlaySound(audioClipRefSO.objectPickup, player.transform.position, 1f);
    }

    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = (CuttingCounter)sender;
        PlaySound(audioClipRefSO.chop, cuttingCounter.transform.position, 1f);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefSO.deliveryFail, deliveryCounter.transform.position, 1f);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, RecipeSO e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefSO.deliverySuccess, deliveryCounter.transform.position, 1f);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volumeMultiplier * volume);
    }

    public void PlayFootStepSound(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(audioClipRefSO.footStep[Random.Range(0, audioClipRefSO.footStep.Length)], position, 1f);
    }

    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefSO.warning, Vector3.zero);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefSO.warning, position);
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1.0f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
