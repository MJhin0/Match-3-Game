using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    //Use to determine if a tile has been matched over; a fully matched tile is at 0
    public int matchedState;
    //Possible sprites for a tile
    public List<Sprite> tileList = new List<Sprite>();

    // Start is called before the first frame update
    void Start()
    {
    }

    public void setTileLayer(int layer){
        matchedState = layer;
        gameObject.GetComponent<SpriteRenderer>().sprite = tileList[layer];
    }

    public void breakLayer(){
        if(matchedState == 0) return;
        else{
            Gameplay.level.UpdateScore(matchedState * 25);
            matchedState--;
            gameObject.GetComponent<SpriteRenderer>().sprite = tileList[matchedState];
            if(matchedState == 0) Gameplay.level.tilesCleared++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
