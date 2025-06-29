using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource;

    [Header("SFX")]
    public AudioSource sfxSource;

    [Header("AudioClip")]
    public AudioClip titleBgm, stageBgm, bossBgm, resultBgm;
    public AudioClip itemSfx, clearSfx, hitsSfx, bossAppearSfx, gameOverSfx;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
