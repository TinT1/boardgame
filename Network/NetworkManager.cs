using UnityEngine;
using Photon;
using System.Collections.Generic;
using System.Collections;

public class NetworkManager : Photon.PunBehaviour
{
    const string VERSION = "v4.2";
    public string roomName = "BoardGameRoom";

    private List<PhotonPlayer> players;
    private List<GameObject> spawnPlayers;

	void Start ()
    {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        players = new List<PhotonPlayer>();
        spawnPlayers = new List<GameObject>();

        spawnPlayers.Add(GameObject.Find("Row 0").transform.GetChild(0).gameObject);
        spawnPlayers.Add(GameObject.Find("Row 0").transform.GetChild(7).gameObject);
        spawnPlayers.Add(GameObject.Find("Row 7").transform.GetChild(0).gameObject);
        spawnPlayers.Add(GameObject.Find("Row 7").transform.GetChild(7).gameObject);
    }
    
    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4};
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        ArrangePlayer();
        foreach (PhotonPlayer player in PhotonNetwork.playerList)players.Add(player);
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        
    }

    public void ArrangePlayer()
    {

        for (int i = 1; i <= PhotonNetwork.room.playerCount; i++)
            if (PhotonNetwork.room.playerCount == i) PhotonNetwork.Instantiate(GameObject.Find("SavingPlayer").GetComponent<DontDestroyOnload>().playerName, spawnPlayers[i - 1].transform.position, Quaternion.identity, 0);
    }
        

}
