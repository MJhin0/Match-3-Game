using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    //Use to determine if a tile has been matched over; a fully matched tile is at 0
    public int matchedState;

    // Start is called before the first frame update
    void Start()
    {
        matchedState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
