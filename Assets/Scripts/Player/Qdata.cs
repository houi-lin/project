using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Qdata : MonoBehaviour
{
    public static Qdata qdata;
    public float[][] q_table;
    public int state_size = 500 * 500;
    public int action_size = 9 * 360;
    private bool first = true;
    public int iteration = 0;

    // Start is called before the first frame update
    void Awake()
    {
        /*
        Vector3 pos_new = Quaternion.AngleAxis(-45, Vector3.up) * new Vector3(0f, 0f, 0f);
        pos_new = pos_new + new Vector3(25f, 0, 25f);
        pos_new *= 10;
        int s = (int)pos_new.x * 500 + (int)pos_new.z;
        */
        //print("Qdata start!");
        //if (qdata == null) qdata = this;
        if (qdata == null)
        {
            qdata = this;
            if (first) initQtable();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void initQtable()
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
}
