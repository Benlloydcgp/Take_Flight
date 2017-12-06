using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; }
    public SaveState state;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

         // using the acceleromter?
         if(state.usingAccelerometer && !SystemInfo.supportsAccelerometer)
        {
            // if false make sure not trying again 
            state.usingAccelerometer = false;
            Save();
        }
    }

    // Save state of script to player pref 
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state));
    }

    // Load the pervious save state from player pref 
    public void Load()
    {
        if (PlayerPrefs.HasKey("save"))
        {
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save"));
        }
        else
        {
            state = new SaveState();
            Save();
            Debug.Log("No save file found, creating a new one!");
        }
    }

    public bool IsColorOwned(int index)
    {
        // Checks if bit is set 
        return (state.colorOwned & (1 << index)) != 0;
    }

    public bool IsTrailOwned(int index)
    {
        // Checks if bit is set 
        return (state.trailOwned & (1 << index)) != 0;
    }

    // Attempt buying a colour, return true/false
    public bool BuyColor(int index, int cost)
    {
        if(state.gold >= cost)
        {
            // Enough money, remove from current gold stack
            state.gold -= cost;
            UnlockColor(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough gold 
            return false;
        }
    }

    // Attempt buying a trail, return true/false
    public bool BuyTrail(int index, int cost)
    {
        if (state.gold >= cost)
        {
            // Enough money, remove from current gold stack
            state.gold -= cost;
            UnlockTrail(index);

            // Save progress
            Save();

            return true;
        }
        else
        {
            // Not enough gold 
            return false;
        }
    }

    // Unlock the color 
    public void UnlockColor(int index)
    {
        // toggle on bit index
        state.colorOwned |= 1 << index;
    }

    // Unlock the trail 
    public void UnlockTrail(int index)
    {
        // toggle on bit index
        state.trailOwned |= 1 << index;
    }

    // Complete level 
    public void CompleteLevel(int index)
    {
        // current active level 
        if (state.completedLevel == index)
        {
            state.completedLevel++;
            Save();
        }
    }

    // Reset the save file 
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}

/*
DONT WANT TO SERIALIZE THE DIFFERENT 
CLASSES THAT GO INSIDE THE SAVE MANAGER 
AWAKE, SAVE, LOAD FUNCTIONS THAT WE CANNOT 
SERIALIZE.
*/
