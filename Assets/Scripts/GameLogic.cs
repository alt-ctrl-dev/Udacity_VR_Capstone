using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.Networking;

public class GameLogic : MonoBehaviour {
    public bool playerTurn = true;
    public bool gameEnded = false;
    public bool playerWon = false;
    public bool drawGame = false;

    public GameObject restartPanel;
    public Text restartText;

    public Text AIFace;

    //For our board we will just have a simple array of 9 integers scanning from top left to bottom right.  0 Represents an open slot, 1 represents a player piece, 2 represents an AI piece
    public int[] boardRepresentation;
    public GameObject[] PlayerPieces;
    public GameObject[] AIPieces;



    public Vector3[] PlayerPiecePositions;
    public Vector3[] AIPiecePositions;
    public int AIMoveCount = 0;

    public GameObject[] gridPlates;
    public GameObject helpPanel;

    private const string url = "http://perfecttictactoe.herokuapp.com/api/v2/play";

    //private int Choice = 0;

    // Use this for initialization
    void Start() {
        AIFace.text = ":)";

        //Save our intial positions for a reset
        for (int i =0; i<5; i++) {
            PlayerPiecePositions[i] = PlayerPieces[i].transform.position;
            AIPiecePositions[i] = AIPieces[i].transform.position;
        }

        initBoard();
    }

    // Update is called once per frame
    void Update() {

    }
    public void initBoard() {
        //Initialize our board array to be full of 'empty' 0s
        boardRepresentation = new int[9];
        for (int i = 0; i < 9; i++) {
            boardRepresentation[i] = 0;
            gridPlates[i].SetActive(true);
            gridPlates[i].GetComponent<MeshRenderer>().enabled = false;

        }

        for (int i = 0; i < 5; i++) {
            PlayerPieces[i].transform.position = PlayerPiecePositions[i];
            AIPieces[i].transform.position = AIPiecePositions[i];

            PlayerPieces[i].GetComponent<PlayerPiece>().hasBeenPlayed = false;
            
        }
    }

    public void AIMove() {
        //If it is the AI's turn to move
        //Randomly select an open slot on the board
        //Place a piece there
        //Check for Victory
        //Switch it to the player's turn.
        if (playerTurn == false && gameEnded == false) {
            StartCoroutine(GetBestMove(movePosition => {
                Debug.Log("Best Move = "+ movePosition);

                if (boardRepresentation[movePosition] == 0)
                { //See if that slot is open
                    boardRepresentation[movePosition] = 2; //If it's open, set it to our AI move

                    AIPieces[AIMoveCount].transform.position = new Vector3(gridPlates[movePosition].transform.position.x, gridPlates[movePosition].transform.position.y + 0.3f, gridPlates[movePosition].transform.position.z);
                    AIMoveCount++;
                }

                Invoke("checkForVictory", 1);
                playerTurn = true; //Set the player turn again.
                AIFace.text = ":)";
            }));
            
        }
    }

