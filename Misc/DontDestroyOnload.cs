using UnityEngine;
using System.Collections;

public class DontDestroyOnload : MonoBehaviour {
    public string playerName;
	// Use this for initialization
	void Start () {
	
	}
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
	// Update is called once per frame
	void Update () {
	    
	}
}
