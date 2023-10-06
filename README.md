# Inworld AI Unity with multilingual support using Azure Speech SDK

You will need to download and import the custom Azure SDK for Unity package
github website for more info: https://github.com/Azure-Samples/cognitive-services-speech-sdk/blob/master/quickstart/csharp/unity/text-to-speech/README.md
Azure SDK Unity package link: https://aka.ms/csspeech/unitypackage

Once the package has been added, you need to add your Azure subscription key and region in AzureSpeech.cs code.
In AzureSpeech.cs, add more speech recognition languages, if they aren't already included
In AzureVoice,cs, add more voices, if they aren't already included
The current ones include Spanish, Indonesian, Mandarin, & French. So if you want another language you will have to find the code from the Azure website.

Then in the Unity Editor you can select the voice of the Inworld Character in the InworldCharacter in InworldController, as well as the speech recognition language in PlayerController.

When you run it, it will startout using the Inworld Speech Recognition for English.
To switch to the Azure recognition mode click ` and then say something in the desired language and press enter.

From my understanding, the inworld code is not the most recent one that got released Oct 3, 2023.
If you are using that most recent version, this Azure code may not work.

# Inworld AI Unity SDK

The **Inworld AI Unity SDK** is a powerful cross-platform virtual character integration plugin for Unity. It enables Developers to integrate Inworld.ai characteres into Unity Engine.

Please visit our [Unity Documentation](https://docs.inworld.ai/docs/tutorial-integrations/Unity/) page for more details.


Please create an account [here](https://studio.inworld.ai/signup) if you haven't already before getting started.
This tutorial series will begin with an overview of compatibility, assets, and API references.

![WWW-0a57710b165f3676f60d3667866c44b8](https://user-images.githubusercontent.com/123405577/219526213-97716f93-3b24-41c6-9890-2ea42c22faf2.gif)
