using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gameplay : MonoBehaviour
{

    //Instance of the board when other scripts need it
    public static Gameplay level;
    //List of possible tokens
    public List<Sprite> tokenList = new List<Sprite>();
    //The board side length (assuming square board)
    public int sideLength = 8;
    //Tile object and grid, plus token grid
    public GameObject tile;
    public GameObject token;
    private GameObject[,] tileGrid;
    private GameObject[,] tokenGrid;

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
        //Grids of tiles and tokens
        tileGrid = new GameObject[sideLength, sideLength];
        tokenGrid = new GameObject[sideLength, sideLength];
        //Determine the position of [0, 0] so board is centered
        float initialX = this.transform.position.x - (tileSideLength * sideLength / 2 - (tileSideLength / 2));
        float initialY = this.transform.position.y - (tileSideLength * sideLength / 2 - (tileSideLength / 2));

        //Render tiles and add to array
        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){

                //Instantiate Tile Grid
                GameObject nextTile = Instantiate(tile, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), tile.transform.rotation);
                nextTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                tileGrid[i, j] = nextTile;

                //Instantiate Tokens Randomly
                GameObject nextToken = Instantiate(token, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), token.transform.rotation);
                //Determine token color and set sprite/int
                int tokenType = Random.Range(0, tokenList.Count);
                nextToken.GetComponent<SpriteRenderer>().sprite = tokenList[tokenType];
                nextToken.GetComponent<Token>().type = tokenType;
                nextToken.GetComponent<SpriteRenderer>().sortingOrder = 2;
                tokenGrid[i, j] = nextToken;

            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
