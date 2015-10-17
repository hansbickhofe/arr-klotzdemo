using UnityEngine;
using System.Collections;

public class Ship
{
	public int id;
	public float xPos;
	public float yPos;
	public int time;

	public Ship(int newID, float newXpos, float newYpos, int newTime){
		id = newID;
		xPos = newXpos;
		yPos = newYpos;
		time = newTime;
	}
}