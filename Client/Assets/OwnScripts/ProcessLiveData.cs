using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessLiveData : MonoBehaviour {
	 
	List<Ship> allShips = new List<Ship>();
	int arraySize;
	public int maxItems = 10;
	public int lifeTime = 100;

	int id;
	float xPos;
	float yPos;
	int time;

	// Update is called once per frame
	void Update () {
		ProcessData();
		CleanupOldData();
	}

	void ProcessData(){
		bool idFound = false;

		//create random values
		id = Random.Range(0,maxItems);
		xPos = Random.Range(-5.0f,5.0f);
		yPos = Random.Range(-5.0f,5.0f);
		time = 0;

		// check if id already exists
		// better way to search array ?!?!
		arraySize = allShips.Count;
		for (int i = 0; i<arraySize; i++){
			if ((int)allShips[i].id == id) {
				//print(id+": is already there!");
				idFound = true;
				break;
			}
		}
	
		// neue id eintragen
		if (!idFound && arraySize < maxItems){
			allShips.Add(new Ship(id, xPos, yPos, time));
			//print (id+": added!");
			arraySize++;

			// check array for debug
//			string listPosString = "";
//
//			for (int i = 0; i<arraySize; i++){
//				//listString += allShips[i].id+", ";
//				listPosString += allShips[i].pos+", ";
//			}

			//print(arraySize+" : "+listPosString);
		}
	}

	void CleanupOldData(){
		arraySize = allShips.Count;

		for (int i = 0; i<arraySize; i++){
			if (allShips[i].time > lifeTime) {
				allShips.RemoveAt(i);
				//print(allShips[i].id+": Removed!");
				break;
			} else {
				allShips[i].time++;
			}
		}
	}
}
