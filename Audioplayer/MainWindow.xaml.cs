using Microsoft.Win32;
using NAudio.Wave;
using System.IO;
using System.IO.Enumeration;
using System.Media;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace Audioplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> audioPathsFull = new List<String>();
        List<String> audioPathsShort = new List<String>();
        string currentAudioShortName;
        public WaveOutEvent outputDevice;

        private System.Timers.Timer aTimer;

        private MediaPlayer mediaPlayer;
        private double totalSeconds;
        double totalTime;
        bool canPlay = true;

        public MainWindow()
        {
            InitializeComponent();
            audioPathsShort.Clear();
            audioPathsFull.Clear();
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
            totalSeconds = 0;
        }

        private async void MediaPlayer_MediaOpened(object? sender, EventArgs e)
        {
            totalTime = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds + mediaPlayer.NaturalDuration.TimeSpan.TotalMinutes * 60;
            //double TotalMilliseconds = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            slider.Minimum = 0;
            slider.Maximum = (int)totalTime;
            slider.Value = 0;
        }    

        

        private void MediaPlayer_MediaEnded(object? sender, EventArgs e)
        {
            //headerBlock.Text = "Воспроизведение завершено";
            mediaPlayer.Close();
            aTimer.Stop();
            aTimer.Dispose();
        }


        public void CurrentPosition(Object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                slider.Value = mediaPlayer.Position.Seconds + mediaPlayer.Position.Minutes * 60;
                headerBlock.Text = slider.Value.ToString();
            });
        }
        // начало воспроизведения
        void Play_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = audioList.SelectedIndex;

            mediaPlayer.Open(new Uri(audioPathsFull[selectedIndex], UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();

            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += CurrentPosition;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            headerBlock.Text = audioPathsShort[selectedIndex];
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                              e.SignalTime);
        }

        // пауза
        void Pause_Click(object sender, RoutedEventArgs e)
        {
           
        }
        // остановка
        void Stop_Click(object sender, RoutedEventArgs e)
        {
            
        }
        // если открытие файла завершилось с ошибкой
        void MediaPlayer_MediaFailed(object? sender, ExceptionEventArgs e)
        {
            headerBlock.Text = "Ошибка открытия файла";
        }
        
        // окончание воспроизведения
        void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            
        }

        private void chooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            { 
                foreach (string safeFileName in openFileDialog.SafeFileNames)
                {
                    audioPathsShort.Add(safeFileName);
                    audioList.Items.Add(safeFileName);
                }

                foreach (string fileName in openFileDialog.FileNames)
                {
                    audioPathsFull.Add(fileName);
                }

                if (audioPathsShort.Count > 0 && audioPathsFull.Count > 0)
                {
                    headerBlock.Text = audioPathsShort[0];
                }
                else
                {
                    MessageBox.Show("Файлы отсутствуют");
                }
                audioList.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Файл не выбран");

            }
        }

        // Выбор файла из списка
        private void audioList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (audioList.SelectedIndex >= 0)
            {
                //outputDevice.Stop();
                int selectedIndex = audioList.SelectedIndex;

                //outputDevice.Init(new AudioFileReader(audioPathsFull[selectedIndex]));
                currentAudioShortName = audioPathsShort[selectedIndex];
                //outputDevice.Play();
                headerBlock.Text = audioPathsShort[selectedIndex];
            }
            else
            {
                MessageBox.Show("Выделите аудиозапись");
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            int selectedIndex = audioList.SelectedIndex;
            headerBlock.Text = "";

            audioPathsFull.RemoveAt(selectedIndex);
            audioPathsShort.RemoveAt(selectedIndex);
            audioList.Items.RemoveAt(selectedIndex);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*
            canPlay = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(slider.Value);
            canPlay = true;
            */
        }

        private void Grid_PreviewDragOver(object sender, DragEventArgs e)
        {

        }

        private void audioList_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = false;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (var file in files)
                {
                    var ext = System.IO.Path.GetExtension(file);
                    if (ext.Equals(".mp3"))
                    {
                        e.Handled = true;

                        
                    }
                }
            }
        }

        private void audioList_PreviewDrop(object sender, DragEventArgs e)
        {

        }

        private void audioList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (var file in files)
                {
                    audioPathsShort.Add(file);
                    audioPathsFull.Add(file);
                    audioList.Items.Add(file);
                }
            }
        }

        private void OpenPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}