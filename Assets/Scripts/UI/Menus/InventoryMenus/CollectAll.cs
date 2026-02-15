using UnityEngine;
using UnityEngine.UI;

public class CollectAll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public Button collectButton;

    //public ContainerInventoryMenu inventoryMenu;

    void Start()
    {
        //inventoryMenu = ContainerInventoryMenu.Instance;
        //Button btn = collectButton.GetComponent<Button>();
        //btn.onClick.AddListener(CollectClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollectClick() // TODO: does this need to be its own class and not just part of the menu file?
    {
        ContainerInventoryMenu.Instance.CollectAll();
    }
}
