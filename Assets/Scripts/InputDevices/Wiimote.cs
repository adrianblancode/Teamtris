//----------------------------------------------------------------------
//		By. Jens Zeilund | zeilund at gmail dot com
//		www.sketchground.dk | www.itu.dk/people/jzso
//----------------------------------------------------------------------
using System;
using System.Collections;

public class Wiimote : Controller {
	
	private int id;
	private DateTime lastUpdate;
	
	// Wiimote Buttons:
	private float m_buttonA;
	private float m_buttonB;
	private float m_buttonLeft;
	private float m_buttonRight;
	private float m_buttonUp;
	private float m_buttonDown;
	private float m_buttonOne;
	private float m_buttonTwo;
	private float m_buttonPlus;
	private float m_buttonMinus;
	private float m_buttonHome;
	
	// Wiimote Analog:
	private float m_pryPitch;
	private float m_pryRoll;
	private float m_pryYaw;
	private float m_pryAccel;
	private float m_irX;
	private float m_irY;
	private float m_irSize;
	
	private float m_accX;
	private float m_accY;
	private float m_accZ;

	private float m_ir1RawX;
	private float m_ir1RawY;
	private float m_ir1RawSize;
	private float m_ir2RawX;
	private float m_ir2RawY;
	private float m_ir2RawSize;
	private float m_ir3RawX;
	private float m_ir3RawY;
	private float m_ir3RawSize;
	private float m_ir4RawX;
	private float m_ir4RawY;
	private float m_ir4RawSize;
	
	//Extensions:
	// Nunchuk:
	private float m_nunchukC;
	private float m_nunchukZ;
	private float m_nunchuckPitch;
	private float m_nunchukRoll;
	private float m_nunchukYaw;
	private float m_nunchukAccel;
	private float m_nunchukJoyX;
	private float m_nunchukJoyY;
	
	// Balance Board:
	private float m_balanceBL;
	private float m_balanceBR;
	private float m_balanceTL;
	private float m_balanceTR;
	private float m_balanceSum;

	// Wii Motion plus:
	private float m_motionAnglePitch;
	private float m_motionAngleRoll;
	private float m_motionAngleYaw;
	
	private float m_motionVeloPitch;
	private float m_motionVeloRoll;
	private float m_motionVeloYaw;
	
	// Public getters
	public float BUTTON_A {get {return m_buttonA; } }
	public float BUTTON_B {get {return m_buttonB; } }
	public float BUTTON_LEFT {get {return m_buttonLeft; } }
	public float BUTTON_RIGHT {get {return m_buttonRight; } }
	public float BUTTON_UP {get {return m_buttonUp; } }
	public float BUTTON_DOWN {get {return m_buttonDown; } }
	public float BUTTON_ONE {get {return m_buttonOne; } }
	public float BUTTON_TWO {get {return m_buttonTwo; } }
	public float BUTTON_PLUS {get {return m_buttonPlus; } }
	public float BUTTON_MINUS {get {return m_buttonMinus; } }
	public float BUTTON_HOME {get {return m_buttonHome; } }
	
	public float PRY_PITCH {get {return m_pryPitch; } }
	public float PRY_ROLL {get {return m_pryRoll; } }
	public float PRY_YAW {get {return m_pryYaw; } }
	public float PRY_ACCEL {get {return m_pryAccel; } }
	public float IR_X {get {return m_irX;}}
	public float IR_Y {get {return m_irY;}}
	public float IR_SIZE {get {return m_irSize;}}
	
	public float ACC_X {get {return m_accX;}}
	public float ACC_Y {get {return m_accY;}}
	public float ACC_Z {get {return m_accZ;}}

	public float IR_RAW1X {get {return m_ir1RawX;}}
	public float IR_RAW1Y {get {return m_ir1RawY;}}
	public float IR_RAW1SIZE {get {return m_ir1RawSize;}}

	public float IR_RAW2X {get {return m_ir2RawX;}}
	public float IR_RAW2Y {get {return m_ir2RawY;}}
	public float IR_RAW2SIZE {get {return m_ir2RawSize;}}

