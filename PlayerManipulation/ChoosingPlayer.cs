using UnityEngine;
using System.Collections;

public class ChoosingPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
      
    }

    void OnMouseDown()
    {
        GameObject.Find("SavingPlayer").GetComponent<DontDestroyOnload>().playerName = gameObject.name;
        Application.LoadLevel("InGame");
        
    }

    
}
