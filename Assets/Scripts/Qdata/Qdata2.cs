using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Qdata2 : MonoBehaviour
{
    public static Qdata2 qdata;
    public static float[][] q_table;
    public static int state_size = 500 * 500;
    public static int action_size = 9 * 360;
    private static bool first = true;
    public static int iteration = 0;
    // Start is called before the first frame update
    void Start()
    {
        /*
        Vector3 pos_new = Quaternion.AngleAxis(-45, Vector3.up) * new Vector3(0f, 0f, 0f);
        pos_new = pos_new + new Vector3(25f, 0, 25f);
        pos_new *= 10;
        int s = (int)pos_new.x * 500 + (int)pos_new.z;
        */
        //print("Qdata start!");
        //if (qdata == null) qdata = this;
        if (first) initQtable();
        DontDestroyOnLoad(this);
    }

    public static void initQtable()
    {
        print("initQtable");
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
