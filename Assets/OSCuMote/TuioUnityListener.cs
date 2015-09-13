
using System.Collections;
using System.Collections.Generic;
using TUIO;

public class TuioUnityListener : TuioListener {
	
	private TuioClient client;
	public ArrayList cursors = new ArrayList();
	public ArrayList objects = new ArrayList();
	
	public TuioUnityListener() {
		client = new TuioClient();
		client.addTuioListener(this);
		client.connect();
	}
	
	public List<TuioCursor> getCursors()
	{
		return client.getTuioCursors();
	}
	public List<TuioObject> getObjects()
	{
		return client.getTuioObjects();
	}
	
	public void addTuioObject(TuioObject o) 
	{
		objects.Add(o);
	}
	public void removeTuioObject(TuioObject o) 
	{
		objects.Remove(o);
	}
	public void updateTuioObject(TuioObject o) {}
	
	public void addTuioCursor(TuioCursor o) {
		cursors.Add(o);
	}
	public void removeTuioCursor(TuioCursor o) {
		cursors.Remove(o);
	}
	public void updateTuioCursor(TuioCursor o) {
		//cursors.At[o]
	}
	public void refresh(TuioTime bundleTime) {}
}
