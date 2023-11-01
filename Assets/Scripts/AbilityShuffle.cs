using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShuffle : MonoBehaviour
{
    // Instance of the ability when other scripts need it
    public static AbilityShuffle abilityShuffle;
    public Image sprite;

    // Variables for if the specific ability is selected
    public bool isSelected;
    private static Color selectFade = new Color(1f, 1f, 1f, 0.5f);
    private static Color deselectFade = new Color(1f, 1f, 1f, 1f);

    // Variables for how much the ability is charged
    public int currentBar = 0;
    private int maxBar = 1;
    public bool activateShuffle = false;
    private static Color activated = new Color(1f, 1f, 1f, 1f);
    private static Color deactivated = new Color(0.75f, 0.75f, 0.75f, 1f);


    // Start is called before the first frame update
    void Start()
    {
        // Get the component
        abilityShuffle = GetComponent<AbilityShuffle>();
        sprite = GetComponent<Image>();
        sprite.fillAmount = currentBar / maxBar;
        sprite.color = deactivated;
    }

    // Left-clicking on the ability
    void OnMouseDown() {
        if (!isSelected) {
            if (activateShuffle == true) {
                Select();
            }
        }
        else {
            Deselect();
        }
        // For activating Shuffle Ability
        if (activateShuffle == true) {
            Gameplay.level.Shuffle();
            Deselect();
            DeactivateShuffle();
        }

    }

     // For when it is left-clicked on
    public void Select() {
        isSelected = true;
        sprite.color = selectFade;
    }
    public void Deselect() {
        isSelected = false;
        sprite.color = deselectFade;
    }

    // Depletes bar and deactivates ability
    public void DeactivateShuffle() {
        currentBar = 0;
        activateShuffle = false;
        sprite.color = deactivated;
    }

    // Adds to the gauge by 1, will be used in other scripts
    public void AddToBar() {
        currentBar++;
        if (currentBar >= maxBar) {
            sprite.color = activated;
        }
    }

    // Update is called once per frame
    void Update() {
        if (currentBar >= maxBar) {
            activateShuffle = true;
        }
        sprite.fillAmount = 1.0f * currentBar / maxBar;
    }
}
