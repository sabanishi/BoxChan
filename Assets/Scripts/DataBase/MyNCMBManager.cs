using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class MyNCMBManager : MonoBehaviour
{
    public static MyNCMBManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void PushPlayerData()
    {

    }

    public static void FetchPlayerData()
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("PlayerData");
    }
}
