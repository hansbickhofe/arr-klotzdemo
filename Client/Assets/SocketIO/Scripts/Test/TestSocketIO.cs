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
	public int lifeTime;
	float timer;
	public float sendDataTime;
	public int speed;

	public GameObject ship;
	int id;
	float xPos;
	float zPos;
	int shipTime;
	
	public void Start() {
		id = UnityEngine.Random.Range(0,100000);
		xPos = UnityEngine.Random.Range(-5.0f,5.0f);
		zPos = UnityEngine.Random.Range(-5.0f,5.0f);
		shipTime = 0;

		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		socket.On ("channelname",receiveSocketData);
	}

	public void Update(){
		xPos += Input.GetAxis ("Horizontal") * speed;
		zPos += Input.GetAxis ("Vertical") * speed;

		//timer
		timer += Time.deltaTime;
		if (timer > sendDataTime) {
			SendJsonData();
			timer = 0;
		}
	}

	public void SendJsonData(){
		Dictionary<string,string> json = new Dictionary<string, string>();
		json.Add("id",id.ToString());
		json.Add("xPos",xPos.ToString());
		json.Add("yPos",zPos.ToString());
		json.Add("time",shipTime.ToString());
		
		socket.Emit("channelname",new JSONObject(json));
		
		print ("json send: "+json);
	}
	
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		//print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);

		id = int.Parse(jo["id"].str);
		xPos = float.Parse(jo["xPos"].str);
		zPos = float.Parse(jo["yPos"].str);
		shipTime = int.Parse(jo["time"].str);
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
				allShips[i].time = shipTime;
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

			// neue daten ins array schreiben
			allShips.Add(new Ship(newShip, id, xPos, zPos, shipTime));
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
