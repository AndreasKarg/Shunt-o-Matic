using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using ShunterPhysics;
using NAudio.Wave;
using UnityEngine;

namespace Shunt_o_matic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ShunterLocoSimulation _shunterLocoSimulation;
        private readonly TrainSimulation _trainSimulation;
        private IAdjustableSound _engineRunSound;
        private IAdjustableSound _engineIdleSound;
        private IAdjustableSound _gearboxSound;

        private readonly WaveOutEvent _waveOut = new WaveOutEvent();

        private const float TimerInterval = 1.0f / 30.0f;

        public MainWindow()
        {
            _shunterLocoSimulation = new ShunterLocoSimulation();
            _trainSimulation = new TrainSimulation(_shunterLocoSimulation);
            InitializeComponent();

            var soundMixer = new MixingWaveProvider32();

            InitializeSounds(soundMixer);

            _waveOut.Init(soundMixer);
            _waveOut.Volume = 1.0f;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(TimerInterval);
            timer.Tick += timer_Tick;
            timer.Start();
            _waveOut.Play();
        }

        private void InitializeSounds(MixingWaveProvider32 soundMixer)
        {
            var workingDir = System.AppDomain.CurrentDomain.BaseDirectory;
            var soundsFolder = System.IO.Path.Combine(workingDir, "Audio");

            IAdjustableSound LoadSoundOrCreateDummyFrom(string fileName)
            {
                var filePath = System.IO.Path.Combine(soundsFolder, fileName);
                if (!File.Exists(filePath))
                {
                    StatusBlock.Text += $"File {filePath} not found. Not loading sound...\n";
                    return new DummySound();
                }

                var loadedSound = new AdjustableSound(filePath);
                soundMixer.AddInputStream(loadedSound);
                return loadedSound;
            }

            _engineRunSound = LoadSoundOrCreateDummyFrom("train_engine_layer_piston.wav");
            _engineIdleSound = LoadSoundOrCreateDummyFrom("train_engine_layer_idle_01.wav");
            _gearboxSound = LoadSoundOrCreateDummyFrom("train_engine_layer_gear.wav");
        }

        void UpdateSounds()
        {
            var engineRpm = _shunterLocoSimulation.EngineActualRpm;
            _engineRunSound.PlaybackSpeed = engineRpm / ShunterLocoSimulation.MaxEngineRPM;
            _engineIdleSound.PlaybackSpeed = engineRpm / ShunterLocoSimulation.IdleEngineRPM;

            const float idleToRunSoundCrossoverRpm = 400.0f;
            const float crossoverRpmBandWidth = 150.0f;

            var runSoundVolume =
                Mathf.Lerp(0, 1, (engineRpm - idleToRunSoundCrossoverRpm) / (crossoverRpmBandWidth / 2.0f) + 0.5f);

            var idleSoundVolume =
                Mathf.Lerp(1, 0, (engineRpm - idleToRunSoundCrossoverRpm) / (crossoverRpmBandWidth / 2.0f) + 0.5f);

            //_engineRunSound.Volume = runSoundVolume;
            _engineIdleSound.Volume = idleSoundVolume;
            _gearboxSound.Volume = 2.0f;

            var gearboxSpeed = Mathf.Lerp(0.1f, 2.0f,_trainSimulation.Speed / 80.0f);
            _gearboxSound.PlaybackSpeed = gearboxSpeed;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _trainSimulation.Tick((float)Throttle.Value, TimerInterval);
            TELabel.Content = _trainSimulation.TractiveEffort / 1000.0f;
            AccelerationLabel.Content = _trainSimulation.Acceleration;
            SpeedLabel.Content = _trainSimulation.Speed * 3.6f;
            EngRPMLabel.Content = _shunterLocoSimulation.EngineActualRpm;

            UpdateSounds();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _trainSimulation.Reset();
        }
    }
}
