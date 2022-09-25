using System;
using Microsoft.UI.Xaml.Controls;
using UnoOnnxSamples.Models;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Linq;
using Windows.Media.Capture;
using System.IO;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoOnnxSamples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageTwo : Page
    {
        MobilePyTorchSuperResolution superResolution;
        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();
        
        public PageTwo()
        {
            this.InitializeComponent();
            this.superResolution = new MobilePyTorchSuperResolution();
            foreach (var item in EmbeddedResources)
            {
                Console.WriteLine(item);
            }
        }
        
        async Task RunSuperResolutionAsync()
        {
            RunButton.IsEnabled = false;
            try
            {
                var sampleImage = await superResolution.GetSampleImageAsync("fish.jpeg");
                var result = await superResolution.GetSuperResolutionImage(sampleImage, "fish.jpeg");

                var dialog = new ContentDialog();
                dialog.Content = result;
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();

            }
            catch (Exception exception)
            {

                var dialog = new ContentDialog();
                dialog.Content = $"ERROR:{exception.Message}";
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();
            }
            finally
            {
                RunButton.IsEnabled = true;
            }
        }

        private async void RunButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await RunSuperResolutionAsync();
    }
}
