using System;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public float msToWait = 5000;
    private Button chestButton;
    private ulong lastChestOpen;

    private void Start()
    {
        chestButton = GetComponent<Button>();
        lastChestOpen = ulong.Parse(PlayerPrefs.GetString("LastChestOpen"));

        if (!IsChestReady())
            chestButton.interactable = false;
    }

    private void Update()
    {
        if(!chestButton.IsInteractable())
        {
            if (IsChestReady())
                chestButton.interactable = true;
        }
    }

    public void ChestClick()
    {
        lastChestOpen = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("LastChestOpen", lastChestOpen.ToString());
        chestButton.interactable = false;
    }

    private bool IsChestReady()
    {
        ulong diff = ((ulong)DateTime.Now.Ticks - lastChestOpen);
        ulong m = diff / TimeSpan.TicksPerMillisecond;
        float secondsLeft = (float)(msToWait - m) / 1000.0f;

        if (secondsLeft < 0)
            return true;

        return false;
    }


}
