/*
	TUIO C# Library - part of the reacTIVision project
	http://reactivision.sourceforge.net/

	Copyright (c) 2005-2009 Martin Kaltenbrunner <mkalten@iua.upf.edu>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using OSC.NET;

public class jsTuioClient {}

namespace TUIO
{
	/**
	 * The TuioClient class is the central TUIO protocol decoder component. It provides a simple callback infrastructure using the {@link TuioListener} interface.
	 * In order to receive and decode TUIO messages an instance of TuioClient needs to be created. The TuioClient instance then generates TUIO events
	 * which are broadcasted to all registered classes that implement the {@link TuioListener} interface.<P> 
	 * <code>
	 * TuioClient client = new TuioClient();<br/>
	 * client.addTuioListener(myTuioListener);<br/>
	 * client.start();<br/>
	 * </code>
	 *
	 * @author Martin Kaltenbrunner
	 * @version 1.4
	 */ 
	public class TuioClient 
	{
		private bool connected = false;
		private int port = 3333;
		private OSCReceiver receiver;
		private Thread thread;
		
		private Dictionary<long,TuioObject> objectList = new Dictionary<long,TuioObject>(32);
		private List<long> aliveObjectList = new List<long>(32);
		private List<long> newObjectList = new List<long>(32);
		private Dictionary<long,TuioCursor> cursorList = new Dictionary<long,TuioCursor>(32);
		private List<long> aliveCursorList = new List<long>(32);
		private List<long> newCursorList = new List<long>(32);
		private List<TuioObject> frameObjects = new List<TuioObject>(32);
		private List<TuioCursor> frameCursors = new List<TuioCursor>(32);
			
		private List<TuioCursor> freeCursorList = new List<TuioCursor>();
		private int maxFingerID = -1;

		private int currentFrame = 0;
		private TuioTime currentTime;
				
		private List<TuioListener> listenerList = new List<TuioListener>();
		
		/**
		 * The default constructor creates a client that listens to the default TUIO port 3333
		 */
		public TuioClient() {}

		/**
		 * This constructor creates a client that listens to the provided port
		 *
		 * @param  port  the listening port number
		 */
		public TuioClient(int port) {
			this.port = port;
		}
	
		/**
		 * Returns the port number listening to.
		 *
		 * @return  the listening port number
		 */
		public int getPort() {
			return port;
		}
				
		/**
		 * The TuioClient starts listening to TUIO messages on the configured UDP port
		 * All reveived TUIO messages are decoded and the resulting TUIO events are broadcasted to all registered TuioListeners
		 */
		public void connect() {

			TuioTime.initSession();
			currentTime = new TuioTime();
            currentTime.reset();
			
			try {
				receiver = new OSCReceiver(port);
				thread = new Thread(new ThreadStart(listen));
				thread.Start();
				connected = true;
			} catch (Exception e) {
				Console.WriteLine("failed to connect to port "+port);
				Console.WriteLine(e.Message);
			}
		}
		
		/**
		 * The TuioClient stops listening to TUIO messages on the configured UDP port
		 */
		public void disconnect() {
        		if (receiver!=null) receiver.Close();
	        	receiver = null;
				connected = false;
		}

		/**
		 * Returns true if this TuioClient is currently connected.
		 * @return	true if this TuioClient is currently connected
		 */
		public bool isConnected() { return connected; }

		private void listen() {
			while(connected) {
				try {
					OSCPacket packet = receiver.Receive();
					if (packet!=null) {
						if (packet.IsBundle()) {
							ArrayList messages = packet.Values;
							for (int i=0; i<messages.Count; i++) {
								processMessage((OSCMessage)messages[i]);
							}
						} else processMessage((OSCMessage)packet);						
					} else Console.WriteLine("null packet");
				} catch (Exception e) { Console.WriteLine(e.Message); }
			}
		}



		/**
		 * The OSC callback method where all TUIO messages are received and decoded
		 * and where the TUIO event callbacks are dispatched
		 *
		 * @param  message	the received OSC message
		 */
		private void processMessage(OSCMessage message) {
			string address = message.Address;
			ArrayList args = message.Values;
			string command = (string)args[0];

			if (address == "/tuio/2Dobj") {
				if (command == "set") {

					if (currentTime.getTotalMilliseconds()==0)
						currentTime = TuioTime.getSessionTime();

					long s_id  = (int)args[1];
					int f_id  = (int)args[2];
					float x = (float)args[3];
					float y = (float)args[4];
					float a = (float)args[5];
					float X = (float)args[6];
					float Y = (float)args[7];
					float A = (float)args[8];
					float m = (float)args[9];
					float r = (float)args[10];

					
					if (!objectList.ContainsKey(s_id)) {
						TuioObject addObject  = new TuioObject(currentTime,s_id,f_id,x,y,a);		
						objectList.Add(s_id, addObject);

						for (int i=0;i<listenerList.Count;i++) {
							TuioListener listener = (TuioListener)listenerList[i];
							if (listener!=null) listener.addTuioObject(addObject);
						}
					} else {
						TuioObject updateObject = objectList[s_id];

						if((updateObject.getX()!=x) || (updateObject.getY()!=y) || (updateObject.getAngle()!=a)) {
							
							TuioObject tobj = new TuioObject(currentTime,s_id,updateObject.getSymbolID(),x,y,a);
							tobj.update(currentTime,x,y,a,X,Y,A,m,r);
							frameObjects.Add(tobj);
							/*updateObject.update(currentTime,x,y,a,X,Y,A,m,r);
							for (int i=0;i<listenerList.Count;i++) {
								TuioListener listener = (TuioListener)listenerList[i];
								if (listener!=null) listener.updateTuioObject(updateObject);
							}*/
							//objectList[s_id] = tobj;
						}
					}
							
				} else if (command == "alive") {
		
					for (int i=1;i<args.Count;i++) {
						// get the message content
						long s_id = (int)args[i];
						newObjectList.Add(s_id);
						// reduce the object list to the lost objects
						if (aliveObjectList.Contains(s_id))
							 aliveObjectList.Remove(s_id);
					}
					
					// remove the remaining objects
					for (int i=0;i<aliveObjectList.Count;i++) {
						long s_id = aliveObjectList[i];
						TuioObject removeObject = objectList[s_id];
						removeObject.remove(currentTime);
						objectList.Remove(s_id);
						

						for (int j=0;j<listenerList.Count;j++) {
							TuioListener listener = (TuioListener)listenerList[j];
							if (listener!=null) listener.removeTuioObject(removeObject);
						}
					}

					List<long> buffer = aliveObjectList;
					aliveObjectList = newObjectList;
					
					// recycling of the List
					newObjectList = buffer;
					newObjectList.Clear();
						
				} else if (command=="fseq") {
					int fseq = (int)args[1];
					bool lateFrame = false;
					
					if (fseq>0) {
						if ((fseq>=currentFrame) || ((currentFrame-fseq)>100)) currentFrame = fseq;
						else lateFrame = true;
					}
					
					if (!lateFrame) {
						
						IEnumerator<TuioObject> frameEnum = frameObjects.GetEnumerator();
						while(frameEnum.MoveNext()) {
							TuioObject tobj = frameEnum.Current;
							TuioObject updateObject = getTuioObject(tobj.getSessionID());
							updateObject.update(currentTime,tobj.getX(),tobj.getY(),tobj.getAngle(),tobj.getXSpeed(),tobj.getYSpeed(),tobj.getRotationSpeed(),tobj.getMotionAccel(),tobj.getRotationAccel());
			
							for (int i=0;i<listenerList.Count;i++) {
								TuioListener listener = (TuioListener)listenerList[i];
								if (listener!=null) listener.updateTuioObject(updateObject);
							}
						}
						
						for (int i=0;i<listenerList.Count;i++) {
							TuioListener listener = (TuioListener)listenerList[i];
							if (listener!=null) listener.refresh(currentTime);
						}					
						if (fseq>0) currentTime.reset();
					}
					frameObjects.Clear();
				}

			} else if (address == "/tuio/2Dcur") {

				if (command == "set") {
					
					if (currentTime.getTotalMilliseconds()==0)
						currentTime = TuioTime.getSessionTime();

					long s_id  = (int)args[1];
					float x = (float)args[2];
					float y = (float)args[3];
					float X = (float)args[4];
					float Y = (float)args[5];
					float m = (float)args[6];
					
					if (!cursorList.ContainsKey(s_id)) {
						
						int f_id = cursorList.Count;
						if (cursorList.Count<=maxFingerID) {
							TuioCursor closestCursor = freeCursorList[0];
							IEnumerator<TuioCursor> testList = freeCursorList.GetEnumerator();
							while(testList.MoveNext()) {
								TuioCursor testCursor = testList.Current;
								if (testCursor.getDistance(x,y)<closestCursor.getDistance(x,y)) closestCursor = testCursor;
							}
							f_id = closestCursor.getCursorID();
							freeCursorList.Remove(closestCursor);
						} else maxFingerID = f_id;		
			
						TuioCursor addCursor  = new TuioCursor(currentTime,s_id,f_id,x,y);		
						cursorList.Add(s_id, addCursor);

						for (int i=0;i<listenerList.Count;i++) {
							TuioListener listener = (TuioListener)listenerList[i];
							if (listener!=null) listener.addTuioCursor(addCursor);
						}
					} else {
						TuioCursor updateCursor = (TuioCursor)cursorList[s_id];
						if((updateCursor.getX()!=x) || (updateCursor.getY()!=y)) {
							
							TuioCursor tcur = new TuioCursor(currentTime,s_id,updateCursor.getCursorID(),x,y);
							tcur.update(currentTime,x,y,X,Y,m);
							frameCursors.Add(tcur);
							/*updateCursor.update(currentTime,x,y,X,Y,m);
							for (int i=0;i<listenerList.Count;i++) {
								TuioListener listener = (TuioListener)listenerList[i];
								if (listener!=null) listener.updateTuioCursor(updateCursor);
							}*/

							//cursorList[s_id] = tcur;
						}
					}
					
				} else if (command == "alive") {
		
					for (int i=1;i<args.Count;i++) {
						// get the message content
						long s_id = (int)args[i];
						newCursorList.Add(s_id);
						// reduce the cursor list to the lost cursors
						if (aliveCursorList.Contains(s_id)) 
							aliveCursorList.Remove(s_id);
					}
					
					// remove the remaining cursors
					for (int i=0;i<aliveCursorList.Count;i++) {
						long s_id = aliveCursorList[i];
						if (!cursorList.ContainsKey(s_id)) continue;
						TuioCursor removeCursor = cursorList[s_id];
                        int c_id = removeCursor.getCursorID();
						cursorList.Remove(s_id);
						removeCursor.remove(currentTime);

                        if (c_id == maxFingerID)
                        {
                            maxFingerID = -1;


                            if (cursorList.Count > 0)
                            {

                                IEnumerator<KeyValuePair<long, TuioCursor>> clist = cursorList.GetEnumerator();
                                while (clist.MoveNext())
                                {
                                    int f_id = clist.Current.Value.getCursorID();
                                    if (f_id > maxFingerID) maxFingerID = f_id;
                                }
								
							   List<TuioCursor> freeCursorBuffer = new List<TuioCursor>();
							   IEnumerator<TuioCursor> flist = freeCursorList.GetEnumerator();
                                while (flist.MoveNext())
                                {
								   TuioCursor testCursor = flist.Current;

                                    if (testCursor.getCursorID() < maxFingerID) freeCursorBuffer.Add(testCursor);
                                }
								freeCursorList = freeCursorBuffer;
                            }
                        } else  if (c_id < maxFingerID) freeCursorList.Add(removeCursor);
							

						for (int j=0;j<listenerList.Count;j++) {
							TuioListener listener = (TuioListener)listenerList[j];

							if (listener!=null) listener.removeTuioCursor(removeCursor);
						}
					}
					
					List<long> buffer = aliveCursorList;
					aliveCursorList = newCursorList;
					
					// recycling of the List
					newCursorList = buffer;
					newCursorList.Clear();
				} else if (command=="fseq") {
					int fseq = (int)args[1];
					bool lateFrame = false;
					
					if (fseq>0) {
						if ((fseq>=currentFrame) || ((currentFrame-fseq)>100)) currentFrame = fseq;
						else lateFrame = true;
					}
					
					if (!lateFrame) {
						
						IEnumerator<TuioCursor> frameEnum = frameCursors.GetEnumerator();
						while(frameEnum.MoveNext()) {
							TuioCursor tcur = frameEnum.Current;
							TuioCursor updateCursor = getTuioCursor(tcur.getSessionID());
							updateCursor.update(currentTime,tcur.getX(),tcur.getY(),tcur.getXSpeed(),tcur.getYSpeed(),tcur.getMotionAccel());
			
							for (int i=0;i<listenerList.Count;i++) {
								TuioListener listener = (TuioListener)listenerList[i];
								if (listener!=null) listener.updateTuioCursor(updateCursor);
							}
						}
						
						for (int i=0;i<listenerList.Count;i++) {
							TuioListener listener = (TuioListener)listenerList[i];
							if (listener!=null) listener.refresh(currentTime);
						}					
						if (fseq>0) currentTime.reset();
					}
					frameCursors.Clear();
				}

			}
		}
		
		/**
		 * Adds the provided TuioListener to the list of registered TUIO event listeners
		 *
		 * @param  listener  the TuioListener to add
		 */
		public void addTuioListener(TuioListener listener) {
			listenerList.Add(listener);
		}
		
		/**
		 * Removes the provided TuioListener from the list of registered TUIO event listeners
		 *
		 * @param  listener  the TuioListener to remove
		 */
		public void removeTuioListener(TuioListener listener) {	
			listenerList.Remove(listener);
		}
		
		/**
		 * Removes all TuioListener from the list of registered TUIO event listeners
		 */
		public void removeAllTuioListeners() {	
			listenerList.Clear();
		}

		/**
		 * Returns a Vector of all currently active TuioObjects
		 *
		 * @return  a Vector of all currently active TuioObjects
		 */
		public List<TuioObject> getTuioObjects() {
			return new List<TuioObject>(objectList.Values);
		}
	
		/**
		 * Returns a Vector of all currently active TuioCursors
		 *
		 * @return  a Vector of all currently active TuioCursors
		 */
		public List<TuioCursor> getTuioCursors() {

			return new List<TuioCursor>(cursorList.Values);
		}	

		/**
		 * Returns the TuioObject corresponding to the provided Session ID
		 * or NULL if the Session ID does not refer to an active TuioObject
		 *
		 * @return  an active TuioObject corresponding to the provided Session ID or NULL
		 */
		public TuioObject getTuioObject(long s_id) {
			TuioObject tobject = null;
			objectList.TryGetValue(s_id,out tobject);
			return tobject;
		}
		
		/**
		 * Returns the TuioCursor corresponding to the provided Session ID
		 * or NULL if the Session ID does not refer to an active TuioCursor
		 *
		 * @return  an active TuioCursor corresponding to the provided Session ID or NULL
		 */
		public TuioCursor getTuioCursor(long s_id) {
			TuioCursor tcursor = null;
			cursorList.TryGetValue(s_id, out tcursor);
			return tcursor;
		}		 

	}
}
