using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;

    private float baseSpeed = 50.0f;
    private float rotSpeedX = 50.0f;
    private float rotSpeedY = 50.5f;

    private float deathTime;
    private float deathDuration = 2;

    public GameObject deathExplosion;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Create the trail 
        GameObject trail = Instantiate(Manager.Instance.playerTrails[SaveManager.Instance.state.activeTrail]);

        // Set the trail as a children of the model 
        trail.transform.SetParent(transform.GetChild(0));

        // Fix the rotation of the trail
        trail.transform.localEulerAngles = Vector3.forward * -90f;
    }

    private void Update()
    {
        // if the player is dead 
        if (deathTime != 0)
        {
            // wait x seconds then restart 
            if (Time.time - deathTime > deathDuration)
            {
                SceneManager.LoadScene("Game");
            }

            return;
        }

        // give player forward Velocity 
        Vector3 moveVector = transform.forward * baseSpeed;

        // Get players input 
        Vector3 inputs = Manager.Instance.GetPlayerInput();

        // Get the delta direction 
        Vector3 yaw = inputs.x * transform.right * rotSpeedX * Time.deltaTime;
        Vector3 pitch = inputs.y * transform.up * rotSpeedY * Time.deltaTime;
        Vector3 dir = yaw + pitch;

        // limit the player from doing a loop 
        float maxX = Quaternion.LookRotation(moveVector + dir).eulerAngles.x;

        // Not going to far up/down add the direction to the moveVector 
        if (maxX < 90 && maxX > 70 || maxX > 270 && maxX < 290)
        {
            // to far
        }
        else
        {
            // add dir to the current move 
            moveVector += dir;

            // have player face where he is going 
            transform.rotation = Quaternion.LookRotation(moveVector);
        }

        // Move him 
        controller.Move(moveVector * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Set the death timestamp 
        deathTime = Time.time;

        // player explosion effect
        GameObject go = Instantiate(deathExplosion) as GameObject;
        go.transform.position = transform.position;

        // hide player mesh 
        transform.GetChild(0).gameObject.SetActive(false);
    }
}