using UnityEngine;
using System.Collections;

public abstract class FreeEditorEventSubscriber {

	public abstract void HandleEditorBeginState(FreeEditorState state);
	public abstract void HandleEditorEndState(FreeEditorState state);
	public abstract void HandleEditorUpdate(FreeEditorState state);

	protected void SubscribeToEditorState(){

		FreeEditorListener.SubscribeToStateChange (this);

	}

	protected void UnsubscribeFromEditorState(){

		FreeEditorListener.UnsubscribeFromStateChange (this);

	}

	protected void SubscribeToEditorUpdate(){

		FreeEditorListener.SubscribeToUpdate (this);

	}

	protected void UnsubscribeFromEditorUpdate(){

		FreeEditorListener.UnsubscribeFromUpdate (this);

	}

}
