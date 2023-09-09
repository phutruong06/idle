using MackySoft.XPool;
using UnityEngine;

namespace IdleGunner.Gameplay.Pooler
{
    public class AudioInstanceStopTrigger : MonoBehaviour
    {
        [SerializeField] private AudioInstanceMono m_popupText;
        [SerializeField] private IPool<AudioInstanceMono> m_Pool;

        internal void Initialize(AudioInstanceMono ps, IPool<AudioInstanceMono> pool)
        {
            m_popupText = ps;
            m_Pool = pool;

            m_popupText.onEnd.AddListener(() =>
            {
                m_Pool?.Return(m_popupText);
            });
        }

    }
}
