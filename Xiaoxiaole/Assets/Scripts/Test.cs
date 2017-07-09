using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        /*
        Debug.Log("test begin");
        StartCoroutine("TestCoroutine");
        for (int i = 0; i < 1000; i++)
            Debug.Log("b:" + i);
        Debug.Log(Time.time);
        */
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < 1000; i++)
        //    Debug.Log("e:" + i);
	}

    IEnumerator TestCoroutine()
    {
        for (int i = 0; i < 100; i++)
            Debug.Log("a:" + i);
        Debug.Log(Time.time);
        yield return new WaitForSeconds(0.001f);
        Debug.Log(Time.time);
        for (int i = 0; i < 100; i++)
            Debug.Log("c:" + i);
    }

    private void OnMouseDown()
    {
        Debug.Log("mouse down");
    }
}
