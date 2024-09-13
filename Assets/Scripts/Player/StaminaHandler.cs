using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class StaminaHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Image staminaBar;
    public float regenRate = 0.1f;
    private float lastMovement = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseOverTime();

        void IncreaseOverTime()
        {
            float thisMovement = Time.time;
            if ((thisMovement - lastMovement) >= regenRate)
            {
                if (staminaBar.fillAmount < 1f)
                {
                    staminaBar.fillAmount += 0.01f;
                }
                lastMovement = thisMovement;
            }
        }

    }

}
