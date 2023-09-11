using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{

    //Instance of the board
    public Gameplay level;
    //List of possible tokens
    public List<Sprite> tokenList = new List<Sprite>();
    //The board side length (assuming square board)
    public int sideLength = 8;
    //Tile object and grid
    public GameObject tile;
    private GameObject[,] tileGrid;

    float swapSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        //Get the component
        level = GetComponent<Gameplay>();
        //Tile dimensions (square)
        Vector2 tileDimensions = tile.GetComponent<SpriteRenderer>().bounds.size;
        
        //Draw the board
        drawBoard(tileDimensions.x);
    }

    void drawBoard(float tileSideLength){
        //Grid of objects
        tileGrid = new GameObject[sideLength, sideLength];
        //Determine the position of [0, 0] so board is centered
        float initialX = this.transform.position.x - (tileSideLength * sideLength / 2 - (tileSideLength / 2));
        float initialY = this.transform.position.y - (tileSideLength * sideLength / 2 - (tileSideLength / 2));

        //Render tiles and add to array
        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){
                GameObject nextTile = Instantiate(tile, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), tile.transform.rotation); 
                tileGrid[i, j] = nextTile;
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
