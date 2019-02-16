using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class PlayerAgent : MonoBehaviour
{
    private int action;
	  private int lastState;
    private int currentState;
	  public float reward;
    public bool isShoot;
    public float h;
    public float v;
    public int roatationAngle;
    public Quaternion newRotation;

    private int time = 500;
    System.Diagnostics.Stopwatch stopwatch;
    private bool isInAction = false;
    System.Diagnostics.Stopwatch iterationWatch;
    public int numberOfKilled = 0;

    //parameters for Q learning
    float learning_rate = 0.5f;
    float gamma = 0.99f;
    float e = 1; // Initial epsilon value for random action selection.
    float eMin = 0.1f; // Lower bound of epsilon.
    int annealingSteps = 2000; // Number of steps to lower e to eMin.
    
    string filename = "./qtable.txt";


    void Awake()
    {
        print("iteration= " + ++Qdata.iteration);

        
        //Default parameters
        // True: shoot; False: not shoot
        isShoot = true;
        // h: [-1, 0, 1] (Left, Right); v: [-1, 0, 1] (Down, up)
        h = 0; v = 0;
        // Orientation angle from 0 to 359
        roatationAngle = 45;
        newRotation = Quaternion.Euler(0, roatationAngle, 0);

        action = -1;
        lastState = GetState(transform.position);
        iterationWatch = System.Diagnostics.Stopwatch.StartNew();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //First call or an action timestamp is up
        if (!isInAction || stopwatch.ElapsedMilliseconds > time)
        {
            currentState = GetState(transform.position);

            if (isInAction) {
                //learning
                SendState(reward, false);
            }

            //Select next action
            reward = 0;
            makeAction(GetAction());
            stopwatch = System.Diagnostics.Stopwatch.StartNew();

            isInAction = true;
        }
    }

    private void selectRandomMovement()
    {
        int movementRandom = Random.Range(0, 9);
    }

    private void selectRandomOrient()
    {
        roatationAngle = Random.Range(0, 360);
        newRotation = Quaternion.Euler(0, roatationAngle, 0);
    }

    public void updateReward(float score)
    {
        reward += score;
    }

    public int GetState(Vector3 position)
    {
        // rotate the grid to a rectangle
        Vector3 pos_new = Quaternion.AngleAxis(-45, Vector3.up) * position;
        // reset the origin
        pos_new = pos_new + new Vector3(25f, 0, 25f);
        // the basic unit of a position is 0.1
        pos_new *= 10;
        
        // get the state index
        return (int) pos_new.x * 500 + (int) pos_new.z;
    }
    
    public int GetAction()
    {
        action = Qdata.q_table[lastState].ToList().IndexOf(Qdata.q_table[lastState].Max());
        if (Random.Range(0f, 1f) < e) { action = Random.Range(0, 3240); }
        if (e > eMin) { e = e - ((1f - eMin) / (float)annealingSteps); }
        return action;
    }

    private void makeAction(int action)
    {
        changeMovement(action / 360);
        changeOrientation(action % 360);
    }

    private void changeMovement(int move) {
        switch (move) {
            //up
            case 0:
                h = 0f;
                v = 0.1f;
                break;
            //no move
            case 1:
                h = 0f;
                v = 0f;
                break;
            //down
            case 2:
                h = 0f;
                v = -0.1f;
                break;
            //up right
            case 3:
                h = 0.1f;
                v = 0.1f;
                break;
            //right
            case 4:
                h = 0.1f;
                v = 0f;
                break;
            //down right
            case 5:
                h = 0.1f;
                v = -0.1f;
                break;
            //up left
            case 6:
                h = -0.1f;
                v = 0.1f;
                break;
            //left
            case 7:
                h = -0.1f;
                v = 0f;
                break;
            //down left
            case 8:
                h = -0.1f;
                v = -0.1f;
                break;
            default:
                h = 0f;
                v = 0f;
                break;
        }
    }

    private void changeOrientation(int roatationAngle)
    {
        newRotation = Quaternion.Euler(0, roatationAngle, 0);
    }

    public void SendState(float reward, bool done)
    {
        if (action != -1) {
            if (done == true) {
                Qdata.q_table[lastState][action] += learning_rate * (reward - Qdata.q_table[lastState][action]);
            }
            else {
                Qdata.q_table[lastState][action] += learning_rate * (reward + gamma * Qdata.q_table[currentState].Max() - Qdata.q_table[lastState][action]);
            }
        }
        lastState = currentState;
    }

    public void GameOver()
    {
        SaveQtable();
    }

    private void SaveQtable()
    {
        // update qtable for the terminal state
        SendState(reward, true);

        iterationWatch.Stop();
        print("Game time= " + iterationWatch.ElapsedMilliseconds + ", Final score= " + ScoreManager.score);

        /*
        System.IO.StreamWriter file = new System.IO.StreamWriter(filename);
        for (int rowIndex = 0; rowIndex < Qdata.state_size; rowIndex++)
        {
            System.Text.StringBuilder line = new System.Text.StringBuilder();
            for (int colIndex = 0; colIndex < Qdata.action_size; colIndex++)
                line.Append(Qdata.q_table[rowIndex][colIndex]).Append("\t");
            file.WriteLine(line);
        }
        file.Close();
        */
    }
}