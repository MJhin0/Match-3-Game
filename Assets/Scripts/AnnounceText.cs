using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AnnounceText : MonoBehaviour
{
    public static AnnounceText aText;
    public static int phase = 0;
    public TextMeshProUGUI announceText;
    public float wait0_1 = 0.5f;
    private Vector3 scaleChange0 = new Vector3(0.05f, 0.0f, 0.00f);
    private Vector3 scaleChange2 = new Vector3(-0.05f, 0.0f, 0.00f);
    private Vector3 scaleChange3 = new Vector3(-0.05f, -0.05f, 0.00f);
    private Vector3 positionChange3 = new Vector3(0.0f, 0.01f, 0.0f);
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
                    if(persist) phase = 3;
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

            case 3:
                if(announceText.transform.localScale.x > 0.5) announceText.transform.localScale += scaleChange3;
                if(announceText.transform.position.y < 3) { 
                    announceText.transform.position += positionChange3; 
                    positionChange3.y += 0.01f;
                }
                
                if(announceText.transform.localScale.x <= 0.5 && announceText.transform.position.y >= 3){
                    announceText.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
                    announceText.transform.position = new Vector3(2.5f, 3.0f, 0.0f);
                    enabled = false;
                }
                break;

            default:
                break;
        }
    }
}
