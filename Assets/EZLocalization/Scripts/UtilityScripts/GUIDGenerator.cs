using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDGenerator
{
    static char[] characterToIndex = 
        new char[] { '0', '1', '2', '3', '4',
                     '5', '6', '7', '8', '9',
                     'A', 'B', 'C', 'D', 'E', 'F' };

    public static ID GenerateNewGUIDForEntity(int listIndex)
    {
        ID newID = ConvertIntToGUID(listIndex);
        return newID;
    }

    public static int ConvertGUIDToInt(ID code)
    {
        int result = 0;

        if (code.hex.Length != 6)
        {
            Debug.LogError("Code: " + code + " does not match the 6 value structure");
        }

        char[] charArray = code.hex.ToCharArray();
        Array.Reverse(charArray);
        code.hex = new string(charArray);

        for (int i = 0; i < code.hex.Length; i++)
        {
            result += ConvertCharacterToIndex(code.hex[i]) * ((int)Mathf.Pow(characterToIndex.Length, i));
        }
        return result;
    }

    public static ID ConvertIntToGUID(int index)
    {
        string result = "";

        int placesFilled = 0;

        for (int i = 0; i < 6; i++)
        {
            if (index >= ((int)Mathf.Pow(characterToIndex.Length, i)))
            {
                placesFilled = i + 1;
            }
        }
        for (int i = 0; i < 6 - placesFilled; i++)
        {
            result += "0";
        }

        string endCap = "";

        int value = index;
        for (int i = 5; i >= 0; i--)
        {
            int cap = 16;
            int floor = (int)Mathf.Pow(characterToIndex.Length, i);
            if (i > 0)
            {
                cap = (int)Mathf.Pow(characterToIndex.Length, i + 1);
            }
            for (int j = 0; j < characterToIndex.Length; j++)
            {
                int valueIndex = characterToIndex.Length - j;
                int capPercent = (cap / characterToIndex.Length) * valueIndex;
                //Debug.Log("Value " + value + " cap Percent checking " + capPercent);
                if (value >= capPercent)
                {
                    //Debug.Log("Value " + value + " at index " + i + " floor " + floor + " cap " + cap + "\n<color=red>Cap Division: " + capPercent + " This Value " + characterToIndex[valueIndex].ToString() + " index " + j + "</color>");
                    endCap += characterToIndex[valueIndex].ToString();
                    value -= capPercent;
                    break;
                }
     
            }
        }

        result += endCap;

        int zerosToAdd = 6 - result.Length;
        for (int i = 0; i < zerosToAdd; i++)
        {
            result += "0";
        }
        ID resultID = new ID();
        resultID.hex = result;

        return resultID;
    }

    static int ConvertCharacterToIndex(char c)
    {
        for (int j = 0; j < characterToIndex.Length; j++)
        {
            if(characterToIndex[j] == c)
            {
                return j;
            }
        }
        Debug.LogError("That character: " + c.ToString() + " is invalid for a GUID");
        return -1;
    }
}
[System.Serializable]
public struct ID
{
    [SerializeField]
    string Hex;

    public string hex {
        get {

            if (string.IsNullOrEmpty(Hex))
            {
                return null;
            }
            else
            {
                return Hex;
            }
        }
        set
        {
            Hex = value;
        }
    }

    public ID(string h)
    {
        Hex = h;
    }

    public override bool Equals(object other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        ID otherID = (ID)other;
        return otherID.hex == hex;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(ID per1, ID per2)
    {
        return per1.hex.Equals(per2.hex);
    }
    public static bool operator !=(ID per1, ID per2)
    {
        return !per1.hex.Equals(per2.hex);
    }

    public override string ToString()
    {
        return Hex;
    }
}

