using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleGunner.Core
{
    public class AdsManager : Singleton<AdsManager>
    {
        public static class AdsUnitIdAndroid
        {
            // Free reward
            public static string Reward_Free1 = "ca-app-pub-1285289314306992/8714364894";
            public static string Reward_Free2 = "ca-app-pub-1285289314306992/3838861017";
            public static string Reward_Free3 = "ca-app-pub-1285289314306992/7007949837";
            public static string Reward_Free4 = "ca-app-pub-1285289314306992/9925757216";
            public static string Reward_Free5 = "ca-app-pub-1285289314306992/4868602156";

            // Gameplay
            public static string Reward_GameRevive = "ca-app-pub-1285289314306992/7946920585";
            public static string Reward_ClaimMoreGold = "ca-app-pub-1285289314306992/4341988548";
            public static string Interstitial_GameEnd = "ca-app-pub-1285289314306992/3756108723";

            // Free Gold
            public static string Reward_FreeGoldPerDay = "ca-app-pub-1285289314306992/1480021483";
            public static string Reward_FreeDiamondPerDay = "ca-app-pub-1285289314306992/1991157373";
            public static string Reward_FreeTicketPerDay = "ca-app-pub-1285289314306992/9704080140";
        }

        public class AdsUnitTest
        {
            // Test Unit Ads Id
            public static string Reward_Test = "ca-app-pub-3940256099942544/5224354917";
            public static string Interstitial_Test = "ca-app-pub-3940256099942544/1033173712";
        }

        private void Start()
        {
            MobileAds.Initialize(initStatus => 
            {
            });
        }

        /// <summary>
        ///  Create Interstitial ad. *Note: Remember to show the ads
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="OnAdLoadedCallback"></param>
        /// <param name="OnAdClosedCallback"></param>
        /// <param name="OnAdOpeningCallback"></param>
        /// <param name="OnAdFailedToLoadCallback"></param>
        /// <param name="OnAdFailedToShowCallback"></param>
        /// <param name="OnAdDidRecordImpressionCallback"></param>
        /// <param name="OnPaidEventCallback"></param>
        /// <returns></returns>
        public static InterstitialAd CreateAndLoadInterstitialAd(string adUnitId, EventHandler<EventArgs> OnAdLoadedCallback = null,
                                                                           EventHandler<EventArgs> OnAdClosedCallback = null,
                                                                           EventHandler<EventArgs> OnAdOpeningCallback = null,
                                                                           EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoadCallback = null,
                                                                           EventHandler<AdErrorEventArgs> OnAdFailedToShowCallback = null,
                                                                           EventHandler<EventArgs> OnAdDidRecordImpressionCallback = null,
                                                                           EventHandler<AdValueEventArgs> OnPaidEventCallback = null)
        {
            InterstitialAd interstitialAd = new InterstitialAd(adUnitId);

            interstitialAd.OnAdLoaded += OnAdLoadedCallback;
            interstitialAd.OnAdOpening += OnAdOpeningCallback;
            interstitialAd.OnAdClosed += OnAdClosedCallback;
            interstitialAd.OnAdFailedToLoad += OnAdFailedToLoadCallback;
            interstitialAd.OnAdFailedToShow += OnAdFailedToShowCallback;
            interstitialAd.OnAdDidRecordImpression += OnAdDidRecordImpressionCallback;
            interstitialAd.OnPaidEvent += OnPaidEventCallback;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            interstitialAd.LoadAd(request);
            return interstitialAd;
        }

        /// <summary>
        ///  Create Rewarded ad. *Note: Remember to show the ads
        /// </summary>
        /// <param name="adUnitId"></param>
        /// <param name="OnAdLoadedCallback"></param>
        /// <param name="OnUserEarnedRewardCallback"></param>
        /// <param name="OnAdOpeningCallback"></param>
        /// <param name="OnAdClosedCallback"></param>
        /// <param name="OnAdFailedToLoadCallback"></param>
        /// <param name="OnAdFailedToShowCallback"></param>
        /// <param name="OnAdDidRecordImpressionCallback"></param>
        /// <param name="OnPaidEventCallback"></param>
        /// <returns></returns>
        public static RewardedAd CreateAndLoadRewardedAd(string adUnitId, EventHandler<EventArgs> OnAdLoadedCallback = null, 
                                                                   EventHandler<Reward> OnUserEarnedRewardCallback = null,
                                                                   EventHandler<EventArgs> OnAdOpeningCallback = null,
                                                                   EventHandler<EventArgs> OnAdClosedCallback = null,
                                                                   EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoadCallback = null,
                                                                   EventHandler<AdErrorEventArgs> OnAdFailedToShowCallback = null,
                                                                   EventHandler<EventArgs> OnAdDidRecordImpressionCallback = null,
                                                                   EventHandler<AdValueEventArgs> OnPaidEventCallback = null)
        {
            RewardedAd rewardedAd = new RewardedAd(adUnitId);

            rewardedAd.OnAdLoaded += OnAdLoadedCallback;
            rewardedAd.OnAdOpening += OnAdOpeningCallback;
            rewardedAd.OnUserEarnedReward += OnUserEarnedRewardCallback;
            rewardedAd.OnAdClosed += OnAdClosedCallback;
            rewardedAd.OnAdFailedToLoad += OnAdFailedToLoadCallback;
            rewardedAd.OnAdFailedToShow += OnAdFailedToShowCallback;
            rewardedAd.OnAdDidRecordImpression += OnAdDidRecordImpressionCallback;
            rewardedAd.OnPaidEvent += OnPaidEventCallback;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            rewardedAd.LoadAd(request);
            return rewardedAd;
        }

    }
}