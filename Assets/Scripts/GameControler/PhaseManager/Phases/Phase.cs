using System.Collections;
using UnityEngine;

public abstract class Phase : ScriptableObject, IPhase
{
    [SerializeField] PhaseType type;

    [HideInInspector] protected bool HasRan;
    [HideInInspector] protected bool isResolving;
    [HideInInspector] protected PhaseManager phaseManager;
    [HideInInspector] protected Battlefield battlefield;

    public virtual void Setup(PhaseManager phaseManager, Battlefield battlefield)
    {
        this.phaseManager = phaseManager;
        this.battlefield = battlefield;
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