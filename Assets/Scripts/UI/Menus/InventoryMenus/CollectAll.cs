using UnityEngine;
using UnityEngine.UI;

public class CollectAll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public Button collectButton;

    public InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        //Button btn = collectButton.GetComponent<Button>();
        //btn.onClick.AddListener(CollectClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectClick()
    {
        inventoryManager.CollectAll();
    }
}
