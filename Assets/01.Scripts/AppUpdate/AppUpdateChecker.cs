using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppUpdateChecker : MonoBehaviour
{
    private AndroidJavaObject _currentActivity;
    private AndroidJavaClass _updateManagerClass;
    private UpdateStatusProxy _updateStatusProxy;

    // 백그라운드 스레드에서 들어오는 상태 값을 메인 스레드로 안전하게 넘기기 위한 큐
    private readonly Queue<string> _statusQueue = new Queue<string>();

    void Awake()
    {
        // 런타임 플랫폼이 안드로이드일 때만 네이티브 오브젝트 미리 캐싱 (호출 비용 최소화)
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    _currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                
                _updateManagerClass = new AndroidJavaClass("com.timethivius.update.UpdateManager");
                
                // C# 콜백 메서드를 프록시에 바인딩
                _updateStatusProxy = new UpdateStatusProxy(OnUpdateStatusReceivedInBackground);
            }
            catch (Exception e)
            {
                Debug.LogError($"[AppUpdateChecker] 초기화 실패: {e.Message}");
            }
        }
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            CheckForUpdate();
        }
    }

    void Update()
    {
        // 백그라운드 스레드에서 수신된 상태가 있다면 메인 스레드 프레임에서 안전하게 꺼내 처리
        string statusToProcess = null;

        lock (_statusQueue)
        {
            if (_statusQueue.Count > 0)
            {
                statusToProcess = _statusQueue.Dequeue();
            }
        }

        if (statusToProcess != null)
        {
            ProcessUpdateStatus(statusToProcess);
        }
    }

    public void CheckForUpdate()
    {
        try
        {
            if (_updateManagerClass != null && _currentActivity != null)
            {
                // 인자로 프록시(리스너) 객체를 함께 전달
                _updateManagerClass.CallStatic("CheckUpdate", _currentActivity, _updateStatusProxy);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("업데이트 체크 실패: " + e.Message);
        }
    }

    private void OnUpdateStatusReceivedInBackground(string status)
    {
        lock (_statusQueue)
        {
            _statusQueue.Enqueue(status); // 메인 스레드에서 처리하도록 큐에 삽입
        }
    }

    // UI 및 이벤트 처리 로직
    private void ProcessUpdateStatus(string status)
    {
        if (status == "UPDATE_FOUND")
        {
            EventBus.Publish(new UISelectEvent(StartUpdate, "업데이트가 있습니다. 업데이트 하시겠습니까?"));
        }
        else if (status == "NO_UPDATE_AVAILABLE")
        {
            EventBus.Publish(new UIVersionEvent("최신 버전입니다."));
        }
        else if (status == "UPDATE_IN_PROGRESS")
        {
            EventBus.Publish(new UIVersionEvent("업데이트 진행 중입니다."));
        }
        else if (status.StartsWith("ERROR_") || status == "FLOW_ERROR" || status == "JAVA_EXCEPTION")
        {
            EventBus.Publish(new UIVersionEvent("업데이트 확인 실패"));
        }
    }

    private void StartUpdate()
    {
        try
        {
            if (_updateManagerClass != null && _currentActivity != null)
            {
                // StartUpdate를 실행할 때도 필요한 경우 리스너를 전달해 추적
                _updateManagerClass.CallStatic("StartUpdate", _currentActivity, _updateStatusProxy);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"업데이트 시작 실패: {e.Message}");
        }
    }
}