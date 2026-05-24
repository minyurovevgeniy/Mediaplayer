using Microsoft.VisualBasic.Devices;
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
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace Audioplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Audiofile> audiofiles = new List<Audiofile>();

        string currentAudioShortName;
        public WaveOutEvent outputDevice;

        private System.Timers.Timer aTimer;

        private MediaPlayer mediaPlayer;
        
        double totalTime;
        

        public MainWindow()
        {
            InitializeComponent();
            audiofiles.Clear();

            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.MediaFailed += MediaPlayer_MediaFailed;
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

            mediaPlayer.Open(new Uri(audiofiles[selectedIndex].path, UriKind.RelativeOrAbsolute));
            mediaPlayer.Play();

            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += CurrentPosition;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            headerBlock.Text = audiofiles[selectedIndex].name;
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
                int fileCount = openFileDialog.FileNames.Length;
                for (int i = 0; i < fileCount; i++)
                {
                    audiofiles.Add(new Audiofile(openFileDialog.SafeFileNames[i], openFileDialog.FileNames[i]));
                    audioList.Items.Add(openFileDialog.SafeFileNames[i]);
                }
                if (audiofiles.Count > 0)
                {
                    headerBlock.Text = audiofiles[0].name;
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
                int selectedIndex = audioList.SelectedIndex;
                currentAudioShortName = audiofiles[selectedIndex].name;
                headerBlock.Text = audiofiles[selectedIndex].name;
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

            audiofiles.RemoveAt(selectedIndex);
            audioList.Items.RemoveAt(selectedIndex);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

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
                string[] filesNames = e.Data.GetData(DataFormats.FileDrop) as string[];
                foreach (var file in filesNames)
                {
                    audiofiles.Add(new Audiofile(System.IO.Path.GetFileName(file), file));
                    audioList.Items.Add(System.IO.Path.GetFileName(file));
                }
            }
        }

        private void OpenPlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Audiofile>));
            using (StreamReader reader = new StreamReader("playlist.xml"))
            {
                audiofiles = (List<Audiofile>)serializer.Deserialize(reader);
            }

            audioList.Items.Clear();

            foreach (Audiofile audio in audiofiles)
            {
                audioList.Items.Add(audio.name);
            }
        }

        public void SavePlaylistMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            // Create the serializer for a List of Person objects
            XmlSerializer serializer = new XmlSerializer(typeof(List<Audiofile>));

            // 2. Configure properties
            saveFileDialog.Filter = "XML files (*.xml)|*.xml";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = "playlist"; // Default file name

            // 3. Show the dialog and check the result
            if (saveFileDialog.ShowDialog() == true)
            {
                // Write the list to a file
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    serializer.Serialize(writer, audiofiles);
                }
            }
        }

    }
}