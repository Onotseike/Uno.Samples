using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnoOnnxSamples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            contentFrame.Navigate(typeof(Views.PageOne));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            var selectedTag = (string)selectedItem.Tag;
            switch (selectedTag)
            {
                case "PageOne":
                    contentFrame.Navigate(typeof(Views.PageOne));
                    break;
                case "PageTwo":
                    contentFrame.Navigate(typeof(Views.PageTwo));
                    break;
                case "PageThree":
                    contentFrame.Navigate(typeof(Views.PageThree));
                    break;
                case "PageFour":
                    contentFrame.Navigate(typeof(Views.PageFour));
                    break;

                default:
                    contentFrame.Navigate(typeof(Views.PageOne));
                    break;
            }
        }
    }
}
