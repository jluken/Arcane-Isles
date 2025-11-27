using UnityEngine;

public class NPCStateMachine
{
    public NPCState CurrentPlayerState { get; set; }

    public void Initialize(NPCState startingState)
    {
        CurrentPlayerState = startingState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(NPCState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }

}
