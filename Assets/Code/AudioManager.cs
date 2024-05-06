using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#OpeningBGM")]
    public AudioClip openingClip;
    public float openingVolume;
    AudioSource openingPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;

    [Header("#Volume Sliders")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    int channelIndex;

    public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win }

    void Awake()
    {
        instance = this;
        Init();
        instance.PlayOpening(true);
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        GameObject openingObject = new GameObject("OpeningPlayer");
        openingObject.transform.parent = transform;
        openingPlayer = openingObject.AddComponent<AudioSource>();
        openingPlayer.playOnAwake = false;
        openingPlayer.loop = true;
        openingPlayer.clip = openingClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;
        }

        // Load volume settings from PlayerPrefs
        bgmVolume = PlayerPrefs.GetFloat("BgmVolume", bgmVolume);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", sfxVolume);

        // Set initial volume slider values
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        // Update volume settings
        ChangeBgmVolume();
        ChangeSfxVolume();
    }

    public void PlayBgm(bool IsPlay)
    {
        if (IsPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void PlayOpening(bool IsPlay)
    {
        if (IsPlay)
        {
            openingPlayer.Play();
        }
        else
        {
            openingPlayer.Stop();
        }
    }

    public void EffectBgm(bool IsPlay)
    {
        bgmEffect.enabled = IsPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;
            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }
            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void ChangeBgmVolume()
    {
        float volume = bgmSlider.value;
        bgmPlayer.volume = volume;
        openingPlayer.volume = volume;
        PlayerPrefs.SetFloat("BgmVolume", volume);
    }

    public void ChangeSfxVolume()
    {
        float volume = sfxSlider.value;

        foreach (var player in sfxPlayers)
        {
            player.volume = volume;
        }

        PlayerPrefs.SetFloat("SfxVolume", volume);
    }
}
