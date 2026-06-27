using UnityEngine;

[CreateAssetMenu(fileName = "Bomb", menuName = "Scriptable Objects/Test Sigil")]
public class TestSigil: Sigil
{
    public override AbilityAction action => new TestAction(icon: sprite);
}
