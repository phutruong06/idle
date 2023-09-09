#if UNITY_ANDROID
using UnityEngine;
using System;
using System.Collections.Generic;
//gpg
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
//for encoding
using System.Text;
//for extra save ui
using UnityEngine.SocialPlatforms;
//for text, remove
using UnityEngine.UI;
using System.Threading;
using TMPro;
using System.Threading.Tasks;

namespace IdleGunner
{
    public class GooglePlayManager : Singleton<GooglePlayManager>
    {
        [Header("SETTINGS")]
        [SerializeField] private string GGCS_fileName = "IdleGunner";

        [SerializeField] private bool enableSaveGame => PlayGamesPlatform.Instance.localUser.authenticated;
        public bool isProcessing
        {
            get;
            private set;
        }
        public string loadedData
        {
            get;
            private set;
        }
        public bool isAuthenticated
        {
            get
            {
                return Social.localUser.authenticated;
            }
        }
        protected override void Awake()
        {
            base.Awake();
        }
        public void Authorize()
        {
            PlayGamesPlatform.Instance.Authenticate((status) =>
            {
                if (status == SignInStatus.Success)
                {
                    print($"Welcome {PlayGamesPlatform.Instance.localUser.id}\n");
                }
                else
                {
                    print($"Fail:{status}");
                }
            });
        }

        private void OpenSavedGame(string filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, callback);
        }

        private Texture2D GetScreenshot()
        {
            // Create a 2D texture that is 1024x700 pixels from which the PNG will be
            // extracted
            Texture2D screenShot = new Texture2D(1024, 700);

            // Takes the screenshot from top left hand corner of screen and maps to top
            // left hand corner of screenShot texture
            screenShot.ReadPixels(
                new Rect(0, 0, Screen.width, (Screen.width / 1024) * 700), 0, 0);
            return screenShot;
        }

        public void SaveGame(byte[] data, Action<SavedGameRequestStatus, ISavedGameMetadata> callback = null)
        {
            OpenSavedGame(GGCS_fileName, (status, game) =>
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                builder = builder
                    .WithUpdatedPlayedTime(GameManager.totalPlayTimeSpan)
                    .WithUpdatedDescription("Saved game at " + DateTime.Now);

                byte[] pngData = GetScreenshot().EncodeToPNG();
                builder = builder.WithUpdatedPngCoverImage(pngData);

                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                savedGameClient.CommitUpdate(game, updatedMetadata, data, callback);
            });
        }

        public void LoadGame(Action<SavedGameRequestStatus, byte[]> completedCallback)
        {
            OpenSavedGame(GGCS_fileName, (status, game) =>
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.ReadBinaryData(game, completedCallback);
            });
        }
    }

    [System.Serializable]
    public static class GPG_CloudSaveSystem
    {
        //keep track of saving or loading during callbacks.
        [SerializeField] private static bool m_saving;
        //save name. This name will work, change it if you like.
        [SerializeField] private static string m_saveName = "IdleGunner-ZombieDefense";

        //check with GPG (or other*) if user is authenticated. *e.g. GameCenter
        private static bool Authenticated => Social.Active.localUser.authenticated;

        public static async void SaveGame(byte[] savedData, TimeSpan totalPlaytime, Action<SavedGameRequestStatus, ISavedGameMetadata> callback = null, CancellationToken ctk = default)
        {
            if (!Authenticated)
            {
                Debug.Log("Google Play not authenticated");
                return;
            }

            m_saving = true;
            OpenSavedGame((status, game) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                    SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                    builder = builder
                    .WithUpdatedPlayedTime(totalPlaytime)
                    .WithUpdatedDescription("Saved game at " + DateTime.Now);
                    SavedGameMetadataUpdate updatedMetadata = builder.Build();

                    savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
                    m_saving = false;
                }
                else
                {
                    Debug.LogError("Open save game failed, retrying...");
                    SaveGame(savedData, totalPlaytime, callback, ctk);
                }
            });

            await TaskEx.WaitUntil(() => !m_saving, ctk: ctk);
        }

        public static async void LoadSaveGame(Action<SavedGameRequestStatus, byte[]> callback = null)
        {
            if (!Authenticated)
            {
                Debug.Log("Google Play not authenticated");
                return;
            }

            OpenSavedGame((status, game) =>
            {
                ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
                savedGameClient.ReadBinaryData(game, callback);
                Debug.LogError("Loaded save game from Google Play!");
            });
        }

        private static void OpenSavedGame(Action<SavedGameRequestStatus, ISavedGameMetadata> openSaveGameCallback)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(m_saveName, DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, openSaveGameCallback);
        }
    }
#endif
}