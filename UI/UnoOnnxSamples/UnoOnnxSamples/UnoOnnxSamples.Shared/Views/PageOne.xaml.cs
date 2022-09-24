using System;
using Microsoft.UI.Xaml.Controls;
using UnoOnnxSamples.Models;
using System.Threading.Tasks;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UnoOnnxSamples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageOne : Page
    {
        MobileOnnxImgaeClassifier _classifier;
        public string[] EmbeddedResources { get; } = typeof(MainPage).Assembly.GetManifestResourceNames();
        public PageOne()
        {
            this.InitializeComponent();
            this._classifier = new MobileOnnxImgaeClassifier();
            foreach (var item in EmbeddedResources)
            {
                Console.WriteLine(item);
            }
        }

        async Task RunInferenceAsync()
        {
            RunButton.IsEnabled = false;
            try
            {
                var sampleImage = await _classifier.GetSampleImageAsync();
                var result = await _classifier.GetClassificationAsync(sampleImage);

                var dialog = new ContentDialog();
                dialog.Content = result;
                dialog.CloseButtonText = "Done";

                var dialogResult = await dialog.ShowAsync();

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                RunButton.IsEnabled = true;
            }
        }

        private async void RunButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await RunInferenceAsync();
    }
}
