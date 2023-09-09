using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace IdleGunner
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : Singleton<MusicManager>
    {
        [SerializeField] private AudioSource _audioSource;

        private void OnValidate()
        {
            TryGetComponent(out _audioSource);
        }

        public void SetMusic(bool value)
        {
            _audioSource.volume = value ? 1 : 0;
        }

        public async void SetAudio(AudioClip clip)
        {
            _audioSource.DOFade(0, 0.5f);

            await Task.Delay(500);

            _audioSource.clip = clip;

            _audioSource.Play();
            _audioSource.DOFade(1, 0.5f);
        }
    }
}