﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using emotitron.NST;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Use this CS file as a reference for how to interact with NSTCastDefinition components. 
/// First add a NSTCastDefinition to a prefab that will be networked. This defines a test (rayCast, capsuleCast, OverlapSphere, etc)
/// nstRewindEngine.NetworkCast(castDefinition) is called to fire the test.
/// INSTCast and INstCastResults callbacks allow you to respond to the network events this generates.
/// </summary>
public class RewindSampleTemplate : NSTComponent, INstCast, INstCastResults
{
	/// Inspector fields
	[Tooltip("Match this name to the name of a CastDefinition on this prefab that you want to monitor")]
	public string CastDefName = "ReplaceWithNameOfCastDefinition";
	[Tooltip("When received OnCastResults() should be handled.")]
	public ApplyTiming applyTiming = ApplyTiming.OnEndInterpolate;
	[Tooltip("Select inputs that will trigger the associated CastDefinition.")]
	public emotitron.InputSystem.InputSelectors triggerInputs = new emotitron.InputSystem.InputSelectors(KeyCode.F);

	
	/// Cached items
	[HideInInspector]
	public int castDefinitionId;
	[HideInInspector]
	public NSTRewindEngine nstRewindEngine;

	void Start ()
	{
		/// Cache the RewindEngine component and CastDefinition. 
		nstRewindEngine = transform.root.GetComponent<NSTRewindEngine>();
		castDefinitionId = nstRewindEngine.CastDefIdLookup(CastDefName);


		if (castDefinitionId == -1)
		{
			var nstCastDef = GetComponent<NSTCastDefinition>();
			if (!nstCastDef)
			{
				Debug.LogError("Unable to find a NSTCastDefinition for " + this.GetType().Name + " on GameObhect " + name);
			}

			Debug.LogWarning("Since no CastDefinition with the name " + CastDefName + " exists, will use NSTCastDefinition named '" + nstCastDef.castDefinition.name + "' with id: " + castDefinitionId);

			castDefinitionId = nstCastDef.castDefinition.id;
		}
	}
	
	void Update ()
	{
		/// Calling NetworkCast queues a cast event for the CastDefinition and starts the process.
		if (na.IsMine && triggerInputs.Test())
			nstRewindEngine.NetworkCast(castDefinitionId);
	}

	/// <summary>
	/// This is called on the initiating owner (usually the player) only. Any client side feedback to the user should happen here,
	/// such as weapon fire graphics and audio. This is called immediately, before the serer is notified and has confirmed
	/// so any actions here are cosmetic in nature. Use the OnResults callback instead to handle confirmed results.
	/// </summary>
	/// <param name="frame">The frame associated with the cast event.</param>
	/// <param name="castDef">The CastDefinition that has been invoked.</param>

	public void OnCast(Frame frame, CastDefinition castDef)
	{
		/// Only process the castDef.id we are interested in (only used if we are using the interface callback)
		/// You can make your own cast id checks if you want this method to handle more than one cast definition.  
		if (castDef.id != castDefinitionId)
			return;

		Vector3 srcPos = castDef.sourceObject.transform.position;
		Quaternion srcRot = castDef.sourceObject.transform.rotation;


		/// Put any immediate owner specific feedback actions here. 
		Debug.DrawRay(srcPos, srcRot * new Vector3(0, 0, 100), Color.green, 1f);
	}
	
	public void OnCastResults(CastResults castresults, ApplyTiming applyTiming)
	{
		/// Only process the castDef.id we are interested in 
		if (castresults.CastDef.id != castDefinitionId)
			return;

		bool isOfftick = castresults.frame.frameid >= nst.frameCount;

		/// For this sample we only run this code for the specified timing. 
		/// OnCastResults actually fires three times, once for each of the ApplyTiming values.
		/// Offtick updates only fire on receive, so you will want to process those immediately if you allow them (checkbox on NST)
		if (applyTiming != this.applyTiming && !isOfftick)
			return;

		/// Use the sourceObject as your origin for any graphics
		Debug.DrawRay(castresults.CastDef.sourceObject.transform.position, castresults.CastDef.sourceObject.transform.rotation * Vector3.forward * 20f, Color.blue, .5f);

		List<NetworkSyncTransform> hits = castresults.nsts;
		List<int> hitGroupMasks = castresults.hitGroupMasks;

		/// Some bool values you can use to determine how to respond to this event differently for owner/server/others.
		bool isMaster = MasterNetAdapter.ServerIsActive;
		bool isClient = MasterNetAdapter.ClientIsActive;
		bool isMine = na.IsMine;
		bool isActingAuthority = na.IAmActingAuthority;


		/// Iterate through all of the NSTs indicated as having been hit in CastResults.
		for (int i = 0; i < hits.Count; i++)
		{
			Debug.Log("Sample Code: Hit: " + hits[i].name + " on Hit Groups " + hitGroupMasks);

			/// Here is a sample tree for responding to this event.
			
			if (isActingAuthority)
			{
				/// Place authority specific handlers here, such as calculating damage from hits
			}

			if (isClient)
			{
				/// Place specific code for other clients here
				if (isMine)
				{
					/// Place owner only code here, such as corrections if OnCast did something on the owner you want to undo
				}
				else
				{
					/// Place code that should run on clients other than the owner.
				}
			}
		}
	}


}

#if UNITY_EDITOR

[CustomEditor(typeof(RewindSampleTemplate))]
[CanEditMultipleObjects]
public class RewindSampleTemplateEditor : NSTSampleHeader
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.HelpBox("Copy the code from this template as a starting point for making your own class that triggers and responds to NSTCastDefinition actions.", MessageType.None);
	}
}

#endif


