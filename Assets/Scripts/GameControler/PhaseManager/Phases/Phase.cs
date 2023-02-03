using System.Collections;
using UnityEngine;

public abstract class Phase : ScriptableObject, IPhase
{
    [SerializeField] PhaseType type;

    protected bool HasRan;
    protected bool isResolving;
    protected PhaseManager phaseManager;

    public virtual void Setup(PhaseManager phaseManager)
    {
        this.phaseManager = phaseManager;
    }

    public virtual IEnumerator IsResolving()
    {
        while(isResolving)
            yield return null;
    }

    public abstract IEnumerator PrePhase(Player currentPlayer, Player enemyPlayer);
    public abstract IEnumerator Resolve(Player currentPlayer, Player enemyPlayer);
    public PhaseType GetPhaseType() =>  type;
}