    private IEnumerator GetBestMove(Action<int> result)
    {
        var jsonString = "{\"player_piece\": \"o\",\"opponent_piece\": \"x\", " +
            "\"board\": [{"+
        "\"id\": \"top-left\",\"value\": \"" +
        (boardRepresentation[0]==1?"x": (boardRepresentation[0] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"top-center\",\"value\": \"" +
        (boardRepresentation[1] == 1 ? "x" : (boardRepresentation[1] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"top-right\",\"value\": \"" +
        (boardRepresentation[2] == 1 ? "x" : (boardRepresentation[2] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"middle-left\",\"value\": \"" +
        (boardRepresentation[3] == 1 ? "x" : (boardRepresentation[3] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"middle-center\",\"value\": \"" +
        (boardRepresentation[4] == 1 ? "x" : (boardRepresentation[4] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"middle-right\",\"value\": \"" +
        (boardRepresentation[5] == 1 ? "x" : (boardRepresentation[5] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"bottom-left\",\"value\": \"" +
        (boardRepresentation[6] == 1 ? "x" : (boardRepresentation[6] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"bottom-center\",\"value\": \"" +
        (boardRepresentation[7] == 1 ? "x" : (boardRepresentation[7] == 2 ? "o" : "")) + "\"" +
        "}, {" +
        "\"id\": \"bottom-right\",\"value\": \"" +
        (boardRepresentation[8] == 1 ? "x" : (boardRepresentation[8] == 2 ? "o" : "")) + "\"" +
        "}]}";

        using (UnityWebRequest request = new UnityWebRequest(url,"POST"))
        {
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonString);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.Send();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                result(-1);
                int movePosition = -1; ;
                for (int i = 0; i > -1; i++)
                {
                    movePosition = UnityEngine.Random.Range(0, 9); //Generate a random movement position
                    if (boardRepresentation[movePosition] == 0) break;
                }
                result(movePosition);
            }
            else
            {
                Debug.Log("Post complete!");
                var jsonObject = JsonUtility.FromJson<MinimaxResponse>(request.downloadHandler.text);

                var index = -1;                
                for (int i = 0; i < jsonObject.data.board.Length; i++)
                {
                    index = -1;
                    switch (jsonObject.data.board[i].id)
                    {
                        case "top-left":
                            index = 0;
                            break;
                        case "top-center":
                            index = 1;
                            break;
                        case "top-right":
                            index = 2;
                            break;
                        case "middle-left":
                            index = 3;
                            break;
                        case "middle-center":
                            index = 4;
                            break;
                        case "middle-right":
                            index = 5;
                            break;
                        case "bottom-left":
                            index = 6;
                            break;
                        case "bottom-center":
                            index = 7;
                            break;
                        case "bottom-right":
                            index = 8;
                            break;
                    }
                    if (!string.IsNullOrEmpty(jsonObject.data.board[i].value))
                    {
                        if (boardRepresentation[index] == 0)
                        {
                            if (jsonObject.data.board[i].value == "o")
                            { break; }
                        }
                    }

                }
                result(index);
            }
        }
    }

    public void gameOver() {
        //Display victory or defeat message
        //Give the player the chance to play again
        gameEnded = true;
        if (playerWon == true) {
            AIFace.text = ":(";
            restartText.text = "You won!";
        } else if (playerWon == false && drawGame == false) {
            AIFace.text = ":D";
            restartText.text = "You lost!";
        } else {
            AIFace.text = ":/";
            restartText.text = "Draw!";
        }
        
        restartPanel.SetActive(true);
    }

    public void newGame() {
        gameEnded = false;
        AIMoveCount = 0;
        initBoard();
        playerTurn = true;
        drawGame = false;
        playerWon = false;
        restartPanel.SetActive(false);

    }

    public void playerMove(GameObject selectedPlate) {
        //Match the numbered plate and update our board representation to match it.
        for(int i =0; i <9;i++) {
            if (selectedPlate == gridPlates[i]) {
                boardRepresentation[i] = 1;
                GetComponent<holdPiece>().pieceBeingHeld.transform.position =
                    new Vector3(selectedPlate.transform.position.x, gridPlates[i].transform.position.y + 0.3f, gridPlates[i].transform.position.z);

            }
        }
        playerTurn = false;
        Invoke("checkForVictory", 1);
        if (gameEnded == false) {
            AIFace.text = ":/";
            Invoke("AIMove", 2);
        }
        
    }

    public void StartTimeoutHelp()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(10.0f);
        helpPanel.SetActive(false);
    }

    public void checkForVictory() {
        //We are doing this elegantly, but that's fine.  There are only 8 possible win conditions in tic tac toe
        if (boardRepresentation[0] == boardRepresentation[1] && boardRepresentation[1] == boardRepresentation[2] && boardRepresentation[0] != 0 && boardRepresentation[1] != 0 && boardRepresentation[2] != 0) { //Victory Detected spanning across top.
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[0] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[3] == boardRepresentation[4] && boardRepresentation[4] == boardRepresentation[5] && boardRepresentation[3] != 0 && boardRepresentation[4] != 0 && boardRepresentation[5] != 0) { //Victory Detected spanning across middle
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[3] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[6] == boardRepresentation[7] && boardRepresentation[7] == boardRepresentation[8] && boardRepresentation[6] != 0 && boardRepresentation[7] != 0 && boardRepresentation[8] != 0) { //Victory Detected spanning across bottom
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[6] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[0] == boardRepresentation[3] && boardRepresentation[3] == boardRepresentation[6] && boardRepresentation[0] != 0 && boardRepresentation[3] != 0 && boardRepresentation[6] != 0) { //Victory Detected spanning across left
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[0] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[1] == boardRepresentation[4] && boardRepresentation[4] == boardRepresentation[7] && boardRepresentation[1] != 0 && boardRepresentation[4] != 0 && boardRepresentation[7] != 0) { //Victory Detected spanning across middle
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[1] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[2] == boardRepresentation[5] && boardRepresentation[5] == boardRepresentation[8] && boardRepresentation[2] != 0 && boardRepresentation[5] != 0 && boardRepresentation[8] != 0) { //Victory Detected spanning across right
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[2] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[0] == boardRepresentation[4] && boardRepresentation[4] == boardRepresentation[8] && boardRepresentation[0] != 0 && boardRepresentation[4] != 0 && boardRepresentation[8] != 0) { //Victory Detected diagonal down
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[0] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        }
        else if (boardRepresentation[2] == boardRepresentation[4] && boardRepresentation[4] == boardRepresentation[6] && boardRepresentation[2] != 0 && boardRepresentation[4] != 0 && boardRepresentation[6] != 0) { //Victory Detected diagonal up
            //if the items are 1s, the AI lost, if the items are 2s the player lost
            gameEnded = true;
            if (boardRepresentation[2] == 1) {
                playerWon = true;
                gameOver();
            }
            else {
                playerWon = false;
                gameOver();
            }
        } else if (boardRepresentation[0] != 0 && boardRepresentation[1] != 0 && boardRepresentation[2] != 0 && boardRepresentation[3] != 0 && boardRepresentation[4] != 0 && boardRepresentation[5] != 0 && boardRepresentation[6] != 0 && boardRepresentation[7] != 0 && boardRepresentation[8] !=0) {
            //If it's a draw
            playerWon = false;
            drawGame = true;
            gameOver();

        }
    }

}

[System.Serializable]
internal struct MinimaxResponse
{
    public string status;
    public MinimaxData data;

    [System.Serializable]
    public struct MinimaxData
    {
        public string player_piece;
        public string opponent_piece;
        public BoardData[] board;

        [System.Serializable]
        public struct BoardData
        {
            public string id;
            public string value;
        }
    }
}