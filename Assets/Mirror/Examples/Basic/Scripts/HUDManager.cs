﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace App.Player
{
    public class HUDManager : NetworkBehaviour
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static HUDManager Instance;

        /// <summary>
        /// Message Text
        /// </summary>
        public Text MessageText;

        /// <summary>
        /// InputOutput Text object
        /// </summary>
        public GameObject InputOutputText;

        /// <summary>
        /// Holds all the text
        /// </summary>
        public GameObject TextHolder;

        /// <summary>
        /// Text Prefab
        /// </summary>
        public GameObject TextPrefab;

        /// <summary>
        /// List of Text gameObjects
        /// </summary>
        public List<GameObject> ListOfTexts = new List<GameObject>();

        /// <summary>
        /// Message when unpaused
        /// </summary>
        private string UnPausedMessage = "Press Escape to pause \n Press E to spawn objects";

        /// <summary>
        /// Message when paused
        /// </summary>
        public string PausedMessage = "Press Escape to unpause";

        /// <summary>
        /// Time recevied TimeStamp
        /// </summary>
        private float _TimeReceived = 0.0f;

        /// <summary>
        /// Awake this instance
        /// </summary>
        private void Awake()
        {
            if (HUDManager.Instance == null)
            {
                HUDManager.Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// Message Toggle
        /// </summary>
        /// <param name="toggle"></param>
        public void MessageToggle(bool toggle)
        {
            this.MessageText.text = toggle ? this.PausedMessage : this.UnPausedMessage;
        }

        [Client]
        public void CommandSent(uint clientID)
        {
            ClearInputOutput();
            GameObject textObj = (GameObject)Instantiate(this.TextPrefab);
            textObj.transform.SetParent(this.TextHolder.transform);
            textObj.transform.localScale = Vector3.one;
            textObj.GetComponent<Text>().text = string.Format("User{0} has spawned the Objects.", clientID);
            this.ListOfTexts.Add(textObj);
            this._TimeReceived = Time.realtimeSinceStartup;
        }

        [ClientRpc]
        public void RpcSpawnMessage()
        {
            GameObject textObj = (GameObject)Instantiate(this.TextPrefab);
            textObj.transform.SetParent(this.TextHolder.transform);
            textObj.transform.localScale = Vector3.one;
            textObj.GetComponent<Text>().text = string.Format("This user has received the spawned Object after {0:0.00}secs.", Time.realtimeSinceStartup - this._TimeReceived);
            this.ListOfTexts.Add(textObj);
            this._TimeReceived = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Clear the log
        /// </summary>
        public void ClearInputOutput()
        {
            for (int i = 0; i < this.ListOfTexts.Count; i++)
            {
                if (this.ListOfTexts[i] == null)
                    continue;
                GameObject.Destroy(this.ListOfTexts[i]);
                this.ListOfTexts[i] = null;
            }
            this.ListOfTexts = new List<GameObject>();
        }
    }
}

