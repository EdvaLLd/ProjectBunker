using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPEXPLORING : MonoBehaviour
{
    [SerializeField]
    RecipeSlot[] lootPossibilities;

    bool canExplore = true;
    float exploreCD = 5;

    public void MissionRewards()
    {
        if (canExplore)
        {
            Character selectedChar = UnitController.GetSelectedCharacter();
            if (selectedChar == null)
            {
                TextLog.AddLog("You must have a character selected to go exploring", MessageTypes.used);
                return;
            }
            selectedChar.UseHunger(20);
            if(Random.Range(0,10) < 4) //40%?
            {
                selectedChar.TakeDamage(30);
            }
            if(UnitController.GetSelectedCharacter() == null) //ifall snubben dog
            {
                return;
            }
            int rewardNumber = Random.Range(0, lootPossibilities.Length);
            Inventory.AddItem(lootPossibilities[rewardNumber].item, lootPossibilities[rewardNumber].amount);
            canExplore = false;
            StartCoroutine(MissionTimer());
        }
        else TextLog.AddLog("You need to wait " + exploreCD.ToString() + " seconds between each exploration");
    }
    IEnumerator MissionTimer()
    {
        yield return new WaitForSeconds(exploreCD);
        canExplore = true;
    }
}
