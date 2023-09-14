using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Token : MonoBehaviour
{

    //Determine the color (blue token vs yellow token)
    //Array positions when calling the board
    public int type;
    public int indexX;
    public int indexY;

    private Token token;
    private bool isSelected;
    private static Token lastSelected = null;
    private static Color selectFade = new Color(1f, 1f, 1f, 0.5f);
    private static Color deselectFade = new Color(1f, 1f, 1f, 1f);

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
        if(isSelected) Deselect();
        else{
            if(lastSelected == null) Select();
            else{
                SwapTokens(lastSelected);
                lastSelected.Deselect();
            }
        }
    }

    public void SwapTokens(Token swappedToken){
        //Return if they are the same color, no need to swap
        if(token.type == swappedToken.type) return;

        //Swap in the token array
        GameObject temp = Gameplay.level.tokenGrid[token.indexX, token.indexY];
        Gameplay.level.tokenGrid[token.indexX, token.indexY] = Gameplay.level.tokenGrid[swappedToken.indexX, swappedToken.indexY];
        Gameplay.level.tokenGrid[token.indexX, token.indexY] = temp;

        //Swap in physical location
        Vector3 position = Gameplay.level.tokenGrid[token.indexX, token.indexY].transform.position;
        Gameplay.level.tokenGrid[token.indexX, token.indexY].transform.position = Gameplay.level.tokenGrid[swappedToken.indexX, swappedToken.indexY].transform.position;
        Gameplay.level.tokenGrid[swappedToken.indexX, swappedToken.indexY].transform.position = position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
