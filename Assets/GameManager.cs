using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image[] gameBoard;
    //Play Parameters
    bool playing = true;
    bool humanTurn = true;
    //Images to be used
    [SerializeField] Sprite Blank_image;
    [SerializeField] Sprite X_Image;
    [SerializeField] Sprite O_Image;
    
    //UI Elements
    [SerializeField] Text GameOverText;
    [SerializeField] Button ResetButton;

    void Start()
    { FreshGameState(); }

    void Update()
    {
        if (playing)
        {
            if (!humanTurn)
            {
                AI_Turn();
                humanTurn = true;
            }
        }
    }

    public void ImageClicked(int tileNumber)
    {
        if (humanTurn && playing && gameBoard[tileNumber].sprite == Blank_image) 
        {
            gameBoard[tileNumber].sprite = X_Image;
            if (CheckForWin() == true)
            {
                playing = false;
                GameOverText.text = "Human has won the game";
                ResetButton.gameObject.SetActive(true);
            }
            humanTurn = false;
        }
    }

    private void AI_Turn()
    {
        int bestMove = -1000;
        int occupiedCells = 0;
        Image bestSquare = null;
        
        foreach (Image square in gameBoard)
        {
            if (square.sprite == Blank_image)
            {
                square.sprite = O_Image;
                if (CheckForWin() == true)
                {
                    playing = false;
                    GameOverText.text = "AI has won the game";
                    ResetButton.gameObject.SetActive(true);
                    return;
                }

                int moveValue = MinTurn();
                square.sprite = Blank_image;
                if (moveValue > bestMove)
                {
                    bestMove = moveValue;
                    bestSquare = square;
                }
            }
            else
            {
                occupiedCells += 1;
                if (occupiedCells >= 9)
                {
                    TieGame();
                    GameOverText.text = "Tie Game!";
                    ResetButton.gameObject.SetActive(true);
                }
            }
        }
        if (bestSquare != null)  //This code might be redundant now - for bestSquare to be null, it would need to have found NO possible squares, which should already go into TieGame();
        { bestSquare.sprite = O_Image; }
    }

    private bool CheckForWin()
    {
        return 
           (gameBoard[0].sprite == gameBoard[1].sprite && gameBoard[1].sprite == gameBoard[2].sprite && gameBoard[0].sprite != Blank_image || //Horizontal
            gameBoard[3].sprite == gameBoard[4].sprite && gameBoard[4].sprite == gameBoard[5].sprite && gameBoard[3].sprite != Blank_image ||
            gameBoard[6].sprite == gameBoard[7].sprite && gameBoard[7].sprite == gameBoard[8].sprite && gameBoard[6].sprite != Blank_image ||
            gameBoard[0].sprite == gameBoard[3].sprite && gameBoard[3].sprite == gameBoard[6].sprite && gameBoard[0].sprite != Blank_image || //Vertical
            gameBoard[1].sprite == gameBoard[4].sprite && gameBoard[4].sprite == gameBoard[7].sprite && gameBoard[1].sprite != Blank_image ||
            gameBoard[2].sprite == gameBoard[5].sprite && gameBoard[5].sprite == gameBoard[8].sprite && gameBoard[2].sprite != Blank_image ||
            gameBoard[0].sprite == gameBoard[4].sprite && gameBoard[4].sprite == gameBoard[8].sprite && gameBoard[0].sprite != Blank_image || //Diagonal
            gameBoard[2].sprite == gameBoard[4].sprite && gameBoard[4].sprite == gameBoard[6].sprite && gameBoard[2].sprite != Blank_image);
    }

    private int MaxTurn()
    {
        bool spaceAvailable = false;
        foreach (Image square in gameBoard)
        {
            if (square.sprite == Blank_image)
            {
                spaceAvailable = true;
                break;
            }
        }
        
        if (CheckForWin() == true)
        { return -10; }

        else if (!spaceAvailable)
        { return 0; }

        int bestMove = -1000;
        foreach (Image square in gameBoard)
            {
                if (square.sprite == Blank_image)
                {
                    square.sprite = O_Image;
                    bestMove = math.max(bestMove, MinTurn());
                    square.sprite = Blank_image;
                }
            }
        return bestMove;
    }

    private int MinTurn()
    {
        bool spaceAvailable = false;
        foreach (Image square in gameBoard)
        {
            if (square.sprite == Blank_image)
            {
                spaceAvailable = true;
                break;
            }
        }

        if (CheckForWin() == true)
        { return 10; }

        else if (!spaceAvailable)
        { return 0;  }

        int bestMove = 1000;
        foreach (Image square in gameBoard)
        {
            if (square.sprite == Blank_image)
            {
                square.sprite = X_Image;
                bestMove = math.min(bestMove, MaxTurn());
                square.sprite = Blank_image;
            }
        }
        return bestMove;
    }

    private void TieGame()
    {
        playing = false;
        Debug.Log("Tie Game");
    }

    public void FreshGameState()
    {
        GameOverText.text = "";
        foreach (Image image in gameBoard)
        { image.sprite = Blank_image; }
        playing = true;
        ResetButton.gameObject.SetActive(false);
    }
}