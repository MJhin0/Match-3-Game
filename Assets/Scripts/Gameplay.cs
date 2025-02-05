using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{

    //Instance of the board when other scripts need it
    public static Gameplay level;
    //List of possible tokens
    public List<Sprite> tokenList = new List<Sprite>();
    //The board dimensions and top of screen
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
    public TextMeshProUGUI tilesLeftText; 

    //Used when matches are made
    public int chainReactions = 0;
    public int columnsMoving = 0;
    public int combo = 1;

    //Time and Score Variables with Text Fields
    public float remainingTime; // total game time in seconds
    public TextMeshProUGUI timeText;
    public int score = 0; //Game score
    public TextMeshProUGUI scoreText;

    //Variable for if Intro is playing
    public bool allowSwaps = false;
    public bool paused = false;

    //Components for button UI
    public GameObject levelEndRetry;
    public GameObject levelEndQuit;
    public GameObject levelEndContinue;
    public GameObject resumeButton;
    public GameObject pauseText;

    // SFX
    public AudioSource match1;
    public AudioSource match2;
    public AudioSource match3;
    public AudioSource match4;
    public AudioSource match5;

    // Start is called before the first frame update
    void Start()
    {
        IntroText.phase = 0;
        enabled = false;
        //Get the component
        level = GetComponent<Gameplay>();

        topOfScreen = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.nearClipPlane)).y;

        //Set board file, time and score
        filePath = Application.streamingAssetsPath + "/levels/1_" +  PlayerPrefs.GetInt("SelectedLevel", 1) + ".txt";      
        UpdateTime();
        UpdateScore(0);

        //Start the level
        StartCoroutine(levelIntro());

    }

    void drawBoard(){

        //Get the tile dimensions (it's a square)
        tileSideLength = tile.GetComponent<SpriteRenderer>().bounds.size.x;

        //Open the text file and get dimensions
        StreamReader fileRead = new StreamReader(filePath);
        String[] dimensions = fileRead.ReadLine().Split(' ');
        sideLengthX = int.Parse(dimensions[0]);
        sideLengthY = int.Parse(dimensions[1]);

        //Determine the position of [0, 0] so board is centered
        initialX = this.transform.position.x - (tileSideLength * sideLengthX / 2 - (tileSideLength / 2));
        initialY = this.transform.position.y - (tileSideLength * sideLengthY / 2 - (tileSideLength / 2));
                
        //Buffer file input
        fileRead.ReadLine();
        //Create Board of ints
        int[,] board = new int[sideLengthX, sideLengthY];
        for(int i = sideLengthY - 1; i >= 0; i--){
            String line = fileRead.ReadLine();
            for(int j = 0; j < sideLengthX; j++) board[j, i] = int.Parse(line[j].ToString());
        }

        //Buffer file input
        fileRead.ReadLine();
        //Read time limit
        remainingTime = int.Parse(fileRead.ReadLine());
        UpdateTime();

        //Draw the tiles
        instantiateTiles(board);

    }

    public IEnumerator levelIntro(){
        //Draw the board and tiles
        drawBoard();
        //Wait for the text to show
        yield return new WaitUntil(() => IntroText.phase == 1);
        //Draw the tokens
        instantiateTokens();
                yield return new WaitUntil(() => Token.tokensMoving == 0);
        yield return new WaitUntil(() => IntroText.phase == 3);
        enabled = true;
        allowSwaps = true;
    }

    void instantiateTiles(int[,] board){
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
        tilesLeftText.text = "" + (tileCount - tilesCleared);
    }

    void instantiateTokens(){
        
        Token.tokensMoving = 0;
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
                GameObject nextToken = Instantiate(token, new Vector3(initialX + (tileSideLength * i), topOfScreen + j + 1, 0), 
                    token.transform.rotation);

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
                nextToken.GetComponent<Token>().setDrop();
                tokenGrid[i, j] = nextToken;

            }
        }

        if(!hasMovesRemaining()) Shuffle();

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
        yield return new WaitUntil(() => Token.tokensMoving == 0);

        //After everything is refilled, check for matches again for combos
        bool matchExists = false;
        for(int i = 0; i < sideLengthX; i++)
            for(int j = 0; j < sideLengthY; j++){
                //Skip if no tile there
                if(tileGrid[i, j] == null) continue;
                if(tokenGrid[i, j].GetComponent<Token>().findMatch()) matchExists = true;
            }
        if(matchExists) {
            combo += 1;
            StartCoroutine(destroyAndReplace());
            switch (combo) {
                case 1:
                    break; // this SFX is played in token.cs
                case 2: 
                    match2.Play();
                    break;
                case 3:
                    match3.Play();
                    break;
                case 4:
                    match4.Play();
                    break; 
                default:
                    match5.Play();
                    break;
            }
        }
        else{
            combo = 1;
            //Check for win or lose conditions
            if(tileCount == tilesCleared) {
                StartCoroutine(levelComplete());
                yield break;
            }
            if(remainingTime <= 0){
                StartCoroutine(outOfTime());
                yield break;
            }
            if(!hasMovesRemaining()) Shuffle();
        }
        chainReactions--;
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
                //Add to ability
                AbilityExplode.abilityExplode.AddToBar();
                AbilityShuffle.abilityShuffle.AddToBar();
            }
            else{ //Add this token's drop distance to the list
                distances.Add(emptyCount);
            }

        }
        UpdateScore(emptyCount * 1 * combo);
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

    // To shuffle the board
    public void Shuffle() {
        AbilityShuffle.abilityShuffle.PlayShuffle();
        for (int x = 0; x < sideLengthX; x++) {
            for (int y = 0; y < sideLengthY; y++) {
                if ( !(tileGrid[x, y] == null) ){
                    Destroy(tokenGrid[x, y]);
                    tokenGrid[x, y] = null;
                }
            }
        }
        instantiateTokens();
    }

    //These next 3 private methods are solely for the "No More Moves" algorithm
    public bool hasMovesRemaining() {

        //make array to compare, use -1 for no tile spaces
        int[,] tokens = new int[sideLengthX, sideLengthY];
        for(int i = 0; i < sideLengthX; i++){
            for(int j = 0; j < sideLengthY; j++){
                if(tileGrid[i, j] == null) tokens[i, j] = -1;
                else tokens[i, j] = tokenGrid[i, j].GetComponent<Token>().type;
            }
        }

        //Check Horizontal Swaps
        for(int i = 0; i < sideLengthX - 1; i++){
            for(int j = 0; j < sideLengthY; j++){
                if(tokens[i, j] == -1 || tokens[i + 1, j] == -1) continue;
                swapTokens(tokens, i, j, i+1, j);
                if(findMatch(tokens, i, j) || findMatch(tokens, i+1, j)) return true;
                swapTokens(tokens, i, j, i+1, j);
            }
        }

        //Check Vertical Swaps
        for(int i = 0; i < sideLengthX; i++){
            for(int j = 0; j < sideLengthY - 1; j++){
                if(tokens[i, j] == -1 || tokens[i, j + 1] == -1) continue;
                swapTokens(tokens, i, j, i, j+1);
                if(findMatch(tokens, i, j) || findMatch(tokens, i, j+1)) return true;
                swapTokens(tokens, i, j, i, j+1);
            }
        }
        return false;
    }

    private bool findMatch(int[,] tokens, int x, int y){

        int verticalCount = 0;
        int horizontalCount = 0;

        //Check above
        for(int i = y + 1; i < sideLengthY; i++){
            if(tokens[x, i] == -1) break;
            if(tokens[x, i] == tokens[x, y]) verticalCount++;
            else break;
        }
        //Check below
        for(int i = y - 1; i >= 0; i--){
            if(tokens[x, i] == -1) break;
            if(tokens[x, i] == tokens[x, y]) verticalCount++;
            else break;
        }
        //If vertical has match, return
        if(verticalCount >= 2) return true;

        //Check right
        for(int i = x + 1; i < sideLengthX; i++){
            if(tokens[i, y] == -1) break;
            if(tokens[i, y] == tokens[x, y]) horizontalCount++;
            else break;
        }
        //Check left
        for(int i = x - 1; i >= 0; i--){
            if(tokens[i, y] == -1) break;
            if(tokens[i, y] == tokens[x, y]) horizontalCount++;
            else break;
        }
        //If horizontal has match, return
        if(horizontalCount >= 2) return true;

        return false;
    }

    private void swapTokens(int[,] tokens, int x1, int y1, int x2, int y2){
        int temp = tokens[x1, y1];
        tokens[x1, y1] = tokens[x2, y2];
        tokens[x2, y2] = temp;
    }

    public void UpdateScore(int points)
    {
        score += points;
        if (scoreText != null)
        {
            scoreText.text = "" + score;  // Update the score text element
        }
    }

    public void UpdateTime(){
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private IEnumerator outOfTime(){
        //Disable level
        enabled = false;
        allowSwaps = false;
        remainingTime = 0;
        UpdateTime();
        //Announce Loss
        AnnounceText.sendText("Time's Up!", true);
        yield return new WaitForSeconds(1.0f);
        levelEndRetry.SetActive(true);
        levelEndQuit.SetActive(true);
    }

    private IEnumerator levelComplete(){
        //Disable level
        enabled = false;
        allowSwaps = false;
        remainingTime = (float) Math.Floor(remainingTime);
        UpdateTime();
        //Announce win
        AnnounceText.sendText("Level Complete!", true);
        yield return new WaitForSeconds(1.0f);
        levelEndRetry.SetActive(true);
        levelEndContinue.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        tilesLeftText.text = "" + (tileCount - tilesCleared);
        allowSwaps = !(chainReactions > 0 || Token.tokensMoving > 0);

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTime();
        }
        else if(remainingTime <= 0 && chainReactions == 0 && allowSwaps){
            StartCoroutine(outOfTime());
        }

        if(remainingTime < 0) {
            remainingTime = 0;
            UpdateTime();
        }
    }
}
