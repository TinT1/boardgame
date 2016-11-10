using UnityEngine;
using System.Collections;

public class ImBlueDaBuDiBaDiDaj : MonoBehaviour {
    private bool play = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("SavingPlayer").GetComponent<DontDestroyOnload>().playerName == "blue" && play == false)
        {
            play = true;
            Camera.main.GetComponent<AudioSource>().enabled = true;
        }
    }
}
