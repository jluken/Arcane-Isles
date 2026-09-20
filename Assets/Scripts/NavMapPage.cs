using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NavMapPage : MonoBehaviour, IPointerClickHandler
{
    public NavMapScript navMapCanvas;
    public GameObject destMarker;
    public GameObject mapMarker;

    public RawImage mapShadow;
    private Color32[] shadowPixels;
    public RawImage mapTerrain;
    private Color32[] terrainPixels;
    public RawImage mapKey;

    public float travelSpeed;  // TODO: Handle conversion from meaningful units

    public int viewRange = 300; // TODO: make meaningful and alterable based on conditions

    private bool destSet = false;
    private Vector2 dest;

    private float mapWidth;
    private float mapHeight;

    private MapLoc currTarget;

    private static readonly string[] keyVals = new string[] {
        "Land",
        "Ocean",
    };
    private static readonly int keySquareSize = 2;
    private static readonly string[] accessibleTerrainNames = { "Land" };

    private static Color32[] accessibleTerrains;

    public void Awake()
    {
        //Create new copy of shadow
        Texture2D newShadow = new Texture2D(mapShadow.texture.width, mapShadow.texture.height);
        newShadow.SetPixels((((Texture2D)mapShadow.texture).GetPixels()));
        newShadow.Apply();
        mapShadow.texture = newShadow;

        terrainPixels = ((Texture2D)mapTerrain.texture).GetPixels32();
        shadowPixels = ((Texture2D)mapShadow.texture).GetPixels32();
        accessibleTerrains = accessibleTerrainNames.Select(name => ((Texture2D)mapKey.texture).GetPixels32()[Array.IndexOf(keyVals, name) * keySquareSize * keySquareSize]).ToArray();
    }

    public void Start()
    {
        

        
    }

    public void Update()
    {
        if (destSet)
        {
            // TODO: maybe add basic navigation (or maybe player needs to choose path around obstacles)
            var currPos = PartyController.Instance.worldMapPos; // TODO: come up with cleaner way to handle conversion
            var destVect = dest - currPos;  
            var hypotenuse = (float)Math.Sqrt(Math.Pow(destVect.x, 2) + Math.Pow(destVect.y, 2));
            var vectNorm = destVect / hypotenuse;

            var newMapPos = currPos + vectNorm * travelSpeed * Time.unscaledDeltaTime;
            //var newMapY = currYPos + yNorm * travelSpeed * Time.unscaledDeltaTime;

            if (AccessibleTerrain(newMapPos))
            {
                GameData.Instance.gameTime += Time.unscaledDeltaTime * 600; // TODO: figure out actual travel time

                PartyController.Instance.worldMapPos = newMapPos;

                SetMarker(newMapPos);
                navMapCanvas.CenterMapOnPoint(mapMarker.GetComponent<RectTransform>().anchoredPosition);
            }
            else {
                destMarker.SetActive(false);
                destSet = false;
            }
        }
        if (destSet && Utils.AlmostEqual(dest.x, PartyController.Instance.worldMapPos.x, 0.01f) && Utils.AlmostEqual(dest.y, PartyController.Instance.worldMapPos.y, 0.01f))
        {
            Debug.Log("Travel arrived");
            destMarker.SetActive(false);
            destSet = false;
            if (currTarget != null) currTarget.EnterLoc();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
        {
            Debug.Log("Local point: " + localPoint);
            Debug.Log("click point: " + eventData.position);
            currTarget = null;
            destMarker.SetActive(true);
            destMarker.GetComponent<RectTransform>().anchoredPosition = localPoint;
            Travel(localPoint);
        }
    }

    public void SetDestTarget(MapLoc target)
    {
        currTarget = target;
        var targetPt = target.GetComponent<RectTransform>().anchoredPosition;
        destMarker.SetActive(true);
        destMarker.GetComponent<RectTransform>().anchoredPosition = targetPt;
        Travel(targetPt);
    }

    public void SetStartPos()
    {
        mapWidth = GetComponent<RectTransform>().sizeDelta.x;
        mapHeight = GetComponent<RectTransform>().sizeDelta.y;

        float xCoord = PartyController.Instance.worldMapPos[0];
        float yCoord = PartyController.Instance.worldMapPos[1];
        SetMarker(PartyController.Instance.worldMapPos);
        destMarker.SetActive(false);
    }
    public void Travel(Vector2 dest)
    {
        destSet = true;
        this.dest = dest;
        Debug.Log("Travel set");
    }

    private Vector2Int MapToPixel(Texture2D mapTexture, Vector2 mapPos)
    {
        int pixelWidth = mapTexture.width;
        int pixelHeight = mapTexture.height;

        int centerX = (int)(pixelWidth * mapPos.x / mapShadow.GetComponent<RectTransform>().rect.width);
        int centerY = ((int)(pixelHeight * mapPos.y / mapShadow.GetComponent<RectTransform>().rect.height) + mapTexture.height + 1);  // textures count from bottom

        return new Vector2Int(centerX, centerY);
    }

    private bool AccessibleTerrain(Vector2 pos)
    {
        var terrainTexture = (Texture2D)mapTerrain.texture;

        var pixelCenter = MapToPixel(terrainTexture, pos);
        var pixIdx = pixelCenter.x + (pixelCenter.y * terrainTexture.width);

        var pixTerrain = terrainPixels[pixIdx];
        if (accessibleTerrains.Any(terr => Utils.CompareColor32(pixTerrain, terr))) return true;
        return false;
    }

    public void SetMarker(Vector2 dest)
    {
        mapMarker.GetComponent<RectTransform>().anchoredPosition = dest;
        navMapCanvas.CenterMapOnPoint(mapMarker.GetComponent<RectTransform>().anchoredPosition);

        var shadowTexture = ((Texture2D)mapShadow.texture);

        int pixelWidth = shadowTexture.width;
        int pixelHeight = shadowTexture.height;

        var pixelCenter = MapToPixel(shadowTexture, dest);

        int minX = Mathf.Clamp(pixelCenter.x - viewRange, 0, pixelWidth - 1);
        int maxX = Mathf.Clamp(pixelCenter.x + viewRange, 0, pixelWidth - 1);
        int minY = Mathf.Clamp(pixelCenter.y - viewRange, 0, pixelHeight - 1);
        int maxY = Mathf.Clamp(pixelCenter.y + viewRange, 0, pixelHeight - 1);

        int r2 = viewRange * viewRange;
        //int fastIdx = 0;
        var fastPixels = new List<Color32>();
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2Int currentPixel = new Vector2Int(x, y);
                float sqrDistance = (currentPixel - pixelCenter).sqrMagnitude;
                var pixIdx = x + (y * pixelWidth);
                if (sqrDistance <= r2)
                {
                    //pix.a = 0;  
                    fastPixels.Add(Color.clear);// TODO: eventually look at blurred edges
                    shadowPixels[pixIdx] = Color.clear;
                }
                else fastPixels.Add(shadowPixels[pixIdx]);
            }
        }
        shadowTexture.SetPixels32(minX, minY, 1 + maxX - minX, 1 + maxY - minY, fastPixels.ToArray());  // TODO: for speed, only submit the block
        shadowTexture.Apply();
        mapShadow.texture = shadowTexture;
    }
}
