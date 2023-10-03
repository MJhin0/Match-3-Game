using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{

    //Instance of the board when other scripts need it
    public static Gameplay level;
    //List of possible tokens
    public List<Sprite> tokenList = new List<Sprite>();
    //The board dimensions (assuming square board) and top of screen
    public int sideLengthX;
    public int sideLengthY;
    private float topOfScreen;
    //Tile object and grid, plus token grid
    public GameObject tile;
    public GameObject token;

    //Variables to determine draw position
    public float tileSideLength;
    public float initialX;
    public float initialY;
    
    //The game objects for the board and text file path
    public String filePath;
    public GameObject[,] tileGrid;
    public GameObject[,] tokenGrid;

    //Total tiles to break;
    public int tileCount = 0;
    public int tilesCleared = 0;

    //Used when matches are made
    public int chainReactions = 0;
    public int columnsMoving = 0;

    public float gameTime = 10.0f; // total game time in seconds
    private float remainingTime;

    public int score = 0; //Game score
    public Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        //Get the component
        level = GetComponent<Gameplay>();

        topOfScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)).y;

        filePath = Application.streamingAssetsPath + "/levels/1_1.txt";

        Debug.Log(filePath);
        
        //Draw the board
        drawBoard();

        //Set time

        remainingTime = gameTime;
    }

    void drawBoard(){

        //Open the text file and get dimensions
        StreamReader fileRead = new StreamReader(filePath);
        String[] dimensions = fileRead.ReadLine().Split(' ');
        sideLengthX = int.Parse(dimensions[0]);
        sideLengthY = int.Parse(dimensions[1]);
        
        //Buffer file input
        fileRead.ReadLine();
        //Create Board of ints
        int[,] board = new int[sideLengthX, sideLengthY];
        for(int i = sideLengthY - 1; i >= 0; i--){
            String line = fileRead.ReadLine();
            for(int j = 0; j < sideLengthX; j++) board[j, i] = int.Parse(line[j].ToString());
        }
        
        //Get the tile dimensions (it's a square)
        tileSideLength = tile.GetComponent<SpriteRenderer>().bounds.size.x;

        //Determine the position of [0, 0] so board is centered
        initialX = this.transform.position.x - (tileSideLength * sideLengthX / 2 - (tileSideLength / 2));
        initialY = this.transform.position.y - (tileSideLength * sideLengthY / 2 - (tileSideLength / 2));

        //Draw the tiles
        instantiateTiles(tileSideLength, initialX, initialY, board);

        //Draw the tokens
        instantiateTokens(tileSideLength, initialX, initialY);
        
    }

    void instantiateTiles(float tileSideLength, float initialX, float initialY, int[,] board){
        tileGrid = new GameObject[sideLengthX, sideLengthY];
        for(int i = 0; i < sideLengthX; i++){
            for(int j = 0; j < sideLengthY; j++){
                //Get the character from grid, skip if 0
                if(board[i, j] == 0) continue;
                //Instantiate and draw Tile Grid
                GameObject nextTile = Instantiate(tile, new Vector3(initialX + (tileSideLength * i), initialY + 
                    (tileSideLength * j), 0), tile.transform.rotation);
                    nextTile.transform.parent = level.transform;
                nextTile.GetComponent<SpriteRenderer>().sortingOrder = 1;
                nextTile.GetComponent<Tile>().setTileLayer(1);
                tileGrid[i, j] = nextTile;
                tileCount++;
            }
        }
    }

    void instantiateTokens(float tileSideLength, float initialX, float initialY){
        
        tokenGrid = new GameObject[sideLengthX, sideLengthY];
        //SETUP FOR PREVENTING 3 IN A ROW
        //The last element below or to the left to refer to
        int[] lastColumn = new int[sideLengthY];
        for(int i = 0; i < lastColumn.Length; i++) lastColumn[i] = -1;
        int lastUnder = -1;

        //Checks for if two in a row or column has happened
        bool repeatedOnceVertical = false;
        bool[] repeatedOnceHorizontal = new bool[sideLengthY];

        //List of possible tokens, updates each iteration
        List<int> allTokens = new List<int>();
        for(int v = 0; v < tokenList.Count; v++) allTokens.Add(v);
        List<int> allowedTokens = new List<int>();
        allowedTokens.AddRange(allTokens);

        //Render tokens
        for(int i = 0; i < sideLengthX; i++){
            lastUnder = -1;
            repeatedOnceVertical = false;
            for(int j = 0; j < sideLengthY; j++){
                //Skip if no tile
                if(tileGrid[i, j] == null) continue;

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
                int tokenType = allowedTokens[UnityEngine.Random.Range(0, allowedTokens.Count)];

                //Reset list to select from
                allowedTokens = new List<int>();
                allowedTokens.AddRange(allTokens);
                //Check if it matches previous and mark as such
                if(tokenType == lastUnder) repeatedOnceVertical = true;
                if(tokenType == lastColumn[j]) repeatedOnceHorizontal[j] = true;
                //Change lastUnder/lastColumn
                lastUnder = tokenType;
                lastColumn[j] = tokenType;

                //Draw, stop gravity, and add to array
                nextToken.GetComponent<SpriteRenderer>().sprite = tokenList[tokenType];
                nextToken.GetComponent<Token>().type = tokenType;
                nextToken.GetComponent<Token>().setIndex(i, j);
                nextToken.GetComponent<SpriteRenderer>().sortingOrder = 2;
                nextToken.transform.parent = level.transform;
                tokenGrid[i, j] = nextToken;

            }
        }

    }

    public void executeMatch(){
        StartCoroutine(destroyAndReplace());
    }

    public IEnumerator destroyAndReplace(){
        //Only do if tokens stop moving
        yield return new WaitUntil(() => Token.tokensMoving <= 0);
        chainReactions++;
        columnsMoving = 0;
        for(int i = 0; i < sideLengthX; i++)
            for(int j = 0; j < sideLengthY; j++){
                //Skip if no tile there
                if(tileGrid[i, j] == null) continue;
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
        for(int i = 0; i < sideLengthX; i++)
            for(int j = 0; j < sideLengthY; j++){
                //Skip if no tile there
                if(tileGrid[i, j] == null) continue;
                if(tokenGrid[i, j].GetComponent<Token>().findMatch()) matchExists = true;
            }
        if(matchExists) StartCoroutine(destroyAndReplace());
        chainReactions--;
        if(tileCount == tilesCleared) Debug.Log("Clear!");
    }

    private IEnumerator replace(int x, int y){

        //Figure out how many shifts are needed
        int emptyCount = 0;
        //List of distances tokens
        List<int> distances = new List<int>();
        //Highest index tile in the column
        int highestIndex = 0;
        //Destroy tokens and determine drop counts
        for(int i = y; i < sideLengthY; i++){
            if(tileGrid[x, i] != null) highestIndex = i; else continue;
            if (tokenGrid[x, i].GetComponent<Token>().marked){ //destory marked tokens, increment empty count, break tile under
                Destroy(tokenGrid[x, i]);
                tileGrid[x, i].GetComponent<Tile>().breakLayer();
                tokenGrid[x, i] = null;
                emptyCount++;
            }
            else{ //Add this token's drop distance to the list
                distances.Add(emptyCount);
            }

        UpdateScore(emptyCount * 10);    

        }
        //Shift tokens down
        for(int i = emptyCount; i > 0; i--){
            //Keep track/reset of the index in the distance list
            int distIndex = 0;
            //Move the tokens down one
            for(int j = y - 1; j < highestIndex; j++){
                //Only act if the token to fall is actually there
                if(tokenGrid[x, j + 1] != null){
                    //Only act if this token should fall
                    if(distances[distIndex] > 0){

                        //Account for no tile space to fall through
                        int nullTiles = 0;
                        for(int k = j; k > y; k--){
                            if(tileGrid[x, k] == null) nullTiles++;
                            else break;
                        }

                        tokenGrid[x, j - nullTiles] = tokenGrid[x, j + 1];
                        tokenGrid[x, j - nullTiles].GetComponent<Token>().setIndex(x, j - nullTiles);
                        tokenGrid[x, j + 1] = null;
                        //Decrement distance, incrememtn index
                        distances[distIndex]--;
                        distIndex++;
                    }
                    //Otherwise move to next index
                    else distIndex++;
                }
            }
            //Add token to top
            GameObject nextToken = Instantiate(token, new Vector3(initialX + (tileSideLength * x), topOfScreen + emptyCount - i + 1, 0), 
                token.transform.rotation);
            int tokenType = UnityEngine.Random.Range(0, tokenList.Count);
            nextToken.GetComponent<SpriteRenderer>().sprite = tokenList[tokenType];
            nextToken.GetComponent<Token>().type = tokenType;
            nextToken.GetComponent<Token>().setIndex(x, highestIndex);
            nextToken.GetComponent<SpriteRenderer>().sortingOrder = 2;
            nextToken.transform.parent = level.transform;
            tokenGrid[x, highestIndex] = nextToken;
            //Add new token to list of distances
            distances.Add(i - 1);
        }

        yield return new WaitForSeconds(0.1f);

        //Start token drop
        for(int i = y; i <= highestIndex; i++){
            //Only drop if actual tile
            if(tileGrid[x, i] == null) continue;
            tokenGrid[x, i].GetComponent<Token>().setDrop();
        }

        columnsMoving--;
    }

    public void UpdateScore(int points)
    {
        score += points;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;  // Update the score text element
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0 && enabled)
        {
            Debug.Log("Time's up! Final Score: " + score);
            SceneManager.LoadScene("EndScene"); // Load the end screen scene
            return;
        }

        if (score >= 10000)
        {
            Debug.Log("You've reached 100 points!");
            SceneManager.LoadScene("EndScene"); // Load the end screen scene
            return;
        }

    }
}
