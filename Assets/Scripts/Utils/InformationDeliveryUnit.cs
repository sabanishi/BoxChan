using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationDeliveryUnit
{
    private static InformationDeliveryUnit instance;

    public static InformationDeliveryUnit Instance
    {
        get
        {
            if (object.Equals(instance, null))
            {
                instance = new InformationDeliveryUnit();
            }
            return instance;
        }
    }

    private BlockEnum[,] blockEnums;
    public BlockEnum[,] BlockEnums
    {
        get { return blockEnums; }
        set { blockEnums = value; }
    }
}
