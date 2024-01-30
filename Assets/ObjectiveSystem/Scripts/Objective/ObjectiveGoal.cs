[System.Serializable]
public class ObjectiveGoal {

    public ObjectiveType objType;

    public int requiredAmount = 1;
    public int currentAmount;

    /// <summary>
    /// Function that returns the state of the Quest
    /// </summary>
    /// <returns><para>True: If the quest is completed;</para>False: If the quest is still active;</returns>
    public bool IsCompleted() {
        return (currentAmount >= requiredAmount);
    }

}
