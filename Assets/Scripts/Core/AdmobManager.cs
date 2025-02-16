using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
namespace Game.Core
{
    public class AdmobManager : MonoSingelton<AdmobManager>
    {
        private const string ANDROID_REWARDED_AD_UNIT_ID = "ca-app-pub-5429568779857286/6633053078";

        public Action<AdValue> OnAdPaid;
        public Action OnAdImpressionRecorded;
        public Action OnAdClicked;
        public Action OnAdFullScreenContentOpened;
        public Action OnAdFullScreenContentClosed;
        private RewardedAd rewardedAd;

        private bool isFullyInitialized = false;

        public bool IsFullyInitialized { get => isFullyInitialized; }

        public void ShowRewardedAd()
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                Debug.Log("Showing rewarded ad.");
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log(String.Format("Rewarded ad granted a reward: {0} {1}",
                                            reward.Amount,
                                            reward.Type));
                });
            }
            else
            {
                Debug.LogError("Rewarded ad is not ready yet.");
            }
        }

        protected override void InternalInit()
        {
            
        }

        protected override void InternalOnStart()
        {
            isFullyInitialized = false;
            MobileAds.Initialize((InitializationStatus status)=>
            {
                Debug.Log("Initialized mobile ads");
                isFullyInitialized = true;
                LoadRewaredAd();
            });
        }

        protected override void InternalOnDestroy()
        {
            
        }

        private void DestroyAd()
        {
            if(rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
        }

        private void RegisterEventCallbacks(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));

                OnAdPaid?.Invoke(adValue);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
                OnAdImpressionRecorded?.Invoke();
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
                OnAdClicked?.Invoke();
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
                OnAdFullScreenContentOpened?.Invoke();
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
                OnAdFullScreenContentClosed?.Invoke();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);
            };
        }

        private void LoadRewaredAd()
        {
            if(rewardedAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading rewarded ad.");

            var adRequest = new AdRequest();

            RewardedAd.Load(ANDROID_REWARDED_AD_UNIT_ID, adRequest, (RewardedAd ad, LoadAdError error) => 
            {
                if(error != null)
                {
                    Debug.LogError("Failed to load rewarded ad with error: \n" + error);
                }

                if(ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error");
                    return;
                }

                Debug.Log("Rewaded ad loaded with response: " + ad.GetResponseInfo());
                rewardedAd = ad;

                RegisterEventCallbacks(ad);

            });
        }
    }
}
