using OPSSDK;
using Ozeki.Media;
using Ozeki.Media.MediaHandlers;
using Ozeki.VoIP;

namespace OPSIVRSystem
{
    /// <summary>
    /// Manage the media data sources of a call.
    /// </summary>
    class MediaManager
    {

        /// <summary>
        /// Will be attached to calls.
        /// </summary>
        private AudioHandler phoneCallAudioReceiver;

        /// <summary>
        /// Will be attached to calls.
        /// </summary>
        private AudioHandler phoneCallAudioSender;

        /// <summary>
        /// Connects media handlers to each other
        /// </summary>
        private MediaConnector mediaConnector;

        /// <summary>
        /// Mixes audio from different sources. It will be connected to the PhoneCallListener.
        /// </summary>
        private AudioMixerMediaHandler incomingAudioMixer;
        /// <summary>
        /// Mixes audio from different sources. It will be connected to the PhoneCallListener.
        /// </summary>
        private AudioMixerMediaHandler outgoingAudioMixer;

        public MediaManager(ICall call)
        {
            mediaConnector = new MediaConnector();

            phoneCallAudioReceiver = new AudioMixerMediaHandler();
            phoneCallAudioSender = new AudioMixerMediaHandler();

            incomingAudioMixer = new AudioMixerMediaHandler();
            outgoingAudioMixer = new AudioMixerMediaHandler();
            
            call.ConnectAudioSender(outgoingAudioMixer);
            call.ConnectAudioReceiver(incomingAudioMixer);
            //mediaConnector.Connect(phoneCallAudioReceiver, incomingAudioMixer);
            //mediaConnector.Connect(outgoingAudioMixer, phoneCallAudioSender);

            //phoneCallAudioReceiver.AttachToCall(call);
            //phoneCallAudioSender.AttachToCall(call);

        }


        public void AttachVoIPHandlerToCall(VoIPMediaHandler voIPMediaHandler)
        {
            AudioHandler handler = voIPMediaHandler as AudioHandler;
            if (handler == null)
                return;

            if (voIPMediaHandler is WaveStreamRecorder || voIPMediaHandler is MP3StreamRecorder)
                mediaConnector.Connect(incomingAudioMixer, handler);
            else
                mediaConnector.Connect(handler, outgoingAudioMixer);
        }


        public void DeAttachVoIPHandlerToCall(VoIPMediaHandler voIPMediaHandler)
        {
            AudioHandler handler = voIPMediaHandler as AudioHandler;
            if (handler == null)
                return;

            if (voIPMediaHandler is WaveStreamRecorder ||voIPMediaHandler is MP3StreamRecorder)
                mediaConnector.Connect(incomingAudioMixer, handler);
            else
                mediaConnector.Disconnect(handler, outgoingAudioMixer);
        }
    }
}
