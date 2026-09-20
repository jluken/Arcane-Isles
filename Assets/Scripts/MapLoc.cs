using UnityEngine;

public class MapLoc : MonoBehaviour
{
    public NavMapPage mapPage;
    public string levelName;
    public int spawnPt;

    public void EnterLoc()
    {
        if (levelName != null)
        {  // TODO: some places might not load a level (deal with custom behavior later)
            //StartCoroutine(SceneLoader.Instance.ActivateLevel(levelName, spawnPt));  
            SceneLoader.Instance.SetToLevelSpawn(levelName, spawnPt); // TODO: handle multiple entry points
        }
    }

    public void SetMapDest()
    {
        mapPage.SetDestTarget(this);
    }

    public Vector2 MapPos()
    {
        Debug.Log("Getting map pos " + GetComponent<RectTransform>().anchoredPosition);
        return GetComponent<RectTransform>().anchoredPosition;
    }
}
