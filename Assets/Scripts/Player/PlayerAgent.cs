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
    //private int state_size;
    //private int action_size;
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
    //float[][] q_table;
    float learning_rate = 0.5f;
    float gamma = 0.99f;
    float e = 1f; // Initial epsilon value for random action selection.
    float ei = 1f;
    float eMin = 0.1f; // Lower bound of epsilon.
    int annealingSteps = 1000; // Number of steps to lower e to eMin.
    string FileName = "C:\\Users\\metsu\\Desktop\\qtable.txt";
    //bool isGameOver = false;

    //GameObject player;
    //PlayerShooting playerShooting;
    PlayerHealth playerHealth;

    void Awake()
    {
        //Qdata.qdata.initQtable();
        print("iteration= " + ++Qdata.qdata.iteration);
        playerHealth = GetComponent<PlayerHealth>();
        
        switch ((int)Qdata.qdata.iteration/50)
        {
            case 1:
                e = 0.9f; ei = 0.9f;
                break;
            case 2:
                e = 0.8f; ei = 0.8f;
                break;
            case 3:
                e = 0.7f; ei = 0.7f;
                break;
            case 4:
                e = 0.6f; ei = 0.6f;
                break;
            case 5:
                e = 0.5f; ei = 0.5f;
                break;
            case 6:
                e = 0.4f; ei = 0.4f;
                break;
            case 7:
                e = 0.3f; ei = 0.3f;
                break;
        }
        //Qdata.qdata.initQtable();
        /*
        //Update Q table
        if (File.Exists(FileName)) {
            print("Load Q table start....");
            int i = 0;
            string[] stringArray = System.IO.File.ReadAllLines(@FileName);
            foreach (string example in stringArray) {
                int j = 0;
                string[] array = example.Split(new char[] { '\t' });
                foreach (string value in array) {
                    if (value.Trim() != "") {
                        q_table[i][j] = System.Convert.ToSingle(value.Trim());
                        ++j;
                    }
                }
                ++i;
            }
            print("Load Q table end....");
        }
        */

        //Default parameters
        // True: shoot; False: not shoot
        //Always shoot for now
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
        /*
        if (ScoreManager.score >= 10)
        {
            playerHealth.killAllEnemy();
        }
        */
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
            //print("lastState= " + lastState + ", action= " + action);
            //selectRandomMovement();
            //selectRandomOrient();
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
        Vector3 pos_new = Quaternion.AngleAxis(-45, Vector3.up) * position;
        pos_new = pos_new + new Vector3(25f, 0, 25f);
        pos_new *= 10;
        return (int) pos_new.x * 500 + (int) pos_new.z;
    }
    
    public int GetAction()
    {
        // get action list which has the same value
        float max = Qdata.qdata.q_table[lastState].Max();
        int[] action_list = Enumerable.Range(0, Qdata.qdata.q_table[lastState].Length).Where(t => Qdata.qdata.q_table[lastState][t] == max).ToArray();
        // random pick one action with max value
        action = action_list[Random.Range(0, action_list.Length)];
        action_list = null;

        //action = Qdata.qdata.q_table[lastState].ToList().IndexOf(Qdata.qdata.q_table[lastState].Max());
        if (Random.Range(0f, 1f) < e) { action = Random.Range(0, 3240); }
        if (e > eMin) { e = e - ((ei - eMin) / (float)annealingSteps); }
        return action;
    }

    private void makeAction(int action)
    {
        changeMovement(action / 360);
        changeOrientation(action % 360);
    }

    private void changeMovement(int move) {
        switch (move) {
            case 1:
                h = 0f;
                v = 0.1f;
                break;
            case 0:
                h = 0f;
                v = 0f;
                break;
            case 2:
                h = 0f;
                v = -0.1f;
                break;
            case 3:
                h = 0.1f;
                v = 0.1f;
                break;
            case 4:
                h = 0.1f;
                v = 0f;
                break;
            case 5:
                h = 0.1f;
                v = -0.1f;
                break;
            case 6:
                h = -0.1f;
                v = 0.1f;
                break;
            case 7:
                h = -0.1f;
                v = 0f;
                break;
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
                Qdata.qdata.q_table[lastState][action] += learning_rate * (reward - Qdata.qdata.q_table[lastState][action]);
            }
            else {
                Qdata.qdata.q_table[lastState][action] += learning_rate * (reward + gamma * Qdata.qdata.q_table[currentState].Max() - Qdata.qdata.q_table[lastState][action]);
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
        //learn the last time
        SendState(reward, true);

        iterationWatch.Stop();
        print("Game time= " + iterationWatch.ElapsedMilliseconds);
        print("Final score= " + ScoreManager.score);

        /*
        List<string> linesToWrite = new List<string>();
        print("data collection start....");
        for (int rowIndex = 0; rowIndex < state_size; rowIndex++)
        {
            System.Text.StringBuilder line = new System.Text.StringBuilder();
            for (int colIndex = 0; colIndex < action_size; colIndex++)
                line.Append(q_table[rowIndex][colIndex]).Append("\t");
            linesToWrite.Add(line.ToString());
        }
        print("data collection end....");
        //File.WriteAllLines(FileName, linesToWrite.ToArray());
        */
        /*
        System.IO.StreamWriter file = new System.IO.StreamWriter(FileName);
        for (int rowIndex = 0; rowIndex < Qdata.qdata.state_size; rowIndex++)
        {
            System.Text.StringBuilder line = new System.Text.StringBuilder();
            for (int colIndex = 0; colIndex < Qdata.qdata.action_size; colIndex++)
                line.Append(Qdata.qdata.q_table[rowIndex][colIndex]).Append("\t");
            file.WriteLine(line);
        }
        file.Close();
        */
    }
}