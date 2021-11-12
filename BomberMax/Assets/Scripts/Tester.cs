using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{

    private void Start()
    {
        Test();
    }

    public void Test()
    {
        GameObject test = new GameObject("Test");
        Instantiate(test);
    }
}
