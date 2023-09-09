using IdleGunner.Gameplay.UI.Popup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI 
{ 
    public class GameplayTopBar : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] private Button pauseButton;

        private void Start()
        {
            pauseButton.onClick.AddListener(() =>
            {
                PopupManager.Instance.ShowPopup<Popup_GameplayPause>();
            });
        }
    }
}