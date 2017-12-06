using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInDuration = 2;
    private bool gameStarted;

    private Transform playerTransform;
    public Transform arrow;
    public Objective objective;

    private void Start()
    {
        // find the player transform 
        playerTransform = FindObjectOfType<PlayerMotor>().transform;

        // Load up the level 
        SceneManager.LoadScene(Manager.Instance.currentLevel.ToString(), LoadSceneMode.Additive);

        // Get canvasGroup
        fadeGroup = FindObjectOfType<CanvasGroup>();

        //set fade to full 
        fadeGroup.alpha = 1;
    }

    private void Update()
    {
        if (objective != null)
        {
            // if we have an objective rotate arrow 
            Vector3 dir = playerTransform.InverseTransformPoint(objective.GetCurrentRing().position);
            float a = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            a += 180;
            arrow.transform.localEulerAngles = new Vector3(0, 180, a);
        }

        if (Time.timeSinceLevelLoad <= fadeInDuration)
        {
            // initial fade in 
            fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);
        }
        else if (!gameStarted)
        {
            fadeGroup.alpha = 0;
            gameStarted = true;
        }
    }

    public void CompleteLevel()
    {
        // Complete the level and save
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel);

        // Focus the level selection 
        Manager.Instance.menuFocus = 1;

        ExitScene();
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
