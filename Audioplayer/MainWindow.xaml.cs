using Microsoft.Win32;
using System.IO.Enumeration;
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
        String filePath;
        List<String> audioPathsFull = new List<String>();
        List<String> audioPathsShort = new List<String>();
    
        public MainWindow()
        {
            InitializeComponent();
        }

        // начало воспроизведения
        void Play_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Play();
        }
        // пауза
        void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (myMediaElement.CanPause)
                myMediaElement.Pause();
        }
        // остановка
        void Stop_Click(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }
        // если открытие файла завершилось с ошибкой
        void Media_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            headerBlock.Text = "Ошибка открытия файла";
        }
        // открытие файла
        void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            headerBlock.Text = myMediaElement.Name;
        }
        // окончание воспроизведения
        void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            headerBlock.Text = "Воспроизведение завершено";
        }

        private void chooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                audioPathsShort.Clear();
                audioPathsFull.Clear();

                foreach (string safeFileName in openFileDialog.SafeFileNames)
                {
                    audioPathsShort.Add(safeFileName);
                    audioList.Items.Add(safeFileName);
                }

                foreach (string fileName in openFileDialog.FileNames)
                {
                    audioPathsFull.Add(fileName);
                }
                
                if (audioPathsShort.Count > 0 && audioPathsFull.Count>0)
                {
                    myMediaElement.Source = new Uri(audioPathsFull[0]);
                    headerBlock.Text = audioPathsShort[0];
                }
                else
                {
                    MessageBox.Show("Файлы отсутствуют");
                }
            }
            else
            {
                MessageBox.Show("Файл не выбран");

            }
        }

        // Выбор файла из списка
        private void audioList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = audioList.SelectedIndex;
            headerBlock.Text = audioPathsShort[selectedIndex];
            myMediaElement.Source = new Uri(audioPathsFull[selectedIndex]);
            //myMediaElement.Stop();
        }
    }
}