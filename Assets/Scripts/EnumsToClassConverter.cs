using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vafan �r det h�r f�r j�vla l�sning man m�ste g�ra, j�vla skit-unity
//om man vill ha en enum som parameter till en metod som anv�nds i en OnClick() s� l�gger man till det h�r scriptet p�
//knappen, s�tter v�rdet p� den enumen man bryr sig om och i metoden s� skickar man in en EnumsToClassConverter ist�llet
//f�r enumen, s� f�r den r�tt v�rde.

public class EnumsToClassConverter : MonoBehaviour 
{
    public SortingTypes SortingType;
}
