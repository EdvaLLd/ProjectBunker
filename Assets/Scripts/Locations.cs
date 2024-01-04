using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locations// : MonoBehaviour
{
    [System.Serializable]
    public class Location/* : MonoBehaviour*/
    {
        [SerializeField]
        private float maxDistance = 1500;
        [SerializeField]
        private float minDistance = 500;
        public enum environments { Home, Lake, City, Factory, Forest };
        public environments environment;

        public List<Looting.LootItem> locationLoot;

        public float distanceToHome;

        public string locationName = "Unknown Location";

        public float RandomDistance()
        {
            return Random.Range(minDistance, maxDistance);
        }
    }

    public static Location[] SetExplorableLocations()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();

        Location[] replaceArray = new Location[System.Enum.GetNames(typeof(Location.environments)).Length];
        if (System.Enum.GetNames(typeof(Location.environments)).Length > 1)
        {
            /*Location[] */
            replaceArray = new Location[System.Enum.GetNames(typeof(Location.environments)).Length - 1]; // -1 top reduce length by the first element (home).


            Location.environments[] locationArray = (Location.environments[])System.Enum.GetValues(typeof(Location.environments)); /// Refer to this when you want to iterate over enums through for loop.

            for (int arrayIndex = 0; arrayIndex < replaceArray.Length; arrayIndex++) // arrayIndex loops over the the Location class objects in the Location[] replaceArray.
            {
                replaceArray[arrayIndex] = new Location();
                replaceArray[arrayIndex].environment = locationArray[arrayIndex + 1]; // +1 to not loop over first element (home) and begin with the second one instead.

                // Sets up variables within the class.
                replaceArray[arrayIndex].distanceToHome = replaceArray[arrayIndex].RandomDistance();
                replaceArray[arrayIndex].locationName += " No_" + arrayIndex;
                replaceArray[arrayIndex].locationLoot = gameManager.locationalLoot[arrayIndex].lootItems;

            }
        }
        return replaceArray; // Outputs an array without the first element of the enum with the approriate length of quantity of enums - 1.
    }
}
