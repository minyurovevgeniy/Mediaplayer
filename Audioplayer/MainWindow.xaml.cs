using Microsoft.Win32;
using NAudio.Wave;
using System.IO.Enumeration;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        public MainWindow()
        {
            InitializeComponent();

            outputDevice = new WaveOutEvent();

            audioPathsShort.Clear();
            audioPathsFull.Clear();
        }

        // начало воспроизведения
        void Play_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = audioList.SelectedIndex;
            AudioFileReader audioFile = new AudioFileReader(audioPathsFull[selectedIndex]);
            slider.Maximum = audioFile.TotalTime.TotalSeconds;
            Task.Run(() => {
                
                outputDevice.Init(audioFile);
                outputDevice.Play(); // Воспроизводим

            });
            headerBlock.Text = audioPathsShort[selectedIndex];
            audioFile.
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                slider.Value = audioFile.CurrentTime.Seconds;
            }
        }
        // пауза
        void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
            }
        }
        // остановка
        void Stop_Click(object sender, RoutedEventArgs e)
        {
            outputDevice.Stop();
        }
        // если открытие файла завершилось с ошибкой
        void Media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            headerBlock.Text = "Ошибка открытия файла";
        }
        // открытие файла
        void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            //headerBlock.Text = myMediaElement.Name;
        }
        // окончание воспроизведения
        void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            //headerBlock.Text = "Воспроизведение завершено";
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
                outputDevice.Stop();
                int selectedIndex = audioList.SelectedIndex;

                outputDevice.Init(new AudioFileReader(audioPathsFull[selectedIndex]));
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int selectedIndex = audioList.SelectedIndex;
            outputDevice.Stop();
            AudioFileReader moved = new AudioFileReader(audioPathsFull[selectedIndex]);
            moved.CurrentTime = moved.CurrentTime.Add(TimeSpan.FromSeconds(10));
            outputDevice.Init(moved);
            outputDevice.Play();
        }
    }
}