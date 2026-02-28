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
            String shortFileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                shortFileName = openFileDialog.SafeFileName;

            }
            myMediaElement.Source = new Uri(filePath);
            headerBlock.Text= shortFileName;
        }
    }
}