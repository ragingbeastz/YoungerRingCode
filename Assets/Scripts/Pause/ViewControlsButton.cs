using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ViewControlsButton : MonoBehaviour
{
    public GameObject viewControls;

    void Start(){
    }
    public void OnClick(){
        viewControls.SetActive(!viewControls.activeSelf);
    }
}
