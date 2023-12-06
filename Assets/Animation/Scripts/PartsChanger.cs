using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PartsChanger : MonoBehaviour
{

    [SerializeField] private int amountOfVersions;
    [SerializeField] private GameObject ponytail;
    private SpriteResolver[] resolvers;

    private int[] bodyPartCollection;

    //Skinparts ------- Torso --------- Ögon --------- Mun ------- Ben -------- Fötter ---------- Hår
    private int[][] bodyParts = { new int[] {14, 21, 12}, new int[] {15, 22, 13 }, new int[] {3, 4, 5, 6, 7, 8, 9, 10 },
        new int[] {0, 1, 2 }, new int[] {18, 19, 16 }, new int[] {17, 20 }, new int[] {11} };

    // Start is called before the first frame update
    void Start()
    {
        resolvers = GetComponentsInChildren<SpriteResolver>();
        bodyPartCollection = RandomizeCharacter();
        ponytail.GetComponent<LineRenderer>().enabled = false;

        //Jag fattar verkligen inte varför, men om man inte väntar lite efter start med SetUpCharacter så är det några munnar och ögon som inte byter sprite.
        //Det borde verkligen funka, men av någon vill den inte gå med på det. 
        StartCoroutine(ChillOut());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bodyPartCollection = RandomizeCharacter();
            SetUpCharacter();
        }
    }

    private void ChangePants(int version)
    {
        ChangeVersionOnCollection(4, version);
        bodyPartCollection[4] = version;
    }

    private void ChangeShirt(int version)
    {
        ChangeVersionOnCollection(1, version);
        bodyPartCollection[1] = version;
    }

    private void ChangeShoes(int version)
    {
        ChangeVersionOnCollection(5, version);
        bodyPartCollection[5] = version;
    }

    private void ChangeAPart(int bodyPart, int version)
    {
        resolvers[bodyPart].SetCategoryAndLabel(resolvers[bodyPart].GetCategory(), "Entry_" + version);
    }

    private void SetUpCharacter()
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
    }

    private void ChangeVersionOnCollection(int collectionNumber, int version)
    {
        for (int i = 0; i < bodyParts[collectionNumber].Length; i++)
        {
            ChangeAPart(bodyParts[collectionNumber][i], version);
        }
    }

    private int[] RandomizeCharacter()
    {
        int[] rndArr = new int[7];
        for(int i = 0;  i < 7; i++)
        {
            rndArr[i] = Random.Range(0, amountOfVersions);
        }
        return rndArr;
    }

    private IEnumerator ChillOut()
    {
        yield return new WaitForSeconds(0.1f);
        SetUpCharacter();
    }
}
