using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks; // new

// created by NT
// this is where you add the Azure reconition languages
// from the Azure website, they will be in the form "es-US"
// but b/c you can't have "-" in an enum it needs to be in format "es_US" instead
// basically replace "-" with "_"

namespace Azure
{
public class AzureSpeech : MonoBehaviour
{
    private const string SubscriptionKey = ""; // put your Azure subscription key here
    private const string Region = ""; // put your Azure region here

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    public bool waitingForReco; // mic
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;   
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    public enum SpeechRecognitionLanguages {
          es_US, // Spanish
          id_ID, // Indonesian
          zh_CN, // Mandarin
          fr_FR // French
          // Add more recognition languages here (check the Azure website for the language codes)
    };
    public SpeechRecognitionLanguages m_SpeechRecognitionLanguage;
    public Dictionary<string, SpeechSynthesizer> m_SpeechSynthesizerDictionary = new Dictionary<string, SpeechSynthesizer>();

    // converts the "_" to "-" for the language codes
    public string getSpeechRecognitionLanguage() {
        return Enum.GetName(typeof(SpeechRecognitionLanguages), m_SpeechRecognitionLanguage).Replace('_', '-');
    }

    public void AddSynthesizer(string voice) {
        if(!m_SpeechSynthesizerDictionary.ContainsKey(voice)) {        
            speechConfig.SpeechSynthesisVoiceName = voice;
            m_SpeechSynthesizerDictionary.Add(voice, new SpeechSynthesizer(speechConfig, null));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SpeechRecognitionLanguage = getSpeechRecognitionLanguage();

        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);
        synthesizer = new SpeechSynthesizer(speechConfig, null)
        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };
    }

    // returns an audioClip of the text parameter
    public AudioClip TextToSpeech(string text, string voice) {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = null;
        var startTime = DateTime.Now;
        synthesizer = m_SpeechSynthesizerDictionary[voice];

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(text).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                text,
                SampleRate * text.Length / 10, // Can speak 10mins audio as maximum  // old: * 600
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }
                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }
                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });
            return audioClip;
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    // returns a Task<string> of audio input from microphone
    public async Task<string> MicSpeechToText() {
        // Make sure to dispose the recognizer after use!
        using (var recognizer = new SpeechRecognizer(speechConfig))
        {
            lock (threadLocker)
            {
                waitingForReco = true;
            }

            // Starts speech recognition, and returns after a single utterance is recognized. The end of a
            // single utterance is determined by listening for silence at the end or until a maximum of 15
            // seconds of audio is processed.  The task returns the recognition text as result.
            // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
            // shot recognition like command or query.
            // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
            var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

            // Checks result.
            string newMessage = string.Empty;
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                newMessage = result.Text;
            }
            else if (result.Reason == ResultReason.NoMatch)
            {
                newMessage = "NOMATCH: Speech could not be recognized.";
                newMessage = "";
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                newMessage = $"CANCELED: Reason={cancellation.Reason} ErrorDetails={cancellation.ErrorDetails}";
                newMessage = "";
            }

            lock (threadLocker)
            {
                waitingForReco = false;
                return newMessage;
            }
        }
    }

}
}