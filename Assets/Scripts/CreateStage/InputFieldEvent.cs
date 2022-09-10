using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//InputFieldで改行できないようにさせるクラス
public class InputFieldEvent : MonoBehaviour
{
    [SerializeField] private InputField input;
    //InputFieldに一文字入力される毎に呼び出される
    public void OnValueChanged()
    {
        string text = input.text;
        text=text.Replace("\r", "").Replace("\n", "");
        input.text = text;
    }
}
