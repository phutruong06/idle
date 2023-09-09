using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class AudioInstanceMono : MonoBehaviour
{
    [SerializeField, ReadOnly] private AudioSource m_audioSource;
    [SerializeField]
    private UnityEvent m_onEnd = new UnityEvent();
    public UnityEvent onEnd
    {
        get
        {
            return m_onEnd;
        }
        set
        {
            m_onEnd = value;
        }
    }

    private void OnValidate()
    {
        TryGetComponent(out m_audioSource);
    }

    private void Start()
    {
        m_audioSource.loop = false;
    }

    public void Play(Vector3 position, AudioClip audioClip, float pitch = 1, float volume = 1)
    {
        StartCoroutine(PlayCo(position, audioClip, pitch, volume));
    }

    private IEnumerator PlayCo(Vector3 position, AudioClip audioClip, float pitch = 1, float volume = 1)
    {
        m_audioSource.clip = audioClip;
        transform.position = position;

        m_audioSource.pitch = pitch;
        m_audioSource.volume = volume;
        m_audioSource.Play();

        yield return new WaitUntil(() => !m_audioSource.isPlaying);
        onEnd.Invoke();
    }
}
