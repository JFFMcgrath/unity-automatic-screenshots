using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{

	public const int MAX_HISTORY_ENTRIES = 100;

	public float lastStateDuration {
		get;
		private set;
	}

	public float timeSinceLastStateChange{
		get{
			return Time.time - lastStateChange;
		}
	}

	public float lastStateChange{
		get;
		private set;
	}

	public struct StateTransition<S>{

		public readonly S from;

		public readonly S to;

		public StateTransition(S fromState, S toState){

			from = fromState;
			to = toState;

		}

	}

	Dictionary<T,List<StateTransition<T>>> allowedTransitions = new Dictionary<T, List<StateTransition<T>>>();
	Dictionary<T,List<StateTransition<T>>> blockedTransitions = new Dictionary<T, List<StateTransition<T>>>();

	public void AllowTransitions(T from, T[] to){

		for (int i = 0; i < to.Length; i++) {

			AllowTransition(from,to[i]);

		}

	}

	public void AllowTransition(T from, T to){

		if (!allowedTransitions.ContainsKey (from)) {

			allowedTransitions.Add(from,new List<StateTransition<T>>());

		}

		allowedTransitions [from].Add(new StateTransition<T>(from,to));

	}

	public void BlockTransitions(T from, T[] to){

		for (int i = 0; i < to.Length; i++) {

			BlockTransition(from,to[i]);

		}

	}

	public void BlockTransition(T from, T to){

		if (!blockedTransitions.ContainsKey (from)) {

			blockedTransitions.Add(from,new List<StateTransition<T>>());

		}

		blockedTransitions [from].Add(new StateTransition<T>(from,to));

	}

	public bool IsTransitionAllowed(T from, T to){

		if (blockedTransitions.ContainsKey (from)) {

			for(int i = 0; i < blockedTransitions[from].Count; i++){

				if(blockedTransitions[from][i].to.Equals(to)){
					return false;
				}

			}

		}

		if (allowedTransitions.ContainsKey (from)) {

			for(int i = 0; i < allowedTransitions[from].Count; i++){

				if(allowedTransitions[from][i].to.Equals(to)){
					return true;
				}

			}

		}

		return permissive;

	}

	bool permissive = true;
	T currentState;
	List<T> stateHistory = new List<T>();

	public T GetPreviousState(){

		if (stateHistory.Count == 0) {

			return default(T);

		}

		return stateHistory[stateHistory.Count-1];
	}

	public T GetState(){

		return currentState;

	}

	public void SetState(T state){

		if(state.Equals(currentState)) {

			return;

		}

		if (!IsTransitionAllowed (currentState, state)) {

			Debug.LogError ("Blocked transition: " + currentState + " to " + state);

			Debug.Break ();

			return;

		}

		lastStateDuration = timeSinceLastStateChange;

		lastStateChange = Time.time;

		EndState(currentState);

		currentState = state;

		BeginState(currentState);

	}

	public delegate void StateChanged(T state);

	StateChanged beginState;
	StateChanged endState;

	public StateMachine(StateChanged beginState, StateChanged endState) : this(beginState,endState,true){}

	public StateMachine(StateChanged beginState, StateChanged endState, bool permissive){

		this.beginState = beginState;
		this.endState = endState;
		this.permissive = permissive;

	}

	public void ClearStateHistory(){

		stateHistory.Clear ();

	}

	void BeginState(T state){
		if(this.beginState != null) {
			this.beginState(state);
		}
	}

	void EndState(T state){

		if (stateHistory.Count == MAX_HISTORY_ENTRIES) {

			stateHistory.Clear();

		}

		stateHistory.Add (state);

		if(this.endState != null) {
			this.endState(state);
		}
	}

	public void MoveToPreviousState(){

		SetState(stateHistory[stateHistory.Count-1]);

	}

}

