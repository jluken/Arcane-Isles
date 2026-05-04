using UnityEngine;

public class CharStateMachine
{
    public CharState CurrentPlayerState { get; private set; }

    public void Initialize(CharState startingState)
    {
        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(CharState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }
}
