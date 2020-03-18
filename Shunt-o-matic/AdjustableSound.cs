using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SoundTouch.Net.NAudioSupport;

namespace Shunt_o_matic
{
    public class AdjustableSound : IWaveProvider, IAdjustableSound
    {
        private readonly SoundTouchWaveProvider _processorStream;
        private readonly VolumeSampleProvider _volumeSampleProvider;

        public float Volume
        {
            get => _volumeSampleProvider.Volume;
            set => _volumeSampleProvider.Volume = value;
        }

        public double PlaybackSpeed
        {
            get => _processorStream.Rate;
            set => _processorStream.Rate = value;
        }

        public AdjustableSound(string wavFilePath)
        {
            var reader = (WaveStream)new WaveFileReader(wavFilePath);

            // don't pad, otherwise the stream never ends
            var inputStream = new WaveChannel32(reader) { PadWithZeroes = false };

            var loopingStream = new LoopStream(inputStream);

            var resamplingFormat = new WaveFormat(48000, inputStream.WaveFormat.Channels);
            var resampler = new MediaFoundationResampler(loopingStream, resamplingFormat);

            var floatingPointStream = new Wave16ToFloatProvider(resampler);

            _processorStream = new SoundTouchWaveProvider(floatingPointStream);
            
            _volumeSampleProvider = new VolumeSampleProvider(_processorStream.ToSampleProvider());
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return (_volumeSampleProvider.ToWaveProvider()).Read(buffer, offset, count);
        }

        public WaveFormat WaveFormat => _volumeSampleProvider.ToWaveProvider().WaveFormat;
    }
}