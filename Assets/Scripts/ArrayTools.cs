using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class ArrayTools<T>
{
    public static T[] AddElement(T[] inputArray, T element) 
    {
        int updatedLength = inputArray.Length+1;
        int maxIndex = inputArray.Length;
        T[] updatedArray = new T[updatedLength];

        for (int index = 0; index < updatedLength; index++) 
        {
            if (index < maxIndex)
            {
                updatedArray[index] = inputArray[index];
            }
            else 
            {
                break;
            }
        }
        updatedArray[maxIndex] = element;

        return updatedArray;
    }

    public static T[] RemoveLastElementAndShorten(T[] inputArray)
    {
        int updatedLength = Mathf.Clamp(inputArray.Length-1, 0, inputArray.Length-1);
        int maxIndex = Mathf.Clamp(updatedLength - 1, 0, updatedLength - 1);
        T[] updatedArray = new T[updatedLength];

        for (int index = 0; index < updatedLength; index++)
        {
            if (index < maxIndex)
            {
                updatedArray[index] = inputArray[index];
            }
            else
            {
                break;
            }
        }

        return updatedArray;
    }

    public static T[] RemoveElementAtIndexAndShorten(T[] inputArray, int removeIndex)
    {
        int updatedLength = Mathf.Clamp(inputArray.Length - 1, 0, inputArray.Length - 1);
        int maxIndex = Mathf.Clamp(updatedLength - 1, 0, updatedLength - 1);
        T[] updatedArray = new T[updatedLength];

        for (int index = 0; index < updatedLength; index++)
        {
            if (index < maxIndex)
            {
                if (index < removeIndex)
                {
                    updatedArray[index] = inputArray[index];
                }
                else 
                {
                    updatedArray[index] = inputArray[index+1];
                }
            }
            else
            {
                break;
            }
        }

        return updatedArray;
    }

    public static T[] AddElementAtIndex(T[] inputArray, T element, int addIndex)
    {
        int updatedLength = inputArray.Length + 1;
        int maxIndex = inputArray.Length;
        T[] updatedArray = new T[updatedLength];

        for (int index = 0; index < updatedLength; index++)
        {
            if (index < maxIndex)
            {
                if (index < addIndex)
                {
                    updatedArray[index] = inputArray[index];
                }
                if (index == addIndex)
                {
                    updatedArray[index] = element;
                }
                if(index > addIndex)
                {
                    updatedArray[index] = inputArray[index + 1];
                }
            }
            else
            {
                break;
            }
        }

        return updatedArray;
    }

    /*public static int GetElementIndex(T[] inputArray, T element) // Not very universal but works for our situation
    {
        int outputIndex = 0;
        int maxIndex = Mathf.Clamp(inputArray.Length-1, 0, inputArray.Length-1);

        for (int index = 0; index < inputArray.Length; index++)
        {
            if (index < maxIndex)
            {
                if (inputArray[index] == element) 
                {

                }
            }
            else
            {
                break;
            }
        }

        return outputIndex;
    }*/
}
