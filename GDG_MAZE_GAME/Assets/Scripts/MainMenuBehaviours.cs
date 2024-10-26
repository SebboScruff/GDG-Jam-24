using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuBehaviours : MonoBehaviour
{
    public Button startButton, aboutButton, quitButton, aboutBackButton;
    public GameObject aboutParentObj, mainParentObj;

    // this formatting absolutely sucks but who cares
    // it's just a basic menu
    public void Start()
    {
        startButton.onClick.AddListener(delegate { SceneManager.LoadScene(1); });

        aboutButton.onClick.AddListener(delegate { aboutParentObj.SetActive(true); mainParentObj.SetActive(false); });

        quitButton.onClick.AddListener(delegate { Application.Quit(); });

        aboutBackButton.onClick.AddListener(delegate { aboutParentObj.SetActive(false); mainParentObj.SetActive(true); });
    }
}
