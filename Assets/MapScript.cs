using System.Data;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public GameObject localMap;
    public GameObject worldMap;

    public GameObject player; // TODO: these assignments will be handled with a controller once swapping players is functional
    public GameObject localMarker;
    public GameObject worldMarker;

    public GameObject ui;

    private bool mapOpen;

    void Start()
    {
        DeactivateMap(); 
    }

    void Update()
    {
        if (!mapOpen && Input.GetKeyDown(KeyCode.M))
        {
            ActivateLocalMap();
        }
        else if (mapOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M)))
        {
            DeactivateMap();
        }
    }

    public void DeactivateMap()
    {
        worldMap.SetActive(false);
        localMap.SetActive(false);
        mapOpen = false;
        ui.GetComponent<UIScript>().ActivateUI();
    }

    public void ActivateLocalMap()
    {
        worldMap.SetActive(false);
        localMap.SetActive(true);
        ui.GetComponent<UIScript>().DeactivateUI();
        mapOpen = true;

        // TODO: create meaningful relationship between worldspace and map space
        var worldWidth = 20;
        var worldHeight = 20;
        var mapWidth = Screen.width;
        var mapHeight = Screen.height;
        localMarker.transform.localPosition = new Vector3(mapWidth * player.transform.position.x / worldWidth, mapHeight * player.transform.position.y / worldHeight);
    }
    public void ActivateWorldMap()
    {
        worldMap.SetActive(true);
        localMap.SetActive(false);
        ui.GetComponent<UIScript>().DeactivateUI();
        mapOpen = true;
        float xCoord = 0.0f;  // TODO: get coord from scene data
        float yCoord = 0.0f;
        worldMarker.transform.localPosition = new Vector3(xCoord * Screen.width, yCoord * Screen.height);
    }
}
