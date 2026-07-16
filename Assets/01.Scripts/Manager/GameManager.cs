using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    public ShopData ShopData { get; private set;}
    public SettingData SettingData { get; private set;} 
    public GameState GameState { get; private set; }

    async void Awake()
    {
        base.Awake();

        // 프레임 설정
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        
        ShopData = SaveSystem.LoadShopData();
        ShopData.SortShopItemsByMinionIndex();
        await ShopData.InitializeShopItems();

        SettingData = SaveSystem.LoadSettingData();
        ApplySettings();
    }

    void Start()
    {
        GameState = GameState.Lobby;
        LobbySetting();
    }

    public void ChangeGameState(GameState newState)
    {
        GameState = newState;

        switch(GameState)
        {
            case GameState.Wave :
                break;
                
            case GameState.Intermission :
                break;

            case GameState.GameClear :
                GameClear();
                break;

            case GameState.GameOver :
                GameOver();
                break;

            case GameState.GameExit :
                GameExit();
                break;

            case GameState.Lobby :
                SceneFlowManager.Instance.LoadLobbyScene(LobbySetting);
                break;

            case GameState.StageStart :
                GameStart();
                break;

        }
    }

    public void GameStart()
    {
        SceneFlowManager.Instance.LoadStageScene(StageManager.Instance.CurrentStage, StageSetting);
    }

    private void LobbySetting()
    {
        EventBus.Publish(new UIGoldChangedEvent(PlayerManager.Instance.Data.Gold));
        SoundManager.Instance.PlayBGM("LobbyBGM");
    }

    private void StageSetting()
    {
        StageManager.Instance.StartStage();
    }

    public void ApplySettings()
    {
        if (SettingData == null)
            return;

        SoundManager.Instance?.ApplySettings(SettingData);
        AttackRange.SetVisibleForAll(SettingData.IsAtkRangeOn);
    }


    private void GameOver()
    {
        EventBus.Publish(new UIResultEvent(false));
    }

    private void GameClear()
    {
        EventBus.Publish(new UIResultEvent(true));
    }

    private void GameExit()
    {
        SoundManager.Instance.StopBGM();
        Application.Quit();
    }

    [ContextMenu("Test Start")]
    public void TestStart()
    {
        StageManager.Instance.CurrentStage = 1;
        StageManager.Instance.StartStage();
    }

    [ContextMenu("Add Gold")]
    public void AddGold()
    {
        PlayerManager.Instance.AddGold(1000);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        SaveSystem.Save(PlayerManager.Instance.Data);
    }
}
