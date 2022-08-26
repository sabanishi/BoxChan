using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static string ConvertTimeFormat(float time)
    {
        return ((int)time / 60).ToString("D2") + ":" + ((int)time % 60).ToString("D2") + ":" + ((int)((time % 1)*100)).ToString("D2");
    }
}
