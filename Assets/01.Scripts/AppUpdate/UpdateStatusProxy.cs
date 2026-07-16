using System;
using UnityEngine;

public class UpdateStatusProxy : AndroidJavaProxy
{
    private readonly Action<string> onStatusChanged;

    public UpdateStatusProxy(Action<string> onStatusChanged) 
        : base("com.timethivius.update.IUpdateStatusListener")
    {
        this.onStatusChanged = onStatusChanged;
    }

    // Java 내부에서 호출할 메서드
    public void onUpdateStatus(string status)
    {
        onStatusChanged?.Invoke(status);
    }
}