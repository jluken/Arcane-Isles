using UnityEngine;

public class UICharModel : MonoBehaviour
{
    public GameObject CharacterModel;
    public GameObject CharacterMesh;

    public static UICharModel Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetChar(GameObject characterModel)
    {
        var newModel = Instantiate(characterModel, CharacterModel.transform);
        newModel.GetComponent<SkinnedMeshRenderer>().bones = CharacterMesh.GetComponent<SkinnedMeshRenderer>().bones;
        newModel.GetComponent<SkinnedMeshRenderer>().rootBone = CharacterMesh.GetComponent<SkinnedMeshRenderer>().rootBone;
        Destroy(CharacterMesh);
        CharacterMesh = newModel;
    }
}
