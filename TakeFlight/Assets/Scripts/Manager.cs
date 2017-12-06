using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { set; get; }
    public Material playerMaterial;
    public Color[] playerColors = new Color[10];
    public GameObject[] playerTrails = new GameObject[10];

    // used when changing from menu to game scene
    public int currentLevel = 0;
    // use when entering the menu scene 
    public int menuFocus = 0;

    // Touch ID, Starting position 
    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public Vector3 GetPlayerInput()
    {
        // using accelerometer
        if(SaveManager.Instance.state.usingAccelerometer)
        {
            // if true replace Y parameter by Z
            Vector3 a = Input.acceleration;
            a.y = a.z;
            return a;
        }

        // read all touches 
        Vector3 r = Vector3.zero;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // start pressing the screen 
                activeTouches.Add(touch.fingerId, touch.position);
            }
            // if finger removed 
            else if (touch.phase == TouchPhase.Ended)
            {
                if (activeTouches.ContainsKey(touch.fingerId))
                {
                    activeTouches.Remove(touch.fingerId);
                }
            }
            // use delta from orig pos if finger is moving or still 
            else
            {
                float mag = 0;
                r = (touch.position - activeTouches[touch.fingerId]);
                mag = r.magnitude / 450;
                // Maybe needs to be this
                // mag = r.magnitude / 10;
                r = r.normalized * mag;
            }
        }

        return r;
    }
}
