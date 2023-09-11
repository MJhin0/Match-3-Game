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
        level = GetComponent<Gameplay>();
        Vector2 location = tile.GetComponent<SpriteRenderer>().bounds.size;
        
        drawBoard(location.x, location.y);
    }

    void drawBoard(float x, float y){
        tileGrid = new GameObject[sideLength, sideLength];
        float initialX = transform.position.x - (x * sideLength / 2);
        float initialY = transform.position.y - (y * sideLength / 2);

        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){
                GameObject nextTile = Instantiate(tile, new Vector3(initialX + (x * i), initialY + 
                    (y * j), 0), tile.transform.rotation); 
                tileGrid[(int) x, (int) y] = nextTile;
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
