//======================================================================>
// 
//======================================================================>    


public abstract class PlayerState
{
    protected PlayerStateMachine fsm;
    protected Player Player;

    public PlayerState(PlayerStateMachine fsm, Player player)
    {
        this.fsm = fsm;
        this.Player = player;
    }

    
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate()
    {
        
    }    

}