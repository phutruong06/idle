using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Gameplay.UI
{
    public class GameSpeed : MonoBehaviour
    {
        [Header("BUTTONS")]
        [SerializeField] private Button slowerButton;
        [SerializeField] private Button fasterButton;

        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI speedText;

        private int savedGameSpeed
        {
            get
            {
                if (PlayerPrefs.HasKey("GameSpeed"))
                {
                    return PlayerPrefs.GetInt("GameSpeed");
                }
                else
                {
                    PlayerPrefs.SetInt("GameSpeed", 1);
                    return PlayerPrefs.GetInt("GameSpeed");
                }
            }
            set
            {
                if (value < 1)
                    return;

                if (value > 2)
                {
                    if (!GameManager.Instance.coreData.playerData.purchased.unlockSpeed)
                        value = 2;
                    else
                    {
                        if (value > 5)
                            return;
                    }
                    
                }
                PlayerPrefs.SetInt("GameSpeed", value);

            }
        }

        private void Start()
        {
            slowerButton.onClick.AddListener(() =>
            {
                savedGameSpeed--;
                speedText.text = savedGameSpeed.ToString() + "x";
                Time.timeScale = savedGameSpeed;
            });

            fasterButton.onClick.AddListener(() =>
            {
                savedGameSpeed++;
                speedText.text = savedGameSpeed.ToString() + "x";
                Time.timeScale = savedGameSpeed;
            });

            speedText.text = savedGameSpeed.ToString() + "x";
            Time.timeScale = savedGameSpeed;
        }

        private void OnDestroy()
        {
            slowerButton.onClick.RemoveAllListeners();
            fasterButton.onClick.RemoveAllListeners();
        }
    }
}
