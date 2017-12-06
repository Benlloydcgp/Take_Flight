using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float loadTime;
    // Show logo for 3 seconds
    private float minLogoTime = 3.0f;

    private void Start()
    {
        // Get canvas group 
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with white screen
        fadeGroup.alpha = 1;

        // Give it a buffer time 
        if (Time.time < minLogoTime)
        {
            loadTime = minLogoTime;
        }
        else
        {
            loadTime = Time.time;
        }
    }

    private void Update()
    {
        // Fade in
        if(Time.time < minLogoTime)
        {
            fadeGroup.alpha = 1 - Time.time;
        }

        // Fade out 
        if (Time.time > minLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minLogoTime;
            if(fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
