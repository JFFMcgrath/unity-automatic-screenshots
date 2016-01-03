using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public abstract class FreeEditorListener : UnityEditor.AssetModificationProcessor {

	static FreeEditorListener ()
	{

		editorState = new StateMachine<FreeEditorState> (BeginEditorState, EndEditorState);

		EditorApplication.playmodeStateChanged += HandleEditorStateChanged;

	}

	public static string[] OnWillSaveAssets(string[] paths){

		FreeEditorState state = editorState.GetState ();

		editorState.SetState (FreeEditorState.IsSaving);

		editorState.SetState (state);

		return paths;

	}

	static StateMachine<FreeEditorState> editorState;

	static List<FreeEditorEventSubscriber> stateSubscribers = new List<FreeEditorEventSubscriber>();
	static List<FreeEditorEventSubscriber> updateSubscribers = new List<FreeEditorEventSubscriber>();

	public static void SubscribeToStateChange(FreeEditorEventSubscriber subscriber){

		if (!stateSubscribers.Contains (subscriber)) {

			stateSubscribers.Add (subscriber);

		}

	}

	public static void UnsubscribeFromStateChange(FreeEditorEventSubscriber subscriber){

		if (stateSubscribers.Contains (subscriber)) {

			stateSubscribers.Remove (subscriber);

		}

	}

	public static void SubscribeToUpdate(FreeEditorEventSubscriber subscriber){

		bool isEmpty = (updateSubscribers.Count == 0);

		if (!updateSubscribers.Contains (subscriber)) {

			updateSubscribers.Add (subscriber);

		}

		if (updateSubscribers.Count > 0 && isEmpty) {

			EditorApplication.update += Update;

		}

	}

	public static void UnsubscribeFromUpdate(FreeEditorEventSubscriber subscriber){

		if (updateSubscribers.Contains (subscriber)) {

			updateSubscribers.Remove (subscriber);

		}

		if (updateSubscribers.Count == 0) {

			EditorApplication.update -= Update;

		}

	}

	static void HandleEditorStateChanged ()
	{

		FreeEditorState currentState = editorState.GetState ();

		if (EditorApplication.isCompiling) {

			editorState.SetState (FreeEditorState.IsCompiling);

		} else if (EditorApplication.isPlaying) {

			editorState.SetState (FreeEditorState.Playing);

		} else if (EditorApplication.isUpdating) {

			editorState.SetState (FreeEditorState.IsUpdating);

		} else if (EditorApplication.isPaused) {

			editorState.SetState (FreeEditorState.IsPaused);

		}
		else if (EditorApplication.isPlayingOrWillChangePlaymode) {

			if (currentState == FreeEditorState.Playing) {
				editorState.SetState (FreeEditorState.IsPaused);
			} else if (currentState == FreeEditorState.IsPaused) {
				editorState.SetState (FreeEditorState.Playing);
			} else {
				editorState.SetState (FreeEditorState.None);
			}
		}
		else {

			//Nothing is happening!
			editorState.SetState (FreeEditorState.None);

		} 

	}

	static void BeginEditorState (FreeEditorState state)
	{

		switch (state) {

			case FreeEditorState.IsCompiling:
			{
				break;
			}
			case FreeEditorState.IsPaused:
			{
				break;
			}
			case FreeEditorState.IsPlayingOrWillChangePlayMode:
			{
				break;
			}
			case FreeEditorState.IsUpdating:
			{
				break;
			}
			case FreeEditorState.None:
			{
				break;
			}
			case FreeEditorState.Playing:
			{
				break;
			}

		}

		for (int i = 0; i < stateSubscribers.Count; i++) {

			stateSubscribers [i].HandleEditorBeginState (state);

		}

	}

	static void EndEditorState (FreeEditorState state)
	{

		switch (state) {

			case FreeEditorState.IsCompiling:
			{
				break;
			}
			case FreeEditorState.IsPaused:
			{
				break;
			}
			case FreeEditorState.IsPlayingOrWillChangePlayMode:
			{
				break;
			}
			case FreeEditorState.IsUpdating:
			{
				break;
			}
			case FreeEditorState.None:
			{
				break;
			}
			case FreeEditorState.Playing:
			{
				break;
			}

		}

		for (int i = 0; i < stateSubscribers.Count; i++) {

			stateSubscribers [i].HandleEditorEndState (state);

		}

	}

	static void Update ()
	{
		for (int i = 0; i < stateSubscribers.Count; i++) {

			stateSubscribers [i].HandleEditorUpdate (editorState.GetState());

		}
	}

}
