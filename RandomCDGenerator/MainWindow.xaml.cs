using RandomCDGenerator.Engine;
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

namespace RandomCDGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mp3FilePathProvider filePathProvider = new Mp3FilePathProvider();
        private TrackList tracklist;
        private Mp3FileInfo suggestion;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnScandir_Click(object sender, RoutedEventArgs e)
        {
            btnScandir.IsEnabled = false;
            lblDirectoryStatus.Text = "Läser katalog...";

            try
            {
                await filePathProvider.Init(txtMusicDir.Text);
                lblDirectoryStatus.Text = filePathProvider.FileCount.ToString() + " filer tillgängliga";
            }
            catch (Exception ex)
            {
                lblDirectoryStatus.Text = ex.Message;
            }

            btnScandir.IsEnabled = true;
        }

        private void btnNewTracklist_Click(object sender, RoutedEventArgs e)
        {
            tracklist = new TrackList(70 * 60);
            SuggestNextTrack();
        }

        private void SuggestNextTrack()
        {
            var timeSpan = TimeSpan.FromSeconds(tracklist.TimeRemaining);
            txtRemaining.Text = timeSpan.TotalMinutes.ToString() + " minutes remain";
            try
            {
                suggestion = filePathProvider.Next();

                if(suggestion.TaglibFile.Tag.AlbumArtists.Count() > 0)
                {
                    txtArtist.Text = suggestion.TaglibFile.Tag.JoinedAlbumArtists;
                }
                else
                {
                    txtArtist.Text = suggestion.TaglibFile.Tag.JoinedPerformers;
                }
                
                txtTitle.Text = suggestion.TaglibFile.Tag.Title;
                lblPath.Text = suggestion.Path;
                grdSelectTrack.Visibility = Visibility.Visible;
                lblDirectoryStatus.Text = filePathProvider.FileCount.ToString() + " filer tillgängliga";
            }
            catch (NoMoreFilesException)
            {
                MessageBox.Show("No more files available");
                grdSelectTrack.Visibility = Visibility.Hidden;
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            tracklist.Add(suggestion);
            SuggestNextTrack();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            SuggestNextTrack();
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var exporter = new M3UCreator(tracklist);

            this.IsEnabled = false;

            try
            {
                await exporter.Write(txtM3UPath.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.IsEnabled = true;
        }
    }
}
