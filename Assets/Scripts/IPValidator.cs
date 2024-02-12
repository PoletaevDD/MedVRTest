using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "Input Field Validator", menuName = "IP Validator")]
public class IPValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        string[] ipStringArr = text.Split('.');
        pos = text.Length;

        if (ch == '.')
        {
            if (ipStringArr.Length == 4 || text.Length > 0 && text[text.Length - 1] == '.')
                return '\0';
        }
        else
        {
            if (ipStringArr[ipStringArr.Length - 1].Length == 3)
                return '\0';
        }

#if UNITY_EDITOR
        text += ch;
#endif

        return ch;
    }
}
