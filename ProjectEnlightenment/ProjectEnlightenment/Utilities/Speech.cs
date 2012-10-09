using System;
using System.Collections;
using System.Linq;
using System.Text;
using SpeechLib;

namespace ProjectEnlightenment
{
  /// <summary>
  /// audio output class
  /// </summary>
  public sealed class Speech
  {
    /// <summary>
    /// TextToSpeech engine object
    /// </summary>
    private static SpVoice voice = new SpVoice();

    /// <summary>
    /// Asynchronous Speech without clearing the queue before speak
    /// </summary>
    /// <param name="message">Text to speak</param>
    public static void speakNormal(String message) 
    {
      voice.Speak(message, SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }

    /// <summary>
    /// Asynchronous Speech with clearing the queue before speak
    /// </summary>
    /// <param name="message">Text to speak</param>
    public static void speakPurge(String message)
    {
      voice.Speak(message, 
        SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak|SpeechVoiceSpeakFlags.SVSFlagsAsync);
    }

    /// <summary>
    /// Synchronous Speech with clearing the queue before speak
    /// </summary>
    /// <param name="message">Text to speak</param>
    public static void speakPurgeSync(String message) 
    {
      voice.Speak(message, SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }

    /// <summary>
    /// Adjust the rate of the speak for the rest of the messages
    /// -10 slowest
    /// 10  fastest
    /// </summary>
    /// <param name="rate"></param>
    public static void adjustRate(int rate) 
    {
      voice.Rate = rate;
    }

    public static void changeVoice() 
    {
      if (voice.GetVoices("", "").Count > 1)
      {
        int order = 0;
        foreach (ISpeechObjectToken token in voice.GetVoices("", ""))
        {
          order++;
          if (voice.Voice == token)
          {
            break;
          }
        }
          voice.Voice = voice.GetVoices("", "").Item(order%voice.GetVoices("", "").Count);
        }
      else 
      {
        speakPurge("There is one voice you can not change it");
      }
    }
  }
}
