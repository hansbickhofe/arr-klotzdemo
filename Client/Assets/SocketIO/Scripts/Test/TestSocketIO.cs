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
	
	int id;
	float xPos;
	float yPos;
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
			yPos = UnityEngine.Random.Range(-5.0f,5.0f);
			time = 0;

			Dictionary<string,string> json = new Dictionary<string, string>();
			json.Add("id",id.ToString());
			json.Add("xPos",xPos.ToString());
			json.Add("yPos",yPos.ToString());
			json.Add("time",time.ToString());

			socket.Emit("channelname",new JSONObject(json));

			print ("json send: "+json);
		}
	}
	
	public void receiveSocketData(SocketIOEvent e){
		Debug.Log("[SocketIO] data received: " + e.name + " " + e.data);
		JSONObject jo = e.data as JSONObject;
		print ("-> "+ jo["id"].str +" "+ jo["xPos"].str +" "+ jo["yPos"].str+" "+ jo["time"].str);

		id = int.Parse(jo["id"].str);
		xPos = float.Parse(jo["xPos"].str);
		yPos = float.Parse(jo["yPos"].str);
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
				//cube pos updaten
				print(id+": is already there!");
				idFound = true;
				break;
			}
		}
		
		// neue id eintragen
		if (!idFound){
			//cube erzeugen
			allShips.Add(new Ship(id, xPos, yPos, time));
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
				//cube löschen
				print(allShips[i].id+": Removed!");
				allShips.RemoveAt(i);
				break;
			} else {
				allShips[i].time++;
			}
		}
	}
}
