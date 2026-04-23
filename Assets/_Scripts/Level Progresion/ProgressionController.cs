
using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ProgressionController : Singleton<ProgressionController>
{
    private enum TransitionType
    {
        Stay,
        Next,
        Prev
    }

    [SerializeField] float _cardUpgradeChanceIncrement = 0.05f;
    [SerializeField] float _scoreMultiplierIncrement = 0.2f;

    [NonSerialized] public float CardUpgradeChance = 0f;
    [NonSerialized] public float ScoreMultiplier = 1f;
    [NonSerialized] public RoomData CurrentRoomData;

    [SerializeField] string GameSceneName;
    
    public List<RoomData> RoomSequence =  new();
    [Tooltip ("FOR DEBUGGING ONLY! Should be set to 0 for build.")]
    [SerializeField] int _roomIndex = 0;

    private int _highestRoomReached;

    private void Update()   //FOR DEBUGGING< TO DELETE
    {
        if (Keyboard.current.pageUpKey.wasPressedThisFrame)
        {
            OnRoomComplete();
        }
    }
    public override void Awake()
    {
        base.Awake();
        ShouldDieOnReload = false;
        DontDestroyOnLoad(this);    //This needs to persist btwn scene reloads!
        _highestRoomReached = _roomIndex;
    }

    void Start()
    {
        SetSceneData(RoomSequence[_roomIndex], TransitionType.Stay, false);
    }

    private void SetSceneData(RoomData roomData, TransitionType transitionType, bool reloadScene = true)
    {
        //yield return null;
        // Get the index of the current active scene and reload it
        if (reloadScene)
        {
            //GameplayManager.Instance.ResetScene();
            var all = FindObjectsByType<SingletonBase>(FindObjectsSortMode.None);
            foreach(var s in all)
            {
                if(s.ShouldDieOnReload) 
                    Destroy(s);             //make sure they are actually dying
            }

            SceneManager.LoadScene(GameSceneName);
        }

        CurrentRoomData = roomData;

        //1. Set enemy AI threshold
        GameplayManager.Instance.Enemy.SetAIThreshold(roomData.EnemyIntelligenceThreshold);

        //2. Set wall color
        var wallsParent = GameObject.Find("PlayRoomWalls"); 
        if (wallsParent != null)
        {
            Renderer[] childRenderers = wallsParent.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer child in childRenderers)
            {
                child.material.SetColor("_BaseColor", roomData.WallColor);
            }
        } 
        else 
            Debug.LogWarning("[ProgressionController] could not find thePlayRoomWalls parent object!");

        //3. Other UI stuff
        ProgressionUI.Instance.ScoreText.text = $"{ScoreMultiplier}x";

        var num = CurrentRoomData.FloorNumber;
        switch (transitionType)
        {
            case(TransitionType.Next):
            ProgressionUI.Instance.ProgressScreen?.NextFloor(num, num-1);
            break;
            case(TransitionType.Prev):
            ProgressionUI.Instance.ProgressScreen?.PreviousFloor(num, num + 1);
            break;
            case(TransitionType.Stay):
            ProgressionUI.Instance.ProgressScreen?.ShowCurrentFloor(num);
            break;
        }
    }

    public void OnRoomComplete()
    {
        Debug.Log($"[ProgressionController] Moving onto Next room");
        if(_roomIndex == RoomSequence.Count - 1)
        {
            //TODO: WINNER !!
        } else 
        {
            _roomIndex++;
            if(_highestRoomReached < _roomIndex)
            {
                _highestRoomReached++;
                ProgressionUI.Instance.PerkPopup?.UpdateAndOpen(CardUpgradeChance + _cardUpgradeChanceIncrement, ScoreMultiplier + _scoreMultiplierIncrement);
            }

        }
    }

    public void OnRoomFailed()
    {
        if(_roomIndex == 0)
        {
            SetSceneData(CurrentRoomData, TransitionType.Stay);   //try the bottom floor again
            //ProgressionUI.Instance.ProgressScreen?.ShowCurrentFloor(CurrentRoomData.FloorNumber);
        }
        else
        {
            _roomIndex--;
            SetSceneData(RoomSequence[_roomIndex], TransitionType.Prev);
            //ProgressionUI.Instance.ProgressScreen?.PreviousFloor(RoomSequence[_roomIndex].FloorNumber, RoomSequence[_roomIndex+1].FloorNumber);
        }
    }

    public void ContinueToNextRoom()
    {
        SetSceneData(RoomSequence[_roomIndex],TransitionType.Next);
        //ProgressionUI.Instance.ProgressScreen?.NextFloor(CurrentRoomData.FloorNumber, RoomSequence[_roomIndex-1].FloorNumber);
        //ProgressionUI.Instance.PerkPopup?.gameObject.SetActive(false);
    }

    public void UpgradeRarityPerk()
    {
        CardUpgradeChance += _cardUpgradeChanceIncrement;
    }

    public void UpgradeMultiplierPerk()
    {
        ScoreMultiplier += _scoreMultiplierIncrement;
        ProgressionUI.Instance.ScoreText.text = $"{ScoreMultiplier}x";
    }
}
