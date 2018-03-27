using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInSpeed = 0.33f;

    public RectTransform menuContainer;
    public Transform levelPanel;
    public Transform settingsPanel;
    public Transform creditsPanel;

    public Transform colorPanel;
    public Transform trailPanel;

    public Button tiltControlButton;
    public Color tiltControlEnabled;
    public Color tiltControlDisabled;

    public Text colorBuySetText;
    public Text trailBuySetText;
    public Text goldText;

    private MenuCamera menuCam;

    private int[] colorCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 10 };
    private int[] trailCost = new int[] { 0, 5, 5, 5, 10, 10, 10, 15, 15, 10 };
    private int selectedColorIndex;
    private int selectedTrailIndex;
    private int activeColorIndex;
    private int activeTrailIndex;

    private Vector3 desiredMenuPosition;

    private GameObject currentTrail;

    public AnimationCurve enteringLevelZoomCurve;
    private bool isEnteringLevel = false;
    private float zoomDuration = 3.0f;
    private float zoomTransition;

    private Texture previousTrail;
    private GameObject lastPreviewObject;

    public Transform trailPreviewObject;
    public RenderTexture trailPreviewTexture;

    private void Start()
    {
        // check if we have a accelerometer 
        if (SystemInfo.supportsAccelerometer)
        {
            // is currentl enables?
            tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;
        }
        else
        {
            tiltControlButton.gameObject.SetActive(false);
        }

        // Find the menu camera 
        menuCam = FindObjectOfType<MenuCamera>();

        // position our camera 
        SetCameraTo(Manager.Instance.menuFocus);

        // how much gold 
        UpdateGoldText();
        // Get canvas group 
        fadeGroup = FindObjectOfType<CanvasGroup>();

        // Start with white screen
        fadeGroup.alpha = 1;

        // on click events to shop buttons
        Shop();

        // on click events to settings button
        Settings();

        Credits();

        // on click event to level 
        Level();

        // Set players pref
        OnColorSelect(SaveManager.Instance.state.activeColor);
        SetColor(SaveManager.Instance.state.activeColor);

        OnTrailSelect(SaveManager.Instance.state.activeTrail);
        SetTrail(SaveManager.Instance.state.activeTrail);

        // Make buttons bigger for selected item 
        colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;

        // Create the trail preview 
        lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrails[SaveManager.Instance.state.activeTrail]) as GameObject;
        lastPreviewObject.transform.SetParent(trailPreviewObject);
        lastPreviewObject.transform.localPosition = Vector3.zero;

        //AdManager.Instance.ShowBanner();
    }

    private void Update()
    {
        // fade in 
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // Menu Nav 
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, 0.1f);

        // entering level zoom 
        if (isEnteringLevel)
        {
            // add zoom transition float 
            zoomTransition += (1 / zoomDuration) * Time.deltaTime;

            // change the scale 
            menuContainer.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // change the size of canvas 
            Vector3 newDesiredPosition = desiredMenuPosition * 5;

            // adds to the specific position of level 
            RectTransform rt = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rt.anchoredPosition3D * 5;

            // Override previous update
            menuContainer.anchoredPosition3D = Vector3.Lerp(desiredMenuPosition, newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            // fade to white screen 
            fadeGroup.alpha = zoomTransition;

            // animation complete
            if (zoomTransition >= 1)
            {
                // enter the level 
                SceneManager.LoadScene("Game");
            }
        }
    }

    private void Shop()
    {
        // Assign references 
        if (colorPanel == null || trailPanel == null)
        {
            Debug.Log("You did not assign the color/trailPanel panel in the inspector");
        }

        // Add on-click even on the button 
        int i = 0;
        foreach (Transform t in colorPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColorSelect(currentIndex));

            // set color of the image if owned or not 
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsColorOwned(i) ? Manager.Instance.playerColors[currentIndex] : Color.Lerp(Manager.Instance.playerColors[currentIndex], new Color(0, 0, 0, 1), 0.25f);

            i++;
        }

        // Reset the index and do the same for trails 
        foreach (Transform t in trailPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnTrailSelect(currentIndex));

            RawImage img = t.GetComponent<RawImage>();
            img.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.8f, 0.7f, 0.7f);

            i++;
        }

        // Set previous trail to prevent bugs 
        previousTrail = trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RawImage>().texture;

    }

    private void Settings()
    {
        if (settingsPanel == null)
        {
            Debug.Log("You did not assign the settings panel in the inspector");
        }

        int i = 0; 
        foreach (Transform t in settingsPanel)
        {
            int currentIndex = i;

            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnSettingsSelect(currentIndex));
        }
    }

    private void Credits()
    {
        if (creditsPanel == null)
        {
            Debug.Log("You did not assign the credits panel in the inspector");
        }

        int i = 0;
        foreach (Transform t in creditsPanel)
        {
            int currentIndex = i;

            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnCreditsSelect(currentIndex));
        }
    }

    private void Level()
    {
        // Assign references 
        if (levelPanel == null)
        {
            Debug.Log("You did not assign the level panel in the inspector");
        }

        // Add on-click even on the button 
        int i = 0;
        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelSelect(currentIndex));

            Image img = t.GetComponent<Image>();

            // is it unlocked
            if (i <= SaveManager.Instance.state.completedLevel)
            {
                if (i == SaveManager.Instance.state.completedLevel)
                {
                    // Not completed
                    img.color = Color.white;
                }
                else
                {
                    // Completed
                    img.color = Color.green;
                }
            }
            else
            {
                // level isnt unlocked
                b.interactable = false;
                img.color = Color.grey;
            }

            i++;
        }
    }

    private void SetCameraTo(int menuIndex)
    {
        NavigateTo(menuIndex);
        menuContainer.anchoredPosition3D = desiredMenuPosition;
    }

    private void NavigateTo(int menuIndex)
    {
        switch (menuIndex)
        {
            default:
            // Main Menu
            case 0:
                desiredMenuPosition = Vector3.zero;
                menuCam.BackToMainMenu();
                break;
            // Play Menu 
            case 1:
                desiredMenuPosition = Vector3.right * 1280;
                menuCam.MoveToLevel();
                break;
            // Shop Menu
            case 2:
                desiredMenuPosition = Vector3.left * 1280;
                menuCam.MoveToShop();
                break;
            // Settings Menu
            case 3:
                desiredMenuPosition = Vector3.left * 2560;
                menuCam.MoveToSettings();
                break;
            case 4:
                desiredMenuPosition = Vector3.right * 2560;
                menuCam.MoveToCredits();
                break;

        }
    }

    private void SetColor(int index)
    {
        // set the active index
        activeColorIndex = index;
        SaveManager.Instance.state.activeColor = index;
        // change color of player 
        Manager.Instance.playerMaterial.color = Manager.Instance.playerColors[index];

        // change buy/set button text
        colorBuySetText.text = "Current";

        // saving prefs
        SaveManager.Instance.Save();
    }

    private void SetTrail(int index)
    {
        activeTrailIndex = index;
        SaveManager.Instance.state.activeTrail = index;

        // change color of player 
        if (currentTrail != null)
        {
            Destroy(currentTrail);
        }

        // create new trail 
        currentTrail = Instantiate(Manager.Instance.playerTrails[index]) as GameObject;

        // Set it as a child of player 
        currentTrail.transform.SetParent(FindObjectOfType<MenuPlayer>().transform);

        // Fix scale issues 
        currentTrail.transform.localPosition = Vector3.zero;
        currentTrail.transform.localRotation = Quaternion.Euler(0, 0, 90);
        currentTrail.transform.localScale = Vector3.one * 0.01f;

        // change buy/set button text
        trailBuySetText.text = "Current";

        // Saving prefs
        SaveManager.Instance.Save();
    }

    private void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.state.gold.ToString();
    }

    public void OnPlayClick()
    {
        NavigateTo(1);
        // Debug.Log("Play Button Clicked");
    }

    public void OnShopClick()
    {
        NavigateTo(2);
        // Debug.Log("Shop Button Clicked");
    }

    public void OnSettingsClick()
    {
        NavigateTo(3);
    }

    public void OnCreditsClick()
    {
        NavigateTo(4);
    }

    public void OnBackClick()
    {
        NavigateTo(0);
        Debug.Log("Back Button Clicked");
    }

    private void OnColorSelect(int currentIndex)
    {
        Debug.Log("Selecting color button : " + currentIndex);

        // if button clicked do nothing
        if (selectedColorIndex == currentIndex)
        {
            return;
        }

        // make icon bigger
        colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // Put the previous one on normal scale 
        colorPanel.GetChild(selectedColorIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected colour 
        selectedColorIndex = currentIndex;

        // change content of buy/set button 
        if (SaveManager.Instance.IsColorOwned(currentIndex))
        {
            // color is owned
            // already current color?
            if (activeColorIndex == currentIndex)
            {
                colorBuySetText.text = "Current";
            }
            else
            {
                colorBuySetText.text = "Select";
            }
        }
        else
        {
            // color isnt owned
            colorBuySetText.text = "Buy: " + colorCost[currentIndex].ToString();
        }
    }

    private void OnTrailSelect(int currentIndex)
    {
        Debug.Log("Selecting trail button : " + currentIndex);

        if (selectedTrailIndex == currentIndex)
        {
            return;
        }

        // Preview Trail Get the image of the preview button
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().texture = previousTrail;
        // keep preview image as backup 
        previousTrail = trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture;
        // Set the new trail preview 
        trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture = trailPreviewTexture;

        // Change the object of the trial preview 
        if (lastPreviewObject != null)
        {
            Destroy(lastPreviewObject);
        }
        lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrails[currentIndex]) as GameObject;
        lastPreviewObject.transform.SetParent(trailPreviewObject);
        lastPreviewObject.transform.localPosition = Vector3.zero;

        // make icon bigger
        trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // Put the previous one on normal scale 
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;

        // Set the selected trail 
        selectedTrailIndex = currentIndex;

        // change content of buy/set button 
        if (SaveManager.Instance.IsTrailOwned(currentIndex))
        {
            // trail is owned
            // already current color?
            if (activeTrailIndex == currentIndex)
            {
                trailBuySetText.text = "Current";
            }
            else
            {
                trailBuySetText.text = "Select";
            }
        }
        else
        {
            // trail isnt owned
            trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString();

        }
    }

    private void OnLevelSelect(int currentIndex)
    {
        Manager.Instance.currentLevel = currentIndex;
        //SceneManager.LoadScene("Game");
        isEnteringLevel = true;
        Debug.Log("Selecting level : " + currentIndex);
    }

    private void OnSettingsSelect(int currentIndex)
    {

    }

    private void OnCreditsSelect(int currentIndex)
    {
       
    }

    public void OnColorBuySet()
    {
        Debug.Log("Buy/Set color");

        // is selected color owned 
        if (SaveManager.Instance.IsColorOwned(selectedColorIndex))
        {
            // set the color 
            SetColor(selectedColorIndex); 
        }
        else
        {
            // attempt to buy color 
            if (SaveManager.Instance.BuyColor(selectedColorIndex, colorCost[selectedColorIndex]))
            {
                // Success
                SetColor(selectedColorIndex);

                // Change the color 
                colorPanel.GetChild(selectedColorIndex).GetComponent<Image>().color = Manager.Instance.playerColors[selectedColorIndex];

                // Update gold text 
                UpdateGoldText();
            }
            else
            {
                // Not enough gold 
                Debug.Log("Not enough gold");
            }
        }
    }

    public void OnTrailBuySet()
    {
        Debug.Log("Buy/Set trail");

        // is selected trail owned 
        if (SaveManager.Instance.IsTrailOwned(selectedTrailIndex))
        {
            // set the trail 
            SetTrail(selectedTrailIndex);
        }
        else
        {
            // attempt to buy color 
            if (SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost[selectedTrailIndex]))
            {
                // Success
                SetTrail(selectedTrailIndex);

                // Change the trail 
                trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().color = Color.white;

                // Update gold text 
                UpdateGoldText();
            }
            else
            {
                // Not enough gold 
                Debug.Log("Not enough gold");
            }
        }

    }

    public void OnTiltControl()
    {
        // Toggle accelerometer bool 
        SaveManager.Instance.state.usingAccelerometer = !SaveManager.Instance.state.usingAccelerometer;

        // Make sure save player prefs
        SaveManager.Instance.Save();

        // Change display 
        tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;

    }
}



