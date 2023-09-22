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
    public int sideLength;
    //Tile object and grid, plus token grid
    public GameObject tile;
    public GameObject token;

    //Variables to determine draw position
    public float tileSideLength;
    public float initialX;
    public float initialY;
    
    //The game objects for the board
    public GameObject[,] tileGrid;
    public GameObject[,] tokenGrid;

    //Used when matches are made
    public int chainReactions = 0;
    public int columnsMoving = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Get the component
        level = GetComponent<Gameplay>();
        
        //Draw the board
        drawBoard();
    }

    void drawBoard(){
        
        //Get the tile dimensions (it's a square)
        tileSideLength = tile.GetComponent<SpriteRenderer>().bounds.size.x;

        //Determine the position of [0, 0] so board is centered
        initialX = this.transform.position.x - (tileSideLength * sideLength / 2 - (tileSideLength / 2));
        initialY = this.transform.position.y - (tileSideLength * sideLength / 2 - (tileSideLength / 2));

        //Draw the tiles
        instantiateTiles(tileSideLength, initialX, initialY);

        //Draw the tokens
        instantiateTokens(tileSideLength, initialX, initialY);
        
    }

    void instantiateTiles(float tileSideLength, float initialX, float initialY){
        tileGrid = new GameObject[sideLength, sideLength];
        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){
                //Instantiate and draw Tile Grid
                GameObject nextTile = Instantiate(tile, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), tile.transform.rotation);
                nextTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                tileGrid[i, j] = nextTile;
            }
    }

    void instantiateTokens(float tileSideLength, float initialX, float initialY){
        
        tokenGrid = new GameObject[sideLength, sideLength];
        //SETUP FOR PREVENTING 3 IN A ROW
        //The last element below or to the left to refer to
        int[] lastColumn = new int[sideLength];
        for(int i = 0; i < lastColumn.Length; i++) lastColumn[i] = -1;
        int lastUnder = -1;

        //Checks for if two in a row or column has happened
        bool repeatedOnceVertical = false;
        bool[] repeatedOnceHorizontal = new bool[sideLength];

        //List of possible tokens, updates each iteration
        List<int> allTokens = new List<int>();
        for(int v = 0; v < tokenList.Count; v++) allTokens.Add(v);
        List<int> allowedTokens = new List<int>();
        allowedTokens.AddRange(allTokens);

        //Render tokens
        for(int i = 0; i < sideLength; i++){
            lastUnder = -1;
            repeatedOnceVertical = false;
            for(int j = 0; j < sideLength; j++){

                //Instantiate Tokens
                GameObject nextToken = Instantiate(token, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), token.transform.rotation);

                //Determine token color
                //Remove from list if applicable
                if(repeatedOnceVertical){
                    allowedTokens.Remove(lastUnder);
                    repeatedOnceVertical = false;
                }
                if(repeatedOnceHorizontal[j]){
                    allowedTokens.Remove(lastColumn[j]);
                    repeatedOnceHorizontal[j] = false;
                }
                
                //Select token
                int tokenType = allowedTokens[Random.Range(0, allowedTokens.Count)];

                //Reset list to select from
                allowedTokens = new List<int>();
                allowedTokens.AddRange(allTokens);
                //Check if it matches previous and mark as such
                if(tokenType == lastUnder) repeatedOnceVertical = true;
                if(tokenType == lastColumn[j]) repeatedOnceHorizontal[j] = true;
                //Change lastUnder/lastColumn
                lastUnder = tokenType;
                lastColumn[j] = tokenType;

                //Draw and add to array
                nextToken.GetComponent<SpriteRenderer>().sprite = tokenList[tokenType];
                nextToken.GetComponent<Token>().type = tokenType;
                nextToken.GetComponent<Token>().setIndex(i, j);
                nextToken.GetComponent<SpriteRenderer>().sortingOrder = 2;
                tokenGrid[i, j] = nextToken;

            }
        }

    }

    public void executeMatch(){
        StartCoroutine(destroyAndReplace());
    }

    public IEnumerator destroyAndReplace(){
        chainReactions++;
        columnsMoving = 0;
        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){
                if(tokenGrid[i, j].GetComponent<Token>().marked) {
                    columnsMoving++;
                    StartCoroutine(replace(i, j));
                    break;
                }
            }

        //Wait until columns are done moving
        yield return new WaitUntil(() => columnsMoving <= 0);
        yield return new WaitForSeconds(0.5f);

        //After everything is refilled, check for matches again for combos
        bool matchExists = false;
        for(int i = 0; i < sideLength; i++)
            for(int j = 0; j < sideLength; j++){
                if(tokenGrid[i, j].GetComponent<Token>().findMatch()) matchExists = true;
            }
        if(matchExists) StartCoroutine(destroyAndReplace());
        chainReactions--;
    }

    private IEnumerator replace(int x, int y){

        //Figure out how many shifts are needed
        int emptyCount = 0;
        //List of distances tokens
        List<int> distances = new List<int>();
        for(int i = y; i < sideLength; i++)
            if (tokenGrid[x, i].GetComponent<Token>().marked){ //destory marked tokens, increment empty count
                Destroy(tokenGrid[x, i]);
                tokenGrid[x, i] = null;
                emptyCount++;
            }
            else{ //Add this token's drop distance to the list
                distances.Add(emptyCount);
            }
                    
        //Shift tokens down
        for(int i = emptyCount; i > 0; i--){
            //Delay for each tile moved down
            yield return new WaitForSeconds(0.1f);
            //Keep track/reset of the index in the distance list
            int distIndex = 0;
            //Move the tokens down one
            for(int j = y - 1; j < sideLength - 1; j++){
                //Only act if the token to fall is actually there
                if(tokenGrid[x, j + 1] != null){
                    //Only act if this token should fall
                    Debug.Log(distIndex);
                    if(distances[distIndex] > 0){
                        tokenGrid[x, j] = tokenGrid[x, j + 1];
                        tokenGrid[x, j].GetComponent<Token>().setIndex(x, j);
                        tokenGrid[x, j].GetComponent<Token>().move();
                        tokenGrid[x, j + 1] = null;
                        //Decrement distance, incrememtn index
                        distances[distIndex]--;
                        distIndex++;
                    }
                    //Otherwise move to next index
                    else {distIndex++; Debug.Log("increment");}
                }
            }
            //Add token to top
            GameObject nextToken = Instantiate(token, new Vector3(initialX + (tileSideLength * x), initialY + 
                    (tileSideLength * (sideLength - 1)), 0), token.transform.rotation);
            int tokenType = Random.Range(0, tokenList.Count);
            nextToken.GetComponent<SpriteRenderer>().sprite = tokenList[tokenType];
            nextToken.GetComponent<Token>().type = tokenType;
            nextToken.GetComponent<Token>().setIndex(x, sideLength - 1);
            nextToken.GetComponent<SpriteRenderer>().sortingOrder = 2;
            tokenGrid[x, sideLength - 1] = nextToken;
            //Add new token to list of distances
            distances.Add(i - 1);
            Debug.Log(distances.Count);
        }

        columnsMoving--;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
