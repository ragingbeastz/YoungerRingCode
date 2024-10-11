using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseButton : MonoBehaviour
{
  public GameObject pauseMenu;
  public GameObject controlsMenu;

    public void OnClick(){
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (controlsMenu.activeSelf)
        {
            controlsMenu.SetActive(false);
        }
    }
}
