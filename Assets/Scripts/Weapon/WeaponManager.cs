using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }
    public List<GameObject> weaponList = new List<GameObject>(); //武器列表
    int current = 0;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject Current { get { return weaponList[current];} }

    public GameObject ChangeNext()
    {
        current++;
        if (current >= weaponList.Count)
        {
            current = 0;
        }
        return weaponList[current];
    }
}
