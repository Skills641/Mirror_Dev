﻿//Copyright 2018, Davin Carten, All rights reserved

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace emotitron.NST
{
	/// <summary>
	/// Base component for all Rewind components that go on networked objects.
	/// </summary>
	public abstract class NSTRewindComponent : NSTComponent
	{
		[HideInInspector] public NSTRewindEngine nstRewindEngine;
		//[HideInInspector] public NSTRewindSettings nstRewindSettings;

		/// <summary>
		/// Replacement for standard Awake that instead of firing when the component first wakes up, fires after the NetworkSyncTransform
		/// completes its Awake() and then the base class NSTRewindComponent completes its initialization. This ensures the NST has been initialized first.
		/// </summary>
		public override void OnNstPostAwake()
		{
			base.OnNstPostAwake();

			nstRewindEngine = nst.GetComponent<NSTRewindEngine>();

			if (nstRewindEngine == null)
				nstRewindEngine = Rewind.RewindTools.AddRewindEngineOnFly(nst, false);
		}
	}


#if UNITY_EDITOR

	[CustomEditor(typeof(NSTRewindComponent))]
	[CanEditMultipleObjects]
	public abstract class NSTRewindComponentEditor : NSTHeaderEditorBase
	{
		protected NSTRewindComponent nstRewindComponent;
		//protected NSTRewindEngine nstRewindEngine;

		public override void OnEnable()
		{
			headerName = HeaderRewindAddonName;
			headerColor = HeaderRewindAddonColor;
			base.OnEnable();

			nstRewindComponent = (NSTRewindComponent)target;

			if (RewindSettings.Single.autoAddRewindEngine)
			{
				nstRewindComponent.nstRewindEngine = NSTRewindEngine.EnsureExistsOnRoot(nstRewindComponent.transform);
			}

			//nstRewindEngine = nstRewindComponent.nstRewindEngine;

		}
	}
#endif
}




