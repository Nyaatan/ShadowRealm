using UnityEngine;
using System;
namespace TMPro
{

    [Serializable]
    [CreateAssetMenu(fileName = "InputValidator - Digits.asset", menuName = "TextMeshPro/Input Validators/Digits", order = 100)]
    public class IPTextValidator : TMP_InputValidator
    {
        // Custom text input validation function
        public override char Validate(ref string text, ref int pos, char ch)
        {
            if ((ch >= '0' && ch <= '9') || ch == '.')
            {
                text += ch;
                pos += 1;
                return ch;
            }
            return (char)0;
        }
    }
}