using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayAlartManager : MonoBehaviour
{
    [SerializeField] private Text alartText;

    private string alartString;

    public void SetText()
    {
        alartText.text = alartString;
    }

    //もしステージに不備があればtruemを返し、alartStringにその警告文を代入する
    public bool CheckStage(BlockEnum[,] stage)
    {
        alartString = "";
        bool isAlart = false;
        bool isWarpAlone=false;
        bool isWarpThree=false;
        bool isWarpTop = false;

        int playerNum=0;
        int goalNum = 0;
        int deliverBoxNum = 0;
        int[] warpBoxNum = new int[4];
        bool isOkBox=true;
        bool isOkGoal=true;

        for (int x = 0; x < stage.GetLength(0); x++)
        {
            for(int y = 0; y < stage.GetLength(1); y++)
            {
                if (stage[x, y].Equals(BlockEnum.Start))
                {
                    playerNum++;
                }
                if (stage[x, y].Equals(BlockEnum.Goal))
                {
                    goalNum++;
                    if (y == 0 || !stage[x, y - 1].Equals(BlockEnum.NotBox))
                    {
                        isOkGoal = false;
                    }
                }
                if (stage[x, y].Equals(BlockEnum.DeliverBox))
                {
                    deliverBoxNum++;
                }
                if (stage[x, y].Equals(BlockEnum.WarpBox1))
                {
                    warpBoxNum[0]++;
                    if (y == 17)
                    {
                        isWarpTop = true;
                    }
                }
                if (stage[x, y].Equals(BlockEnum.WarpBox2))
                {
                    warpBoxNum[1]++;
                    if (y == 17)
                    {
                        isWarpTop = true;
                    }
                }
                if (stage[x, y].Equals(BlockEnum.WarpBox3))
                {
                    warpBoxNum[2]++;
                    if (y == 17)
                    {
                        isWarpTop = true;
                    }
                }
                if (stage[x, y].Equals(BlockEnum.WarpBox4))
                {
                    warpBoxNum[3]++;
                    if (y == 17)
                    {
                        isWarpTop = true;
                    }
                }
                if (isBox(stage[x, y]))
                {
                    if (y == 0 || (!isBox(stage[x, y - 1]) && !stage[x, y - 1].Equals(BlockEnum.NotBox)))
                    {
                        isOkBox = false;
                    }
                }
            }
        }

        if (playerNum == 0)
        {
            isAlart = true;
            alartString += "・ハコぶちゃんが居ません。\n";
        }
        if (playerNum > 1)
        {
            isAlart = true;
            alartString += "・ハコぶちゃんは複数配置できません。\n";
        }

        if (goalNum == 0)
        {
            isAlart = true;
            alartString += "・ゴールがありません。\n";
        }
        if (goalNum > 1)
        {
            isAlart = true;
            alartString += "・ゴールは複数配置できません。\n";
        }
        if (deliverBoxNum == 0)
        {
            isAlart = true;
            alartString += "・配達対象の段ボールが1つもありません。\n";
        }
        for(int i = 0; i < warpBoxNum.GetLength(0); i++)
        {
            if (!isWarpAlone&&warpBoxNum[i]==1)
            {
                isAlart = true;
                isWarpAlone = true;
                alartString += "・ワープバコは同じ色を2つ組にして配置しないといけません。\n";
            }
            if (!isWarpThree && warpBoxNum[i] >2)
            {
                isAlart = true;
                isWarpThree = true;
                alartString += "・ワープバコは同じ色を3つ以上配置できません。\n";
            }
        }
        if (isWarpTop)
        {
            isAlart = true;
            alartString += "・ワープバコは最上段に配置できません。\n";
        }
        if (!isOkBox)
        {
            isAlart = true;
            alartString += "・ハコの下には必ずハコまたは地面が必要です。\n";
        }
        if (!isOkGoal)
        {
            isAlart = true;
            alartString += "・ゴールの下には必ず地面が必要です。\n";
        }

        return isAlart;
    }

    //ハコの一種であればtrueを返す
    private bool isBox(BlockEnum blockEnum)
    {
        return !(blockEnum.Equals(BlockEnum.Goal) || blockEnum.Equals(BlockEnum.None)
            || blockEnum.Equals(BlockEnum.Start)||blockEnum.Equals(BlockEnum.NotBox)
            ||blockEnum.Equals(BlockEnum.Eraser));
    }
}
