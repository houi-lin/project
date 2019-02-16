using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Qdata : MonoBehaviour
{
    public static float[][] q_table;
    public static int state_size = 500 * 500;
    public static int action_size = 9 * 360;
    private static bool first = true;
    public static int iteration = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (first) initQtable();
        DontDestroyOnLoad(this);
    }

    public static void initQtable()
    {
        first = false;

        //Initial Q table
        q_table = new float[state_size][];
        for (int i = 0; i < state_size; i++)
        {
            q_table[i] = new float[action_size];
            for (int j = 0; j < action_size; j++)
            {
                q_table[i][j] = 0.0f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
