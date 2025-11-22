using UnityEngine;
using static CharStats;

[CreateAssetMenu(fileName = "AbilityAction", menuName = "Scriptable Objects/AbilityAction")]
public abstract class AbilityAction : ScriptableObject
{
    public string attackName;
    public Sprite icon;
    public int damageDie;
    public StatVal modifier;
    public float range;

    public abstract bool CheckValidTarget(NPC actor, Selectable target);

    public abstract int UseAbility(NPC actor, Selectable target);
}
