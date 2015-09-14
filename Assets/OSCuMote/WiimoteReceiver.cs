//----------------------------------------------------------------------
//		By. Jens Zeilund | zeilund at gmail dot com
//		www.sketchground.dk | www.itu.dk/people/jzso
//----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using OSC.NET;

public class WiimoteReceiver {
	
	//Singleton instance of WiimoteReceiver
	static readonly WiimoteReceiver instance = new WiimoteReceiver();

	private bool connected = false;
	//Standard port to recieve OSC messages
	private int port = 8876;
	private OSCReceiver receiver;
	private Thread thread;
	public Dictionary<int,Wiimote> wiimotes = new Dictionary<int, Wiimote>();
	
	WiimoteReceiver() {}
	
	WiimoteReceiver(int port)
	{
		this.port = port;
	}
	
	public int getPort()
	{
		return port;
	}
	
	
	// Connect on standard port.
	public void connect() {
		connect(port);
	}
	
	// Connect on custom port. Note: if already connected, this method will not do anything.
	public bool connect(int port) {
		if(connected == false)
		{
			try 
			{
				receiver = new OSCReceiver(port);
				thread = new Thread(new ThreadStart(listen));
				thread.Start();
				connected = true;
				return true;
			} 
			catch(Exception e)
			{
				Console.WriteLine("Failed to connect to port" + port);
				Console.WriteLine(e.Message);
			}
			return false;
			//oscReceiver.Connect();
		}
		return true;
	}
	
	// Returning the instance of this class.
	public static WiimoteReceiver Instance { get {return instance;}}
	
	// Disconnect ( Stop listening for OSC messages)
	public void disconnect()
	{
		if(receiver != null) receiver.Close();
		receiver = null;
		connected = false;
	}
	
	public bool isConnected() { return connected; }
	
	// Main loop of connection. Unpacks messages and assigns them to Wiimote objects.
	private void listen()
	{
		while(connected)
		{
			try
			{
				OSCPacket packet = receiver.Receive();
				if(packet != null)
				{
					if (packet.IsBundle()) {
						ArrayList messages = packet.Values;
						for (int i=0; i<messages.Count; i++) {
							processMessage((OSCMessage)messages[i]);
						}
					} else processMessage((OSCMessage)packet);		
				} else Console.WriteLine("Null packet");
			} catch (Exception e) { Console.WriteLine(e.Message); }
		}
	}
	
	private void processMessage(OSCMessage message)
	{
		
		string address = message.Address;
		int wiimoteID = int.Parse(address.Substring(5,1));
		//UnityEngine.Debug.Log (message.Address);
		// Is wii OSC message
		if( String.Compare(address.Substring(1,3), "wii") == 0)
		{
			// Does Wiimote object id already exist ?
			if( !wiimotes.ContainsKey(wiimoteID) )
			{
				wiimotes.Add(wiimoteID, new Wiimote(wiimoteID) );
			}
			// Update Wiimote Object
			try
			{
				Wiimote mote = wiimotes[wiimoteID];
				string wiiEvent = address.Substring(7);
				mote.update(wiiEvent, ArrayList.ReadOnly(message.Values), DateTime.Now );
				Console.WriteLine(wiiEvent);
			} catch(Exception e) 
			{
				Console.WriteLine( "Failed to get Wiimote Object from Dictionary" + e.Message);
			}
		}
	}
}
