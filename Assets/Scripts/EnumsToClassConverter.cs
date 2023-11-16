using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//vafan är det här för jävla lösning man måste göra, jävla skit-unity
//om man vill ha en enum som parameter till en metod som används i en OnClick() så lägger man till det här scriptet på
//knappen, sätter värdet på den enumen man bryr sig om och i metoden så skickar man in en EnumsToClassConverter istället
//för enumen, så får den rätt värde.

public class EnumsToClassConverter : MonoBehaviour 
{
    public SortingTypes SortingType;
}
