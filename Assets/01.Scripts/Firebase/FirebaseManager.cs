// using Firebase;
// using Firebase.Analytics;
// using UnityEngine;

// public class FirebaseManager : Singleton<FirebaseManager>
// {
//     private void Start()
//     {
//         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//         {
//             var status = task.Result;

//             if (status == DependencyStatus.Available)
//             {
//                 Debug.Log("Firebase 초기화 완료");
//                 FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
//                 FirebaseAnalytics.LogEvent("game_start");
//             }
//             else
//             {
//                 Debug.LogError("Firebase 초기화 실패: " + status);
//             }
//         });
//     }

//     public void LogEvent(string eventName, params Parameter[] parameters)
//     {
//         FirebaseAnalytics.LogEvent(eventName, parameters);
//     }
// }