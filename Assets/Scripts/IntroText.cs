using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroText : MonoBehaviour
{

    public static int phase = 0;
    public TextMeshProUGUI levelText;
    public float wait0_1 = 0.1f;
    private Vector3 scaleChange0 = new Vector3(0.05f, 0.0f, 0.00f);
    private Vector3 scaleChange2 = new Vector3(-0.03f, -0.03f, 0.00f);
    private Vector3 intendedPosition = new Vector3(-6.5f, 4.5f, 0.0f);
    private float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){
        switch(phase){
            //Expand text
            case 0:
                levelText.text = "Earth " + PlayerPrefs.GetInt("SelectedLevel", 1);
                levelText.transform.localScale += scaleChange0;
                if(levelText.transform.localScale.x >= 1) phase++;
                break;

            //Wait
            case 1:
                wait0_1 -= Time.deltaTime;
                if(wait0_1 < 0) phase++;
                break;

            //Shrink text and move to position
            case 2:
                if(levelText.transform.localScale.x > 0.5) levelText.transform.localScale += scaleChange2;
                levelText.transform.position = Vector3.MoveTowards(levelText.transform.position, intendedPosition, moveSpeed * Time.fixedDeltaTime);
                moveSpeed += 1f;
                if(levelText.transform.localScale.x <= 0.5 && Vector3.Distance(levelText.transform.position, intendedPosition) < 0.001f) {
                    levelText.transform.position = intendedPosition;
                    levelText.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
                    phase++;
                    enabled = false;
                }
                break;

            default:
                break;
        }
    }
}
