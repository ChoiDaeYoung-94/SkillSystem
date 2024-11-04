using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _AS_sfx = null;

    [Header("Audio Clips")]
    [SerializeField] public AudioClip _AC_sfx_qBefore = null;
    [SerializeField] public AudioClip _AC_sfx_qAfter = null;

    private void Awake()
    {
        Init();
    }

    #region Functions
    private void Init()
    {
        instance = this;
        DontDestroyOnLoad(instance);
    }

    public void PlaySFX(AudioClip clip)
    {
        _AS_sfx.clip = clip;
        _AS_sfx.Play();
    }
    #endregion
}