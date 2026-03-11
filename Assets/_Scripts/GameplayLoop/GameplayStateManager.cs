using System.Collections;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleStateManager : Singleton<BattleStateManager>
{
    public bool GameOver = false;
    private IState _currentState;


    //States
    private PreparationPhase _prepPhase = new PreparationPhase();
    private ResolutionPhase _resolutionPhase = new ResolutionPhase();

    public IState CurrentState => _currentState;
    public PreparationPhase PreparationPhase => _prepPhase;
    public ResolutionPhase ResolutionPhase => _resolutionPhase;

    // ----------------------------------------------------

    void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(IState state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }
}
