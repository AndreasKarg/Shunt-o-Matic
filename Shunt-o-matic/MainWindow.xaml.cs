using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ShunterPhysics;

namespace Shunt_o_matic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ShunterLocoSimulation _shunterLocoSimulation;
        private readonly TrainSimulation _trainSimulation;
        private const float TimerInterval = 1.0f/30.0f;

        public MainWindow()
        {
            _shunterLocoSimulation = new ShunterLocoSimulation();
            _trainSimulation = new TrainSimulation(_shunterLocoSimulation);
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(TimerInterval);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _trainSimulation.Tick((float)Throttle.Value, TimerInterval);
            TELabel.Content = _trainSimulation.TractiveEffort / 1000.0f;
            AccelerationLabel.Content = _trainSimulation.Acceleration;
            SpeedLabel.Content = _trainSimulation.Speed * 3.6f;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _trainSimulation.Reset();
        }
    }
}
