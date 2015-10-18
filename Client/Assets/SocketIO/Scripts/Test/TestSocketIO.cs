using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class TestSocketIO : MonoBehaviour
{
	private SocketIOComponent socket;

	//process data
	List<Ship> allShips = new List<Ship>();
	int arraySize;
	public int lifeTime = 100;
	
	public GameObject ship;
	int id;
	float xPos;
	float zPos;
	int time;
	
	public void Start() {
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On ("channelname",receiveSocketData);
	}

	public void Update(){
		if (Input.GetMouseButtonDown (0)){

			//create random values
			id = UnityEngine.Random.Range(0,10);
			xPos = UnityEngine.Random.Range(-5.0f,5.0f);
			zPos = UnityEngine.Random.Range(-5.0f,5.0f);
			time = 0;

			Dictionary<string,string> json = new Dictionary<string, string>();
			json.Add("id",id.ToString());
			json.Add("xPos",xPos.ToString());
			json.Add("yPos",zPos.ToString());
			json.Add("time",time.ToString());

			socket.Emit("channelname",new JSONObject(json));

			print ("json send: "+json);
		}
	}
	
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);

		id = int.Parse(jo["id"].str);
		xPos = float.Parse(jo["xPos"].str);
		zPos = float.Parse(jo["yPos"].str);
		time = int.Parse(jo["time"].str);
		ProcessData();
		CleanupOldData();
	}

	void ProcessData(){
		bool idFound = false;
		
		// check if id already exists
		arraySize = allShips.Count;
		for (int i = 0; i<arraySize; i++){
			if ((int)allShips[i].id == id) {
				//existing ship pos updaten
				allShips[i].xPos = xPos;
				allShips[i].zPos = zPos;
				allShips[i].ship.transform.position = new Vector3(xPos,0.5f,zPos);
				print(id+": is already there!");
				idFound = true;
				break;
			}
		}
		
		// neue id eintragen
		if (!idFound){
			//ship an random pos mit random color erzeugen
			GameObject newShip;
			Vector3 spawnPosition = new Vector3(xPos,2,zPos);
			newShip = Instantiate(ship, spawnPosition, transform.rotation) as GameObject;
			Color randomColor = new Color (UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f),UnityEngine.Random.Range(0.0f,1.0f));
			newShip.GetComponent<Renderer>().material.SetColor("_Color", randomColor);

			allShips.Add(new Ship(newShip, id, xPos, zPos, time));
			print (id+": added!");
			arraySize++;
		}

		//check array for debug
		string listString = "";
		
		for (int i = 0; i<arraySize; i++){
			listString += allShips[i].id+", ";
		}
		
		print(arraySize+" : "+listString);
	}
	
	void CleanupOldData(){
		arraySize = allShips.Count;
		
		for (int i = 0; i<arraySize; i++){
			if (allShips[i].time > lifeTime) {
				//ship object und array eintrag löschen
				Destroy(allShips[i].ship);
				print(allShips[i].id+": Removed!");
				allShips.RemoveAt(i);
				break;
			} else {
				allShips[i].time++;
			}
		}
	}

	void CreateShip(){

	}
}
