
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionController : Singleton<ProgressionController>
{
    [SerializeField] float _cardUpgradeChanceIncrement = 0.05f;
    [SerializeField] float _scoreMultiplierIncrement = 0.2f;

    [NonSerialized] public float CardUpgradeChance = 0f;
    [NonSerialized] public float ScoreMultiplier = 1f;
    [NonSerialized] public RoomData CurrentRoomData;
    
    public List<RoomData> RoomSequence =  new();
    [Tooltip ("FOR DEBUGGING ONLY! Should be set to 0 for build.")]
    [SerializeField] int _roomIndex = 0;

    [SerializeField] TextMeshProUGUI _multiplierText;

    private int _highestRoomReached;
    private PerkPopup _perkPopup;

    public override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(this);    //This needs to persist btwn scene reloads!
        _highestRoomReached = _roomIndex;
    }

    void Start()
    {
        SetSceneData(RoomSequence[_roomIndex], false);
    }

    private void SetSceneData(RoomData roomData, bool reloadScene = true)
    {
        // Get the index of the current active scene and reload it
        if(reloadScene)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
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
                child.material.SetColor("Base Color", roomData.WallColor);
            }
        } 
        else 
            Debug.LogWarning("[ProgressionController] could not find thePlayRoomWalls parent object!");
    }

    public void OnRoomComplete()
    {
        if(_roomIndex == RoomSequence.Count - 1)
        {
            //TODO: WINNER !!
        } else 
        {
            _roomIndex++;
            if(_highestRoomReached < _roomIndex)
            {
                _highestRoomReached++;
                _perkPopup?.UpdateAndOpen(CardUpgradeChance + _cardUpgradeChanceIncrement, ScoreMultiplier + _scoreMultiplierIncrement);
            }

        }
    }

    public void OnRoomFailed()
    {
        if(_roomIndex == 0)
        {
            SetSceneData(CurrentRoomData);   //try the bottom floor again
        }
        else
        {
            _roomIndex--;
            SetSceneData(RoomSequence[_roomIndex]);
        }
    }

    public void ContinueToNextRoom()
    {
        SetSceneData(RoomSequence[_roomIndex]);
        _perkPopup?.gameObject.SetActive(false);
    }

    public void UpgradeRarityPerk()
    {
        CardUpgradeChance += _cardUpgradeChanceIncrement;
    }

    public void UpgradeMultiplierPerk()
    {
        ScoreMultiplier += _scoreMultiplierIncrement;
        _multiplierText.text = $"{ScoreMultiplier}x";
    }
}
