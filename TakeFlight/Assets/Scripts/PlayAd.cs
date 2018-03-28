using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class PlayAd : MonoBehaviour
{
    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show ("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Player watch the ad");
                break;
            case ShowResult.Skipped:
                Debug.Log("Player did not fully watch the ad");
                break;
            case ShowResult.Failed:
                Debug.Log("Player failed to launch the ad ?Internet?");
                break;
        }
    }
}
