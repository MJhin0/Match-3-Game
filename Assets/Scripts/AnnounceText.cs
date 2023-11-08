using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnounceText : MonoBehaviour
{
    public static AnnounceText aText;
    public static int phase = 0;
    public TextMeshProUGUI announceText;
    public float wait0_1 = 0.1f;
    private Vector3 scaleChange0 = new Vector3(0.05f, 0.0f, 0.00f);
    private Vector3 scaleChange2 = new Vector3(-0.05f, 0.0f, 0.00f);
    bool persist = false;

    // Start is called before the first frame update
    void Start()
    {
        aText = GetComponent<AnnounceText>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void sendText(String text, bool stay){
        aText.activate(text, stay);
    }

    private void activate(String text, bool stay){
        announceText.text = text;
        persist = stay;
        phase = 0;
        enabled = true;
    }

    void FixedUpdate(){
        switch(phase){
            //Expand text
            case 0:
                announceText.transform.localScale += scaleChange0;
                if(announceText.transform.localScale.x >= 1) phase++;
                break;

            //Wait, keep text if persist is on
            case 1:
                wait0_1 -= Time.deltaTime;
                if(wait0_1 < 0) {
                    if(persist) enabled = false;
                    else phase++;
                }
                break;

            //Shrink text if persist is false
            case 2:
                announceText.transform.localScale += scaleChange2;
                if(announceText.transform.localScale.x <= 0){
                    announceText.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
                    enabled = false;
                }
                break;

            default:
                break;
        }
    }
}
