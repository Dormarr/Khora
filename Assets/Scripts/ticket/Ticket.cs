using System;

public enum ActionType{
    Load,
    Unload,
    Tick,
    Update
    // Add more generalised actions as needed.
}

public abstract class Ticket
{
    public Guid TicketID { get; private set; } // Unique ticket ID, to track if necessary.
    public int Priority { get; private set; } // Numerical priority. Define in notes.
    public ActionType ActionType { get; private set; } // Use to categorise and filter.
    public float TargetExecutionTime { get; private set; } // For delayed execution. Maybe scheduling?

    protected Ticket(int priority, ActionType actionType, float targetExectuionTime = 0f){
        TicketID = Guid.NewGuid();
        Priority = priority;
        ActionType = actionType;
        TargetExecutionTime = targetExectuionTime;
    }

    public abstract void Execute();
}
