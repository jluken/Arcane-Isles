using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NavMapScript : MenuScreen
{
    // TODO: handle default zoom differently if at sea vs on land
    public float panSpeed;
    public float zoomSpeed;

    public GameObject worldMap;

    public static NavMapScript Instance;

    public RectTransform mapWindow;
    public RectTransform worldMapContent;

    //public GameObject ui;

    private bool mapOpen;

    public void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct LevelMapLocation
    {
        public string levelName;
        public MapLoc loc;
    }

    public LevelMapLocation[] levelLocations;

    public MapLoc GetMapLoc(string levelName)
    {
        return levelLocations.FirstOrDefault(levelMapLoc => levelMapLoc.levelName == levelName).loc;
    }

    void Update()
    {
        Vector2 moveDirection = InputSystem.actions.FindActionMap("UI").FindAction("Navigate").ReadValue<Vector2>();
        float zoom = SelectionController.MouseScroll().y;

        worldMapContent.localPosition = new Vector2(worldMapContent.localPosition.x - (panSpeed * moveDirection.x), worldMapContent.localPosition.y - (panSpeed * moveDirection.y));
        if (zoom < 0 || (zoom > 0 && worldMapContent.GetComponent<RectTransform>().localScale.x < 4))  // TODO: set max map zoom
        {
            Vector2 mousePosition = SelectionController.MousePosition();
            bool inBounds = mousePosition.y >= 0 && mousePosition.y <= Screen.height && mousePosition.x >= 0 && mousePosition.x <= Screen.width;  // TODO: dupe code with camscript
            if (!inBounds) return;

            var yPrior = -1 * (worldMapContent.anchoredPosition.y + (mapWindow.rect.height / 2)) / worldMapContent.localScale.y;
            var xPrior = ((-1 * worldMapContent.anchoredPosition.x) + (mapWindow.rect.width / 2)) / worldMapContent.localScale.x;

            worldMapContent.localScale = new Vector3(System.Math.Max(worldMapContent.localScale.x + (zoomSpeed * zoom), 0.5f),
                System.Math.Max(worldMapContent.localScale.y + (zoomSpeed * zoom), 0.5f),
                System.Math.Max(worldMapContent.localScale.z + (zoomSpeed * zoom), 0.5f));

            
            CenterMapOnPoint(new Vector2(xPrior, yPrior));
        }
    }

    public override void DeactivateMenu()
    {
        worldMap.SetActive(false);;
        mapOpen = false;
    }

    public override void ActivateMenu()
    {
        ActivateWorldMap();
        mapOpen = true;
    }

    public override bool IsActive()
    {
        return mapOpen;
    }

    public void CenterMapOnPoint(Vector2 anchoredPos)
    {
        var rectWidth = mapWindow.rect.width;
        var rectHeight = mapWindow.rect.height;
        var yCenter = (anchoredPos.y) * worldMapContent.localScale.y;
        var xCenter = ( anchoredPos.x) * worldMapContent.localScale.x;

        yCenter = ((-1 * anchoredPos.y * worldMapContent.localScale.y) - (rectHeight / 2));
        xCenter = -1 * ((anchoredPos.x * worldMapContent.localScale.x) - (rectWidth / 2));
        worldMapContent.anchoredPosition = new Vector2(xCenter, yCenter);
        //worldMapContent.anchoredPosition = new Vector3(anchoredPos.x * worldMapContent.sizeDelta.x, anchoredPos.y * worldMapContent.sizeDelta.y);
    }


    public void ActivateWorldMap()
    {
        worldMap.SetActive(true);
        worldMapContent.localScale = new Vector3(1,1,1);  // TODO: don't hardcode zoom (based on sea vs land)
        mapOpen = true;

        worldMapContent.GetComponent<NavMapPage>().SetStartPos();
    }

    
}
