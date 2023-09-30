using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Analytics;
using Vector3 = UnityEngine.Vector3;

public class Token : MonoBehaviour
{

    //Determine the color (blue token vs yellow token)
    //Array positions when calling the board
    public int type;
    public int indexX;
    public int indexY;
    public bool marked = false;

    //Selection variables
    private Token token;
    private bool isSelected;
    private static Token lastSelected = null;
    private static Color selectFade = new Color(1f, 1f, 1f, 0.5f);
    private static Color deselectFade = new Color(1f, 1f, 1f, 1f);

    //Variables for determine if a token is in motion, and swap speed
    private bool isMoving = false;
    public static int tokensMoving = 0;
    private Vector3 intendedPosition;
    private float swapSpeed = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
    }
    
    //Gets the token object upon instantiation
    void Awake()
    {
        token = GetComponent<Token>();
    }

    //Called on instantiation
    public void setIndex(int x, int y){
        indexX = x;
        indexY = y;
    }

    public void Select(){
        isSelected = true;
        GetComponent<SpriteRenderer>().color = selectFade;
        lastSelected = token;
    }

    //Deselect a token: brighten token and 
    public void Deselect(){
        isSelected = false;
        GetComponent<SpriteRenderer>().color = deselectFade;
        lastSelected = null;
    }

    //Selects/Deselects tokens
    void OnMouseDown() {
        if(Gameplay.level.chainReactions > 0) return;
        if(isSelected) Deselect();
        else{
            if(lastSelected == null) Select();
            else{
                SwapTokens(lastSelected);
            }
        }
    }

    public void SwapTokens(Token swappedToken){
        //Return if they are the same color or not adjacent, no need to swap
        if(token.type == swappedToken.type || 
            (Mathf.Abs(token.indexX - swappedToken.indexX) + Mathf.Abs(token.indexY - swappedToken.indexY) > 1)) {
                lastSelected.Deselect();
                Select();
                return;
            }

        //Swap array positions in the Gameplay object
        GameObject temp = Gameplay.level.tokenGrid[token.indexX, token.indexY];
        Gameplay.level.tokenGrid[token.indexX, token.indexY] = Gameplay.level.tokenGrid[swappedToken.indexX, swappedToken.indexY];
        Gameplay.level.tokenGrid[swappedToken.indexX, swappedToken.indexY] = temp;

        //Swap array positions in the Token objects
        int tempX = token.indexX;
        int tempY = token.indexY;
        token.setIndex(swappedToken.indexX, swappedToken.indexY);
        swappedToken.setIndex(tempX, tempY);

        //Swap in physical location
        setTarget();
        swappedToken.setTarget();

        //Check for matches
        bool match1 = findMatch();
        bool match2 = lastSelected.findMatch();
        if(match1 || match2) {
            lastSelected.Deselect();
            //StartCoroutine(Gameplay.level.destroyAndReplace());
            Gameplay.level.executeMatch();
        }else lastSelected.Deselect();

    }

    public void move(){
        Gameplay.level.tokenGrid[indexX, indexY].transform.position = new Vector3(Gameplay.level.initialX + (Gameplay.level.tileSideLength * indexX), 
            Gameplay.level.initialY + (Gameplay.level.tileSideLength * indexY), 0);
    }

    public void setTarget(){
        isMoving = true;
        tokensMoving++;
        intendedPosition = new Vector3(Gameplay.level.initialX + (Gameplay.level.tileSideLength * indexX), 
            Gameplay.level.initialY + (Gameplay.level.tileSideLength * indexY), 0);
    }

    public bool findMatch(){
        //Lists to track what has been matched
        List<GameObject> matched = new List<GameObject>();
        List<GameObject> verticalMatched = new List<GameObject>();
        List<GameObject> horizontalMatched = new List<GameObject>();
        bool isMatch = false;

        //Token to compare with
        Token compare;

        //Counts for what's matched
        int verticalCount = 0;
        int horizontalCount = 0;

        //Check above
        for(int i = indexY + 1; i < Gameplay.level.sideLengthY; i++){
            //Stop tracking if a no tile spot is hit before the board side
            if(Gameplay.level.tileGrid[indexX, i] == null) break;
            compare = Gameplay.level.tokenGrid[indexX, i].GetComponent<Token>();
            if(type == compare.type){
                verticalCount++;
                verticalMatched.Add(Gameplay.level.tokenGrid[indexX, i]);
            }
            else break;
        }
        //Check below
        for(int i = indexY - 1; i >= 0; i--){
            //Stop tracking if a no tile spot is hit before the board side
            if(Gameplay.level.tileGrid[indexX, i] == null) break;
            compare = Gameplay.level.tokenGrid[indexX, i].GetComponent<Token>();
            if(type == compare.type){
                verticalCount++;
                verticalMatched.Add(Gameplay.level.tokenGrid[indexX, i]);
            }
            else break;
        }
        //Check if vertical match is made
        if(verticalCount >= 2){
            isMatch = true;
            matched.AddRange(verticalMatched);
        }

        //Check right
        for(int i = indexX + 1; i < Gameplay.level.sideLengthX; i++){
            //Stop tracking if a no tile spot is hit before the board side
            if(Gameplay.level.tileGrid[i, indexY] == null) break;
            compare = Gameplay.level.tokenGrid[i, indexY].GetComponent<Token>();
            if(type == compare.type){
                horizontalCount++;
                horizontalMatched.Add(Gameplay.level.tokenGrid[i, indexY]);
            }
            else break;
        }
        //Check left
        for(int i = indexX - 1; i >= 0; i--){
            //Stop tracking if a no tile spot is hit before the board side
            if(Gameplay.level.tileGrid[i, indexY] == null) break;
            compare = Gameplay.level.tokenGrid[i, indexY].GetComponent<Token>();
            if(type == compare.type){
                horizontalCount++;
                horizontalMatched.Add(Gameplay.level.tokenGrid[i, indexY]);
            }
            else break;
        }
        //Check if horizontal match is made
        if(horizontalCount >= 2){
            isMatch = true;
            matched.AddRange(horizontalMatched);
        }

        //Check if matched
        if(isMatch) destroyTokens(matched);
        return isMatch;
    }

    private void destroyTokens(List<GameObject> tokens){
        for(int i = 0; i < tokens.Count; i++){
            tokens[i].GetComponent<Token>().marked = true;
        }
        marked = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving){
            transform.position = Vector3.MoveTowards(transform.position, intendedPosition, swapSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, intendedPosition) < 0.001f) {
                isMoving = false;
                tokensMoving--;
            }
        }
    }
}