	public float IR_RAW3X {get {return m_ir3RawX;}}
	public float IR_RAW3Y {get {return m_ir3RawY;}}
	public float IR_RAW3SIZE {get {return m_ir3RawSize;}}

	public float IR_RAW4X {get {return m_ir4RawX;}}
	public float IR_RAW4Y {get {return m_ir4RawY;}}
	public float IR_RAW4SIZE {get {return m_ir4RawSize;}}
	
	public float NUNCHUK_C {get {return m_nunchukC;}}
	public float NUNCHUK_Z {get {return m_nunchukZ;}}
	public float NUNCHUK_PITCH {get {return m_nunchuckPitch;}}
	public float NUNCHUK_ROLL {get {return m_nunchukRoll;}}
	public float NUNCHUK_YAW {get {return m_nunchukYaw;}}
	public float NUNCHUK_ACCEL {get {return m_nunchukAccel;}}
	public float NUNCHUK_JOY_X {get {return m_nunchukJoyX;}}
	public float NUNCHUK_JOY_Y {get {return m_nunchukJoyY;}}
	
	public float BALANCE_BOTTOMLEFT {get {return m_balanceBL;}}
	public float BALANCE_BOTTOMRIGHT {get {return m_balanceBR;}}
	public float BALANCE_TOPLEFT {get {return m_balanceTL;}}
	public float BALANCE_TOPRIGHT {get {return m_balanceTR;}}
	public float BALANCE_SUM {get {return m_balanceSum;}}
	
	public float MOTION_ANGLE_PITCH {get {return m_motionAnglePitch;}}
	public float MOTION_ANGLE_ROLL {get {return m_motionAngleRoll;}}
	public float MOTION_ANGLE_YAW {get {return m_motionAngleYaw;}}
	
	public float MOTION_VELO_PITCH {get {return m_motionVeloPitch;}}
	public float MOTION_VELO_ROLL {get {return m_motionVeloRoll;}}
	public float MOTION_VELO_YAW {get {return m_motionVeloYaw;}}
	
	public DateTime LAST_UPDATE {get {return lastUpdate;}}
	public int WIIMOTE_ID {get {return this.id;}}
	
	
	public Wiimote() {}
	
	public Wiimote(int id)
	{
		this.id = id;
	}

	public bool MoveRight() {
		return BUTTON_RIGHT == 1 ? true : false;
	}
	public bool MoveLeft() {
		return BUTTON_LEFT == 1 ? true : false;
	}
	public bool MoveUp() {
		return BUTTON_UP == 1 ? true : false;
	}
	public bool MoveDown() {
		return BUTTON_DOWN == 1 ? true : false;
	}

	public float RotateRight() {
		return BUTTON_A;
	}
	public float RotateLeft() {
		return BUTTON_B;
	}
	
	public bool Confirm() {
		return BUTTON_A == 1 ? true : false;
	}
	public bool Reject() {
		return BUTTON_B == 1 ? true : false;
	}
	public bool Pause() {
		return BUTTON_PLUS == 1 ? true : false;
	}
	public bool Quit() {
		return BUTTON_HOME == 1 ? true : false;
	}
	
