package com.timethivius.update;

import android.app.Activity;
import com.google.android.play.core.appupdate.AppUpdateInfo;
import com.google.android.play.core.appupdate.AppUpdateManager;
import com.google.android.play.core.appupdate.AppUpdateManagerFactory;
import com.google.android.play.core.install.model.AppUpdateType;
import com.google.android.play.core.install.model.UpdateAvailability;
import com.google.android.gms.tasks.OnFailureListener;
import com.google.android.gms.tasks.OnSuccessListener;
import com.google.android.gms.tasks.Task;

public class UpdateManager {

    private static AppUpdateManager appUpdateManager;

    public static void CheckUpdate(Activity activity, final IUpdateStatusListener listener) {

        if (listener != null) {
            listener.onUpdateStatus("CHECKING_STARTED");
        }

        try {
            appUpdateManager = AppUpdateManagerFactory.create(activity);
            Task<AppUpdateInfo> task = appUpdateManager.getAppUpdateInfo();

            task.addOnSuccessListener(new OnSuccessListener<AppUpdateInfo>() {
                @Override
                public void onSuccess(AppUpdateInfo info) {
                    int availability = info.updateAvailability();

                    if (listener != null) {
                        listener.onUpdateStatus("AVAILABILITY_" + availability);
                    }

                    if (availability == UpdateAvailability.UPDATE_AVAILABLE
                            && info.isUpdateTypeAllowed(AppUpdateType.IMMEDIATE)) {
                        if (listener != null) listener.onUpdateStatus("UPDATE_FOUND");
                    }

                    if (availability == UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS) {
                        if (listener != null) listener.onUpdateStatus("UPDATE_IN_PROGRESS");
                    }

                    if (availability == UpdateAvailability.UPDATE_NOT_AVAILABLE) {
                        if (listener != null) listener.onUpdateStatus("NO_UPDATE_AVAILABLE");
                    }
                }
            });

            task.addOnFailureListener(new OnFailureListener() {
                @Override
                public void onFailure(Exception e) {
                    if (listener != null) {
                        listener.onUpdateStatus("ERROR_" + (e.getMessage() != null ? e.getMessage() : "Unknown"));
                    }
                }
            });

        } catch (Exception e) {
            if (listener != null) {
                listener.onUpdateStatus("JAVA_EXCEPTION");
            }
        }
    }

    public static void StartUpdate(Activity activity, final IUpdateStatusListener listener) {

        try {
            if (appUpdateManager == null) {
                appUpdateManager = AppUpdateManagerFactory.create(activity);
            }

            appUpdateManager.getAppUpdateInfo()
                    .addOnSuccessListener(new OnSuccessListener<AppUpdateInfo>() {
                        @Override
                        public void onSuccess(AppUpdateInfo info) {
                            if (info.updateAvailability() == UpdateAvailability.UPDATE_AVAILABLE
                                    && info.isUpdateTypeAllowed(AppUpdateType.IMMEDIATE)) {
                                try {
                                    appUpdateManager.startUpdateFlowForResult(
                                            info,
                                            AppUpdateType.IMMEDIATE,
                                            activity,
                                            1001
                                    );

                                    if (listener != null) listener.onUpdateStatus("UPDATE_STARTED");

                                } catch (Exception e) {
                                    if (listener != null) listener.onUpdateStatus("FLOW_ERROR");
                                }
                            } else {
                                if (listener != null) listener.onUpdateStatus("NO_UPDATE_AVAILABLE");
                            }
                        }
                    });

        } catch (Exception e) {
            if (listener != null) {
                listener.onUpdateStatus("JAVA_EXCEPTION");
            }
        }
    }
}