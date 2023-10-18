using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=gHdXkGsqnlw

public class AbilityExplodeBar : MonoBehaviour
{
    // Instance of the ability when other scripts need it
    public static AbilityExplodeBar abilityExplodeBar;

    // Gauge for how much an ability is charged
    public int currentBar = 0;
    private int maxBar = 50;
    public bool activateExplode = false;

    // Depletes bar and deactivates ability, will be used in other scripts
    public void DeactivateExplode() {
        currentBar = 0;
        activateExplode = false;
    }

    // Adds to the gauge by 1, will be used in other scripts
    public void AddToBar() {
        currentBar++;
        Debug.Log(currentBar);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the component
        abilityExplodeBar = GetComponent<AbilityExplodeBar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBar >= maxBar) {
            activateExplode = true;
        }
    }
}
