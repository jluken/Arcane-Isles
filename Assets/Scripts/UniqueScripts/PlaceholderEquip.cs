using Language.Lua;
using UnityEngine;

public class PlaceholderEquip : MonoBehaviour
{
    // TODO: This will eventually be replaced by modular character models
    
    public GameObject sword;
    public GameObject knife;
    public GameObject gun;
    //public GameObject bomb;

    public void Equip(string weapon)
    {
        Unequip();
        if( weapon == "sword")
        {
            sword.SetActive(true);
        }
        else if (weapon == "knife")
        {
            knife.SetActive(true);
        }
        else if ( weapon == "gun")
        {
            gun.SetActive(true);
        }
        //else if( weapon == "bomb")
        //{
        //    bomb.SetActive(true);
        //}
    }

    public void Unequip()
    {
        sword.SetActive(false);
        gun.SetActive(false);
        //bomb.SetActive(false);
    }
}
