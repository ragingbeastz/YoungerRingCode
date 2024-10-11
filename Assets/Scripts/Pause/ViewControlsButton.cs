using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ViewControlsButton : MonoBehaviour
{
    public GameObject viewControls;
    public GameObject pauseMenu;

    void Start(){
    }
    public void OnClick(){
        viewControls.SetActive(!viewControls.activeSelf);
    }

    public void BackToMenu(){
        viewControls.SetActive(false);
        pauseMenu.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }
}
