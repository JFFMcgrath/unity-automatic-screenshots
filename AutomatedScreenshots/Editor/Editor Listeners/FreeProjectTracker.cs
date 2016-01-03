#define TRACK_PROJECT

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

#if UNITY_5_3 || UNITY_5_3_1

using UnityEngine.SceneManagement;

#endif

[InitializeOnLoad]
public class FreeProjectTracker : FreeEditorEventSubscriber {

	static FreeProjectTracker _instance;

	static FreeProjectTracker(){

		_instance = new FreeProjectTracker ();
		_instance.SubscribeToEditorState ();

	}

	const float SCREENSHOT_INTERVAL_FIRST_LOWER = 5f;
	const float SCREENSHOT_INTERVAL_FIRST_UPPER = 20f;
	const float SCREENSHOT_INTERVAL_LOWER = 15f;
	const float SCREENSHOT_INTERVAL_UPPER = 60f;

	const string SCREENSHOT_DIRECTORY = "FreeProgressPictures";
	const string NO_UI_SUFFIX = "_NOUI_";
	const string FILE_SUFFIX = ".png";

	float _startPlayTime;
	float _nextCaptureTime;
	float _nextNoUICaptureTime;

	void StartCapture(){

		_startPlayTime = Time.time;
		_nextCaptureTime = _startPlayTime + (UnityEngine.Random.Range (SCREENSHOT_INTERVAL_FIRST_LOWER, SCREENSHOT_INTERVAL_FIRST_UPPER));

	}

	void RestartCapture(){

		_startPlayTime = Time.time;
		_nextCaptureTime = _startPlayTime + (UnityEngine.Random.Range (SCREENSHOT_INTERVAL_LOWER, SCREENSHOT_INTERVAL_UPPER));

	}

	void CaptureProgress(bool editor, bool ui){

		#if UNITY_5_3 || UNITY_5_3_1

		string currentScene = SceneManager.GetActiveScene().name;

		#else

		string currentScene = Application.loadedLevelName;

		#endif

		string date;

		if (editor) {
			date = "Editor_"+SystemInfo.deviceModel + "-"+SystemInfo.deviceName+"_"+DateTime.Now.ToString ("dd-MM-yyyy-hh");
		}
		else{
			date = DateTime.Now.ToString ("dd-MM-yyyy-hh-mm-ss");
		}

		string fileName = date + "_" + currentScene;

		string dataPath = Application.dataPath.Replace("Assets","");

		string directory = dataPath + SCREENSHOT_DIRECTORY;

		if (!System.IO.Directory.Exists (directory)) {

			System.IO.Directory.CreateDirectory (directory);

		}

		string path = directory + "/" + fileName + FILE_SUFFIX;
		string no_ui_path = directory + "/" + fileName + NO_UI_SUFFIX + FILE_SUFFIX;

		if(ui){
			Application.CaptureScreenshot (path,1);
		}
		else{
			Application.CaptureScreenshot (no_ui_path,2);
		}
	}

	bool ShouldCapture(){

		return _nextCaptureTime <= Time.time;

	}

	bool ShouldCaptureNoUI(){

		return _nextNoUICaptureTime <= Time.time;

	}

	#region implemented abstract members of FreeEditorEventSubscriber

	public override void HandleEditorBeginState (FreeEditorState state)
	{

		#if !TRACK_PROJECT

		return;

		#endif

		if (state == FreeEditorState.Playing) {

			_startPlayTime = Time.time;

			StartCapture ();

			_nextNoUICaptureTime = _nextCaptureTime + 1f;

			SubscribeToEditorUpdate ();

		}

		if (state == FreeEditorState.IsSaving) {

			CaptureProgress (true,true);

		}

	}

	public override void HandleEditorEndState (FreeEditorState state)
	{

		if (state == FreeEditorState.Playing) {
			UnsubscribeFromEditorUpdate ();
		}
	}

	public override void HandleEditorUpdate (FreeEditorState state)
	{

		if (ShouldCapture ()) {

			CaptureProgress (false,true);

			RestartCapture ();

		}
		else if(ShouldCaptureNoUI()){

			CaptureProgress (false,false);

			_nextNoUICaptureTime = _nextCaptureTime + 1f;

		}


	}



	#endregion

}
