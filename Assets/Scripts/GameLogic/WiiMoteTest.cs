using UnityEngine;
using System.Collections;

public class WiiMoteTest : MonoBehaviour {

	private WiimoteReceiver receiver;
	private Quaternion initPosition;
	public Texture2D cursorOne;
	// Use this for initialization
	void Start () {
		receiver = WiimoteReceiver.Instance;
		receiver.connect();
		initPosition = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(receiver.wiimotes.ContainsKey(1))
		{
			Wiimote mymote = receiver.wiimotes[1];
			transform.rotation = initPosition;
			//float zTilt = ((mymote.BALANCE_TOPRIGHT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_BOTTOMLEFT)) *20;
			//float xTilt = ((mymote.BALANCE_BOTTOMLEFT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_TOPRIGHT)) *20;
			transform.Rotate(-mymote.MOTION_ANGLE_PITCH,(mymote.BUTTON_A*50 - mymote.BUTTON_B*50),-mymote.MOTION_ANGLE_ROLL);
		}
	}

	void OnDestroy() {
		receiver.disconnect ();
	}
	
	void OnGUI()
	{
		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			GUI.BeginGroup(new Rect(10,10, 150,330));
				GUI.Box(new Rect(0,0,110,500), "Wiimote 1:");
				GUI.Label(new Rect(5,20,110,20), "Button A: " + mymote.BUTTON_A.ToString() );
				GUI.Label(new Rect(5,35,110,20), "Button B: " + mymote.BUTTON_B.ToString());
				GUI.Label(new Rect(5,50,110,20), "Button Left: " + mymote.BUTTON_LEFT.ToString());
				GUI.Label(new Rect(5,65,110,20), "Button Right: " + mymote.BUTTON_RIGHT.ToString());
				GUI.Label(new Rect(5,80,110,20), "Button Up: " + mymote.BUTTON_UP.ToString());
				GUI.Label(new Rect(5,95,110,20), "Button Down: " + mymote.BUTTON_DOWN.ToString());
				GUI.Label(new Rect(5,110,110,20), "Button 1: " + mymote.BUTTON_ONE.ToString());
				GUI.Label(new Rect(5,125,110,20), "Button 2: " + mymote.BUTTON_TWO.ToString());
				GUI.Label(new Rect(5,140,110,20), "Button Plus: " + mymote.BUTTON_PLUS.ToString());
				GUI.Label(new Rect(5,155,110,20), "Button Minus: " + mymote.BUTTON_MINUS.ToString());
				
				GUI.Label(new Rect(5,170,110,20), "Pitch: " + mymote.PRY_PITCH.ToString());
				GUI.Label(new Rect(5,185,110,20), "Roll: " + mymote.PRY_ROLL.ToString());
				GUI.Label(new Rect(5,200,110,20), "Yaw: " + mymote.PRY_YAW.ToString());
				GUI.Label(new Rect(5,215,110,20), "Accel: " + mymote.PRY_ACCEL.ToString());
				/*
				GUI.Label(new Rect(5,230,110,20), "Balance Board:");
				GUI.Label(new Rect(5,245,150,20), "Bot Left" + mymote.BALANCE_BOTTOMLEFT.ToString());
				GUI.Label(new Rect(5,260,150,20), "Bot Right" + mymote.BALANCE_BOTTOMRIGHT.ToString());
				GUI.Label(new Rect(5,275,150,20), "Top Left" + mymote.BALANCE_TOPLEFT.ToString());
				GUI.Label(new Rect(5,290,150,20), "Top Right" + mymote.BALANCE_TOPRIGHT.ToString());
				GUI.Label(new Rect(5,305,150,20), "Sum" + mymote.BALANCE_SUM.ToString());
				*/

	
				GUI.Label(new Rect(5,230,110,20), "JOY_X: " + mymote.NUNCHUK_JOY_X.ToString());
				GUI.Label(new Rect(5,245,110,20), "JOY_Y: " + mymote.NUNCHUK_JOY_Y.ToString());
			GUI.EndGroup();
			
			// GUI.DrawTexture(new Rect(mymote.IR_X * Screen.width, (1-mymote.IR_Y)* Screen.height,32,32), cursorOne);
		}
		if(receiver.wiimotes.ContainsKey(2) )
		{
			Wiimote mymote = receiver.wiimotes[2];
			GUI.BeginGroup(new Rect(170,10, 200,330));
				GUI.Box(new Rect(0,0,200,500), "Wiimote 2:");
				GUI.Label(new Rect(5,20,110,20), "Button A: " + mymote.BUTTON_A.ToString() );
				GUI.Label(new Rect(5,35,110,20), "Balance Board:");
				GUI.Label(new Rect(5,50,200,20), "Bot Left" + mymote.BALANCE_BOTTOMLEFT.ToString());
				GUI.Label(new Rect(5,65,200,20), "Bot Right" + mymote.BALANCE_BOTTOMRIGHT.ToString());
				GUI.Label(new Rect(5,80,200,20), "Top Left" + mymote.BALANCE_TOPLEFT.ToString());
				GUI.Label(new Rect(5,95,200,20), "Top Right" + mymote.BALANCE_TOPRIGHT.ToString());
				GUI.Label(new Rect(5,110,200,20), "Sum" + mymote.BALANCE_SUM.ToString());
			GUI.EndGroup();
			if(mymote.BALANCE_SUM > 0.2f)
			{
				float myX = (mymote.BALANCE_TOPRIGHT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_BOTTOMLEFT);
				float myY = (mymote.BALANCE_BOTTOMLEFT + mymote.BALANCE_BOTTOMRIGHT)-(mymote.BALANCE_TOPLEFT + mymote.BALANCE_TOPRIGHT);
				GUI.BeginGroup(new Rect(190,210,100,100));
					GUI.Box(new Rect(0,0,100,100),"");
					GUI.Label(new Rect( (myX*100)+50,(myY*100)+50,20,20),"*");
				GUI.EndGroup();
			}
		}
		if(receiver.wiimotes.ContainsKey(3))
		{
			Wiimote mymote = receiver.wiimotes[3];
			GUI.DrawTexture(new Rect(mymote.IR_X * Screen.width, (1-mymote.IR_Y)* Screen.height,32,32), cursorOne);
		}
/*		
		if(receiver.wiimotes.ContainsKey(1) )
		{	
			Wiimote mymote = receiver.wiimotes[1];
			GUI.Label(new Rect(0,10,100,20), mymote.PRY_ACCEL.ToString() );
			GUI.Label(new Rect(mymote.IR_X * Screen.width, (1.0f - mymote.IR_Y) * Screen.height, 20,20), "C1" );
		}
		GUI.Label(new Rect(0,40,100,20), receiver.test2.ToString());
		GUI.Label(new Rect(0,60,100,20), receiver.test);
		if(receiver.wiimotes.ContainsKey(2) )
			GUI.Label(new Rect(0,100,100,20), receiver.wiimotes[2].BUTTON_A.ToString() );
*/
	}
}
