using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// created by NT
// this is where you add the Azure voices for different languages
// from the Azure website, they will be in the form "es-MX-JorgeNeural"
// but b/c you can't have "-" in an enum it needs to be in format "es_MX_JorgeNeural" instead
// basically replace "-" with "_"

namespace Azure
{
public class AzureVoice : MonoBehaviour
{
    public enum Voices {
          // Multilingual
          en_US_RyanMultilingualNeural,
          en_US_JennyMultilingualV2Neural,
          //  Spanish
          es_MX_JorgeNeural,
          es_MX_CecilioNeural,
          es_MX_GerardoNeural,
          es_MX_LibertoNeural,
          es_MX_LucianoNeural,
          es_MX_PelayoNeural,
          es_MX_YagoNeural,
          // Indonesian
          id_ID_ArdiNeural,
          jv_ID_DimasNeural,
          // Mandarin
          zh_CN_YunxiNeural,
          // French
          fr_FR_ClaudeNeural,
          // Add more voices here (check the Azure website for the voice codes)
    };
    public Voices m_Voice;

    public string getVoice() {
        return Enum.GetName(typeof(Voices), m_Voice).Replace('_', '-');
    }
    
    private void AddVoiceToDictionary() {
        Inworld.InworldController.Player.GetComponent<Azure.AzureSpeech>().AddSynthesizer(getVoice());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke("AddVoiceToDictionary", 1);
    }
}
}