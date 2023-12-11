using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PartsChanger : MonoBehaviour
{

    [SerializeField] private int amountOfVersions;
    [SerializeField] private GameObject ponytail;
    private SpriteResolver[] resolvers;

    private int[] bodyPartCollection;
    private Renderer[] renderers;

    //Skinparts ------- Torso --------- Ögon --------- Mun ------- Ben -------- Fötter ---------- Hår
    private int[][] bodyParts = { new int[] {14, 21, 12}, new int[] {15, 22, 13 }, new int[] {3, 4, 5, 6, 7, 8, 9, 10 },
        new int[] {0, 1, 2 }, new int[] {18, 19, 16 }, new int[] {17, 20 }, new int[] {11} };

    // Start is called before the first frame update
    void Awake()
    {
        resolvers = GetComponentsInChildren<SpriteResolver>();
        ponytail.GetComponent<LineRenderer>().enabled = false;
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bodyPartCollection = RandomizeArr();
            SetUpCharacter();
        }
    }

    public void ChangePants(int version)
    {
        ChangeVersionOnCollection(4, version);
        bodyPartCollection[4] = version;
    }

    public void ChangeShirt(int version)
    {
        ChangeVersionOnCollection(1, version);
        bodyPartCollection[1] = version;
    }

    public void ChangeShoes(int version)
    {
        ChangeVersionOnCollection(5, version);
        bodyPartCollection[5] = version;
    }

    
    public void RandomizeCharacter()
    {
        bodyPartCollection = RandomizeArr();
        SetUpCharacter();
    }

    public void SetUpCharacter()
    {
        for(int i = 0; i < bodyParts.Length; i++)
        {
            ChangeVersionOnCollection(i, bodyPartCollection[i]);
        }
        if (bodyPartCollection[6] == 0)
        {
            ponytail.GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            ponytail.GetComponent<LineRenderer>().enabled = false;
        }
        ChangeSortingLayer();
    }

    private void ChangeVersionOnCollection(int collectionNumber, int version)
    {
        for (int i = 0; i < bodyParts[collectionNumber].Length; i++)
        {
            ChangeAPart(bodyParts[collectionNumber][i], version);
        }
    }

    private int[] RandomizeArr()
    {
        int[] rndArr = new int[7];
        for(int i = 0;  i < 7; i++)
        {
            rndArr[i] = Random.Range(0, amountOfVersions);
        }
        return rndArr;
    }

    private void ChangeAPart(int bodyPart, int version)
    {
        resolvers[bodyPart].SetCategoryAndLabel(resolvers[bodyPart].GetCategory(), "Entry_" + version);
    }

    private void ChangeSortingLayer()
    {

        for(int i = 0; i < GetSortingLayerNames().Length; i++)
        {
            bool nameAvailable = true;
            string currentName = "";
            for(int a = 0; a < UnitController.GetCharacters().Count; a++)
            {
                currentName = UnitController.GetCharacters()[a].transform.GetChild(0).GetComponent<Renderer>().sortingLayerName;
             
                if(currentName == GetSortingLayerNames()[i] || GetSortingLayerNames()[i] == "Default")
                {                    
                    nameAvailable = false;
                }
                
            }
            if (nameAvailable)
            {
                ChangeSortingLayerOnAllParts(GetSortingLayerNames()[i]);
                return;
            }
        }
    }

    private void ChangeSortingLayerOnAllParts(string sortingLayerName)
    {
        GetComponent<Renderer>().sortingLayerName = sortingLayerName;
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sortingLayerName = sortingLayerName;
            Debug.Log("Current layer: " + renderers[i].sortingLayerName + " New layer: " + sortingLayerName);
        }
    }

    private string[] GetSortingLayerNames()
    {
        System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }
}
