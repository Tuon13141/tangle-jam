using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Percas;
using System.Collections;

public class AudioController : SingletonMonoBehaviour<AudioController>
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup musicGroup, soundGroup;

    [Header("Audio Clips")]
    [SerializeField] List<AudioClip> gameMusics;
    [SerializeField] AudioClip sfxSpawnCoins, sfxEarnCoins;

    [Header("Pooling AudioSource")]
    [SerializeField] private int poolSize = 10;

    private SortedList<int, AudioSource> playingSounds;

    private Queue<AudioSource> audioSourcePool;

    private float originalVolume;

    private void Start()
    {
        playingSounds = new();
        audioSourcePool = new Queue<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource pooledSource = gameObject.AddComponent<AudioSource>();
            pooledSource.playOnAwake = false;
            pooledSource.loop = false;
            pooledSource.enabled = false; // Disable until in use
            audioSourcePool.Enqueue(pooledSource);
        }

        HandleMusicValue(GameSetting.MUSIC);
        HandleSoundValue(GameSetting.SOUND);
    }

    public void HandleMusicValue(bool value)
    {
        if (value) TurnOn("Music");
        else TurnOff("Music");
    }

    public void HandleSoundValue(bool value)
    {
        if (value) TurnOn("Sound");
        else TurnOff("Sound");
    }

    public void Mute()
    {
        originalVolume = AudioListener.volume;
        AudioListener.volume = 0;
    }

    public void Unmute()
    {
        AudioListener.volume = originalVolume;
    }

    /// <summary>
    /// Play the SFX from the start, even if it's already playing (rewind).
    /// </summary>
    public void SFXRewind(AudioClip audioClip)
    {
        if (audioClip == null) return;

        try
        {
            int id = audioClip.GetInstanceID();

            if (playingSounds.TryGetValue(id, out var source))
            {
                source.Stop();
                source.Play();
            }
            else
            {
                source = AddAudioSource(audioClip);
                if (source == null) return;

                source.Play();
                playingSounds[id] = source;
            }

            StartCoroutine(HandleReturnToPool(audioClip, source));
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Play the SFX if it's not already playing; does not rewind.
    /// </summary>
    public void SFXNoRewind(AudioClip audioClip)
    {
        try
        {
            if (audioClip == null) return;

            int id = audioClip.GetInstanceID();

            if (playingSounds.TryGetValue(id, out var source))
            {
                if (!source.isPlaying)
                {
                    source.Play();
                    StartCoroutine(HandleReturnToPool(audioClip, source));
                }
            }
            else
            {
                var newSource = AddAudioSource(audioClip);
                if (newSource != null)
                {
                    newSource.Play();
                    StartCoroutine(HandleReturnToPool(audioClip, newSource));
                }
            }
        }
        catch (Exception) { }
    }

    /// <summary>
    /// Play the SFX immediately, even if it's already playing. Supports overlapping.
    /// </summary>
    public void SFXOverride(AudioClip audioClip)
    {
        if (audioClip == null) return;

        var source = GetPooledAudioSource();
        if (source == null) return;

        source.outputAudioMixerGroup = soundGroup;
        source.PlayOneShot(audioClip);

        StartCoroutine(HandleReturnToPoolAfterOneShot(source, audioClip.length));
    }

    public void PlayGameMusic()
    {
        try
        {
            if (audioSource == null) return;

            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = gameMusics[^1];
            audioSource.loop = true;
            audioSource.Play();
        }
        catch (Exception) { }
    }

    public void PlayButtonClick()
    {
        ActionEvent.OnPlaySFXButtonClickOn?.Invoke();
    }

    public void PlayVibration(HapticType hapticType, long milliseconds)
    {
        if (!GameSetting.VIBRATION) return;
        HapticManager.TriggerHapticFeedback(hapticType, milliseconds);
    }

    public void PlaySpawnCoins()
    {
        SFXOverride(sfxSpawnCoins);
    }

    public void PlayEarnCoins()
    {
        SFXOverride(sfxEarnCoins);
    }

    private IEnumerator HandleReturnToPoolAfterOneShot(AudioSource source, float delay)
    {
        if (source == null) yield break;

        yield return new WaitForSeconds(delay);

        // Post-yield try block
        try
        {
            ReturnToPool(source);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while returning AudioSource to pool: {e}");
        }
    }

    private IEnumerator HandleReturnToPool(AudioClip clip, AudioSource source)
    {
        if (source == null) yield break;

        yield return new WaitUntil(() => source == null || !source.isPlaying);

        try
        {
            if (clip != null)
            {
                int id = clip.GetInstanceID();
                if (playingSounds.TryGetValue(id, out var playingSource) && playingSource == source)
                {
                    playingSounds.Remove(id);
                }
            }

            ReturnToPool(source);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in HandleReturnToPool: {e}");
        }
    }

    private void ReturnToPool(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.clip = null;
        source.outputAudioMixerGroup = null;
        if (!audioSourcePool.Contains(source))
        {
            source.enabled = false;
            audioSourcePool.Enqueue(source);
        }
    }

    #region Private Methods
    private AudioSource GetPooledAudioSource()
    {
        AudioSource source = null;

        if (audioSourcePool.Count > 0)
        {
            source = audioSourcePool.Dequeue();
        }
        else
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        if (source == null)
        {
            Debug.LogWarning("AudioSource pool failed to provide a valid AudioSource.");
            return null;
        }

        source.playOnAwake = false;
        source.loop = false;
        source.enabled = true;
        source.clip = null;
        source.outputAudioMixerGroup = null;

        return source;
    }

    private AudioSource AddAudioSource(AudioClip sfx, bool isSFX = true)
    {
        if (sfx == null) return null;

        int id = sfx.GetInstanceID();
        if (playingSounds.TryGetValue(id, out var existingSource))
        {
            return existingSource;
        }

        var source = GetPooledAudioSource();
        if (source == null) return null;

        source.outputAudioMixerGroup = isSFX ? soundGroup : musicGroup;
        source.clip = sfx;

        try
        {
            playingSounds.Add(id, source);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to add sound to dictionary: {e}");
        }

        return source;
    }

    private void TurnOff(string audioType)
    {
        try
        {
            if (audioMixer != null) audioMixer.SetFloat(audioType, -80f);
        }
        catch (Exception) { }
    }

    private void TurnOn(string audioType)
    {
        try
        {
            if (audioMixer != null) audioMixer.SetFloat(audioType, 0f);
        }
        catch (Exception) { }
    }
    #endregion
}
