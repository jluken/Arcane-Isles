using UnityEngine;

public class HumanoidSfx : MonoBehaviour
{
    public AudioManager charAudio;

    public void Step()
    {
        charAudio.PlaySandStep();
    }
}
