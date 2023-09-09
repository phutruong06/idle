using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using IdleGunner.Chapter;
using NaughtyAttributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using IdleGunner.UI;
using IdleGunner.Skills;
using IdleGunner.Gameplay;
using IdleGunner.UI.Popup;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using IdleGunner.Gameplay.UI;
using EnergySuite;
using System.Linq;

namespace IdleGunner
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("INITIALIZE - USE ONCE")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider loadingSlider;

        [Header("CHAPTERS SELECT")]
        [SerializeField, Expandable] private ChapterSettings selectedChapter;

        [Header("SKILL DATAS")]
        public SkillDataCollectionSO skillDataCollection;

        [Header("SETTINGS")]
        [SerializeField, Scene] private int homeScene;

        [Header("DEBUG")]
        public CoreData coreData = new CoreData();
        
        private void Start()
        {
            loadingSlider.value = 0;

            StartCoroutine(InitializeGame());
        }

        private IEnumerator InitializeGame()
        {
            // Init google play
#if !UNITY_EDITOR
            if (HasNetworkConnection())
                await GooglePlayManager.Instance.Authorize();
            loadingSlider.DOValue(0.3f, 0.2f);
#endif

#if UNITY_EDITOR
            //coreData = new CoreData();
#endif

            // TODO: Load save data
#if !UNITY_EDITOR
            GooglePlayManager.Instance.LoadGame((status, bytes) =>
            {
                coreData.playerData = BytesObjectHelper.ByteArrayToObject<PlayerData>(bytes);
                loadingSlider.DOValue(0.4f, 0.2f);
            });
#endif

            // Load energy
            coreData.playerData.energy = EnergySuiteManager.GetAmount(TimeValue.Energy);
            loadingSlider.DOValue(0.5f, 0.2f);

            // Load Settings
            Application.targetFrameRate = GameSettings.highFps ? 60 : 30;
            MusicManager.Instance.SetMusic(GameSettings.music);
            QualitySettings.SetQualityLevel(GameSettings.highGraphics ? 1 : 0, true);
            loadingSlider.DOValue(0.6f, 0.2f);

            // Load 3D Home Scene
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(homeScene, LoadSceneMode.Additive);
            while (asyncLoad.progress <= 0.9f)
            {
                yield return null;
            }
            loadingSlider.DOValue(0.7f, 0.2f);


            // Finish setting up
            loadingSlider.DOValue(0.8f, 0.2f)/*.OnComplete(() => HomeMenuManager.Instance.ReloadData())*/;
            DOTween.Sequence().Append(loadingSlider.DOValue(1f, 0.2f)).Append(_canvasGroup.DOFade(0, 0.2f)).OnComplete(() => { HomeMenuManager.Instance.ShowMenu(); Destroy(_canvasGroup.gameObject); });

        }

        public void InitializeGameplay()
        {
            StartCoroutine(InitializeGameplayCo());
        }

        public IEnumerator InitializeGameplayCo()
        {
            // Set up gameplay
            PlayerController.Instance.SetupPlayer(coreData.playerData.playerGameplay, coreData.playerData.inventory.skillSlot);
            StageSpawner.Instance.SetupMap(selectedChapter);

            yield return new WaitForSeconds(1);

            PopupManager.Instance.HidePopup<Popup_Loading>();
            CameraController.Instance.Initialize(coreData.playerData.playerGameplay.range.value);

            var gameplayWavePopup = PopupManager.Instance.GetPopup<Popup_GameplayWave>();
            gameplayWavePopup.Initialize(new Popup_GameplayWave.Option()
            {
                chapterNumber = selectedChapter.chapterContent.levelNumber,
                chapterNumberTextColor = selectedChapter.chapterContent.levelNummberTextColor,

                chapterName = selectedChapter.chapterContent.levelName,
                chapterNameTextColor = selectedChapter.chapterContent.levelNameTextColor,
                
                chapterBackgroundColor = selectedChapter.chapterContent.levelBackgroundColor
            });
            PopupManager.Instance.ShowPopup(gameplayWavePopup);
            
            yield return new WaitForSeconds(0.5f);

            StageSpawner.Instance.SpawnLoop();
        }

        public static bool HasNetworkConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = new WebClient().OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static float totalPlayTime
        {
            get
            {
                float totalTime = Time.realtimeSinceStartup;
                if (PlayerPrefs.HasKey("totaltime"))
                    totalTime += PlayerPrefs.GetFloat("totaltime");
                return totalTime;
            }
        }
        public static TimeSpan totalPlayTimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds((double)(new decimal(totalPlayTime)));
            }
        }

        private void OnApplicationQuit()
        {
            GooglePlayManager.Instance.SaveGame(BytesObjectHelper.ObjectToByteArray(coreData.playerData),
                (status, game) =>
                {

                });
            PlayerPrefs.SetFloat("totalPlayTime", totalPlayTime);
        }
    }

    [System.Serializable]
    public class CoreData
    {
        public string googlePlayId => PlayGamesPlatform.Instance.localUser.id;
        public PlayerData playerData = new PlayerData();
    }

    [System.Serializable]
    public class PlayerData
    {
        [System.Serializable]
        public class Inventory
        {
            public int bigChestKey = 0;
            public int royalChestKey = 0;

            public double gold = 200;
            public double diamond = 0;

            // Skills
            public List<SkillSlot> skillSlot = new List<SkillSlot>();
            public List<SkillCard> skillCardOwned = new List<SkillCard>();

            // Hero
            public List<Hero> heroOwned = new List<Hero>();

            public SkillCard GetSkillCard(int id)
            {
                return skillCardOwned.Where(x => x.id == id).First();
            }
        }
        [System.Serializable]
        public class SeasonPass
        {
            // Season Pass
            public int seasonPassLevel = 1;
            public int seasonPassExp = 0;
            public bool isPurchasedSeasonPass = false;

            //public List<ChapterRewardData> seasonPassReward = new List<ChapterRewardData>();
            //public List<ChapterRewardData> freeReward = new List<ChapterRewardData>();
        }
        [System.Serializable]
        public class Chapter
        {
            public List<int> chaptersUnlockedIndex = new List<int>();
            public int lastStage = 1;
        }
        [System.Serializable]
        public class Purchased
        {
            public int timePurchased = 0;
            public bool unlockSpeed = false;
            public bool noAds = false;
            public List<Blessing> blessingOwned = new List<Blessing>();
        }
        [System.Serializable]
        public class PiggyBank
        {
            public int piggyBankCount = 0;
            public int piggyBankMax = 1000;
            public int timeClaimed = 0;
        }

        public Inventory inventory = new Inventory();
        public SeasonPass seasonPass = new SeasonPass();
        public Chapter chapter = new Chapter();
        public Purchased purchased = new Purchased();
        public PiggyBank piggyBank = new PiggyBank();
        public int energy = 10;

        // Player gameplay data
        public Gameplay.PlayerGameplay playerGameplay = new Gameplay.PlayerGameplay();

        // Free Rewarded
        public int freeRewardClaimed = 0;

        // TO DO: Achivement & Daily
    }

    [System.Serializable]
    public class Hero
    {

    }

    [System.Serializable]
    public class ExpBlessing : Blessing
    {

    }

    [System.Serializable]
    public class DamageBlessing : Blessing
    {

    }

    [System.Serializable]
    public class Blessing
    {
        public bool isPermanent = false;
        public int dayCost = 60;
        public string pernamentCost = "";
    }

    [System.Serializable]
    public static class GameSettings
    {
        public static bool music
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(music)))
                {
                    return PlayerPrefs.GetInt(nameof(music)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(music), 1);
                else
                    PlayerPrefs.SetInt(nameof(music), 0);
            }
        }
        public static bool sounds
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(sounds)))
                {
                    return PlayerPrefs.GetInt(nameof(sounds)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(sounds), 1);
                else
                    PlayerPrefs.SetInt(nameof(sounds), 0);
            }
        }
        public static bool highFps
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(highFps)))
                {
                    return PlayerPrefs.GetInt(nameof(highFps)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(highFps), 1);
                else
                    PlayerPrefs.SetInt(nameof(highFps), 0);
            }
        }
        public static bool highGraphics
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(highGraphics)))
                {
                    return PlayerPrefs.GetInt(nameof(highGraphics)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(highGraphics), 1);
                else
                    PlayerPrefs.SetInt(nameof(highGraphics), 0);
            }
        }
        public static bool floatingText
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(floatingText)))
                {
                    return PlayerPrefs.GetInt(nameof(floatingText)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(floatingText), 1);
                else
                    PlayerPrefs.SetInt(nameof(floatingText), 0);
            }
        }
        public static bool goldCoin
        {
            get
            {
                if (PlayerPrefs.HasKey(nameof(goldCoin)))
                {
                    return PlayerPrefs.GetInt(nameof(goldCoin)) == 1 ? true : false;
                }
                else
                    return true;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetInt(nameof(goldCoin), 1);
                else
                    PlayerPrefs.SetInt(nameof(goldCoin), 0);
            }
        }
    }
}