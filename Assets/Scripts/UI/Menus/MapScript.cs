using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class MapScript : MenuScreen
{
    public float panSpeed;
    public float zoomSpeed;

    public GameObject localMap;
    public GameObject worldMap;

    public Image localMapImage;
    public Image worldMapImage;
    private Image currentMapImage;

    public GameObject localMarker;
    public GameObject worldMarker;

    //TODO: possibly add additional markers in a list of visible/invisible

    //public GameObject ui;

    private bool mapOpen;

    private LevelManager currentLevel;

    void Start()
    {
        //DeactivateMap(); 
        currentMapImage = localMapImage; // Testing only
    }

    void Update()
    {
        float moveVertical = Input.GetAxisRaw("Vertical");  // TODO: legacy code
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveZoom = Input.mouseScrollDelta.y;

        var mapRect = currentMapImage.GetComponent<RectTransform>();
        mapRect.localPosition = new Vector2(mapRect.localPosition.x - (panSpeed * moveHorizontal), mapRect.localPosition.y - (panSpeed * moveVertical));
        if (moveZoom != 0)
        {
            var priorRectPos = currentMapImage.GetComponent<RectTransform>();
            var yPrior = priorRectPos.anchoredPosition.y / priorRectPos.localScale.y - (priorRectPos.sizeDelta.y / 2);
            var xPrior = -1 * priorRectPos.anchoredPosition.x / priorRectPos.localScale.x - (priorRectPos.sizeDelta.x / 2);

            mapRect.localScale = new Vector3(System.Math.Max(mapRect.localScale.x + (zoomSpeed * moveZoom), 1),
                System.Math.Max(mapRect.localScale.y + (zoomSpeed * moveZoom), 1),
                System.Math.Max(mapRect.localScale.z + (zoomSpeed * moveZoom), 1));

            
            CenterMapOnPoint(new Vector2(xPrior, yPrior));
        }
    }

    public override void DeactivateMenu()
    {
        worldMap.SetActive(false);
        localMap.SetActive(false);
        currentLevel = null;
        mapOpen = false;
    }

    public override void ActivateMenu()
    {
        currentLevel = SceneLoader.Instance.GetLevel();
        ActivateLocalMap();
    }

    public override bool IsActive()
    {
        return mapOpen;
    }

    public override bool overlay => true;

    private void CenterMapOnPoint(Vector2 anchoredPos)
    {
        var currentMapRect = currentMapImage.GetComponent<RectTransform>();
        var rectWidth = currentMapRect.sizeDelta.x;
        var rectHeight = currentMapRect.sizeDelta.y;
        var yCenter = ((rectHeight / 2) + anchoredPos.y) * currentMapRect.localScale.y;
        var xCenter = -1 * ((rectWidth / 2) + anchoredPos.x) * currentMapRect.localScale.x;
        currentMapRect.anchoredPosition = new Vector3(xCenter, yCenter);
    }


    public void ActivateLocalMap()
    {
        worldMap.SetActive(false);
        localMap.SetActive(true);
        currentMapImage = localMapImage;
        currentMapImage.GetComponent<RectTransform>().localScale = new Vector3(2,2,2);
        
        mapOpen = true;

        // TODO: create meaningful relationship between worldspace and map space(?)
        var worldWidth = currentLevel.levelDims[0];
        var worldHeight = currentLevel.levelDims[1];
        localMapImage.sprite = currentLevel.levelMap;
        var playerPos = PartyController.Instance.leader.transform.position;
        var mapWidth = currentLevel.levelMap.rect.size.x;
        var mapHeight = currentLevel.levelMap.rect.size.y;
        localMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(mapWidth * playerPos.x / worldWidth, mapHeight * playerPos.y / worldHeight);
        CenterMapOnPoint(localMarker.GetComponent<RectTransform>().anchoredPosition);
    }


    public void ActivateWorldMap()
    {
        worldMap.SetActive(true);
        localMap.SetActive(false);
        currentMapImage = worldMapImage;
        currentMapImage.GetComponent<RectTransform>().localScale = new Vector3(2,2,2);
        mapOpen = true;

        float xCoord = currentLevel.worldCoords[0];
        float yCoord = currentLevel.worldCoords[1];
        var mapWidth = worldMapImage.sprite.rect.size.x;
        var mapHeight = worldMapImage.sprite.rect.size.y;
        worldMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(xCoord * mapWidth, yCoord * mapHeight, 0);

        CenterMapOnPoint(worldMarker.GetComponent<RectTransform>().anchoredPosition);
    }
}