	public void update(string oscMessage, ArrayList values, DateTime currentTime)
	{
		lastUpdate = currentTime;
		// Analog Wiimote
		if(oscMessage == "accel/pry")
		{
			m_pryPitch = (float)values[0];
			m_pryRoll = (float)values[1];
			m_pryYaw = (float)values[2];
			m_pryAccel = (float)values[3];
		}
		else if(oscMessage == "accel/xyz") {
			m_accX = (float)values[0];
			m_accY = (float)values[1];
			m_accZ = (float)values[2];
		}
		else if(oscMessage == "ir")
		{
			m_irX = (float)values[0];
			m_irY = (float)values[1];
			m_irSize = (float)values[2];
		}
		else if(oscMessage == "ir/xys/1") {
			m_ir1RawX = (float)values[0];
			m_ir1RawY = (float)values[1];
			m_ir1RawSize = (float)values[2];
		}
		else if(oscMessage == "ir/xys/2") {
			m_ir2RawX = (float)values[0];
			m_ir2RawY = (float)values[1];
			m_ir2RawSize = (float)values[2];
		}
		else if(oscMessage == "ir/xys/3") {
			m_ir3RawX = (float)values[0];
			m_ir3RawY = (float)values[1];
			m_ir3RawSize = (float)values[2];
		}
		else if(oscMessage == "ir/xys/4") {
			m_ir4RawX = (float)values[0];
			m_ir4RawY = (float)values[1];
			m_ir4RawSize = (float)values[2];
		}
		// Wiimote Buttons
		else if(oscMessage == "button/A")
		{
			m_buttonA = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/B")
		{
			m_buttonB = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Left")
		{
			m_buttonLeft = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Right")
		{
			m_buttonRight = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Up")
		{
			m_buttonUp = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Down")
		{
			m_buttonDown = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Plus")
		{
			m_buttonPlus = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Minus")
		{
			m_buttonMinus = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/1")
		{
			m_buttonOne = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/2")
		{
			m_buttonTwo = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "button/Home")
		{
			m_buttonHome = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "motion/angles")
		{
			m_motionAnglePitch = (float)values[0];
			m_motionAngleRoll = (float)values[1];
			m_motionAngleYaw = (float)values[2];
		}
		else if(oscMessage == "motion/velo")
		{
			m_motionVeloPitch = (float)values[0];
			m_motionVeloRoll = (float)values[1];
			m_motionVeloYaw = (float)values[2];
		}
		// Balance Board analog
		else if(oscMessage == "balance")
		{
			m_balanceBL = (float)values[0];
			m_balanceBR = (float)values[1];
			m_balanceTL = (float)values[2];
			m_balanceTR = (float)values[3];
			m_balanceSum = (float)values[4];
		}
		//Nunchuk
		else if(oscMessage == "nunchuk/button/C")
		{
			m_nunchukC = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "nunchuk/button/Z")
		{
			m_nunchukZ = Convert.ToSingle(values[0]);
		}
		else if(oscMessage == "nunchuk/joy") 
		{
			m_nunchukJoyX = (float)values[0];
			m_nunchukJoyY = (float)values[1];
		}
		else if(oscMessage == "nunchuk/accel/pry")
		{
			m_nunchuckPitch = (float)values[0];
			m_nunchukRoll = (float)values[1];
			m_nunchukYaw = (float)values[2];
			m_nunchukAccel = (float)values[3];
		}
		else{}
	}
}
/*

OSCulator - OSC messages. (All data between 0 and 1 float)

Classic Controller:

/wii/x/classic/joyl
	0 = x
	1 = y
/wii/x/classic/joyr
	0 = x
	1 = y
/wii/x/classic/analog/L
/wii/x/classic/analog/R

/wii/x/classic/button/L
/wii/x/classic/button/R
/wii/x/classic/button/Left
/wii/x/classic/button/Right
/wii/x/classic/button/Up
/wii/x/classic/button/Down

/wii/x/classic/button/Minus
/wii/x/classic/button/Plus
/wii/x/classic/button/Home
/wii/x/classic/button/A
/wii/x/classic/button/B
/wii/x/classic/button/X
/wii/x/classic/button/Y
/wii/x/classic/button/ZL
/wii/x/classic/button/ZR

Nunchuk:

/wii/x/nunchuk/button/C
/wii/x/nunchuk/button/Z
/wii/x/nunchuk/joy
	0 = x
	1 = y
/wii/x/nunchuk/accel/pry
	0 = pitch
	1 = roll
	2 = yaw
	3 = accel
/wii/x/nunchuk/accel/xyz
	0 = x
	1 = y
	2 = z

Wiimote:

/wii/x/accel/xyz
	0 = x
	1 = y
	2 = z

/wii/x/accel/pry
	0 = pitch
	1 = roll
	2 = yaw
	3 = accel

/wii/x/button/A
/wii/x/button/B
/wii/x/button/Left
/wii/x/button/Right
/wii/x/button/Up
/wii/x/button/Down
/wii/x/button/Minus
/wii/x/button/Plus
/wii/x/button/1
/wii/x/button/2

/wii/x/ir
	0 = x
	1 = y

Balance Board:

Button a is sent as a normal wiimote event.
/wii/x/balance
	0 = bottom left
	1 = bottom right
	2 = top left
	3 = top right
	4 = sum
*/
