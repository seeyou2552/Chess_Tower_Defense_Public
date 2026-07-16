// using UnityEngine;
// using GoogleMobileAds.Api;
// using System;

// public class AdMobManager : Singleton<AdMobManager>
// {
//     [Header("광고 ID")]
//     [TextArea(3, 10)]
//     [SerializeField] private string adUnitId;

//     private RewardedAd rewardAd;

//     private void Start()
//     {
//         MobileAds.Initialize(initStatus =>
//         {

//         });

//         LoadAd();
//     }

//     public void LoadAd()
//     {
//         AdRequest request = new AdRequest();

//         RewardedAd.Load(adUnitId, request,
//             (RewardedAd ad, LoadAdError error) =>
//             {
//                 if (error != null)
//                 {
//                     Debug.LogError(error);
//                     return;
//                 }

//                 rewardAd = ad;
//             });
//     }

//     public bool ShowAd(Action onAdRewarded = null)
//     {
//         if (rewardAd != null)
//         {
//             rewardAd.Show((Reward reward) =>
//             {
//                 onAdRewarded?.Invoke();
//             });
//         }
//         else
//             return false;

//         // 다음 광고 미리 로드
//         rewardAd = null;
//         LoadAd();
            
//         return true;
//     }

    
// }