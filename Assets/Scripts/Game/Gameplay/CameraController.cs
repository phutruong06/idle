using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IdleGunner.Gameplay
{
    public class CameraController : SceneSingleton<CameraController>
    {
        [Header("SETTINGS")]
        [SerializeField] private Vector3 cameraOpenPosition = Vector3.zero;
        [SerializeField] private float cameraOrthoPerMeter = 2.4f;

        [Header("DEBUG")]
        [SerializeField, ReadOnly] private Camera _camera;

        private Vector3 cameraOriginPosition = Vector3.zero;
        private bool open = false;

        private void OnValidate()
        {
            TryGetComponent(out _camera);
        }

        private void Start()
        {
            cameraOriginPosition = transform.position;
        }

        public void OpenCamera(bool value)
        {
            if (value)
            {
                transform.DOMove(cameraOpenPosition, 0.3f);
            }
            else
            {
                transform.DOMove(cameraOriginPosition, 0.3f);
            }

            open = value;
        }

        public void Initialize(double playerRange)
        {
            _camera.orthographicSize = 5f;
            DOTween.Sequence().AppendInterval(1)
                              .Append(_camera.DOOrthoSize((float)(playerRange * cameraOrthoPerMeter), 0.5f));
        }

        public void IncreaseOrthSize(double playerRange)
        {
            _camera.DOOrthoSize((float)(playerRange * cameraOrthoPerMeter), 0.5f);
        }

        public void Shake(float duration, float strenght = 3f, int vibrato = 10, float randomness = 90f, bool fadeOut = true, ShakeRandomnessMode shakeRandomnessMode = ShakeRandomnessMode.Full)
        {
            DOTween.Sequence().Append(_camera.DOShakePosition(duration, strenght, vibrato, randomness, fadeOut, shakeRandomnessMode))
                              .Append(_camera.transform.DOMove(open ? cameraOpenPosition : cameraOriginPosition, 0.2f));

            //_camera.DOShakePosition(duration, strenght, vibrato, randomness, fadeOut, shakeRandomnessMode);
        }
    }
}