using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savetest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveData.ResetData();
        //SaveData.Load();
        //SaveData.SetStageData(SaveData.STAGE_NAME_FOR_NORMAL_PUZZLE[1], 100);
        //Debug.Log(SaveData.GetStageDataFromStagename(SaveData.STAGE_NAME_FOR_NORMAL_PUZZLE[1]));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
