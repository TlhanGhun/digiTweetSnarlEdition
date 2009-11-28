using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Collections;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for SearchResultsControls.xaml
    /// </summary>
    public partial class SearchResultsPanelControl : UserControl, IDisposable
    {
        #region Instance Variables

        private string _keyword;

        #endregion

        #region Events

        public event RoutedEventHandler TextLoaded;
        public event MouseButtonEventHandler AvatarClicked;

        public delegate void MoreSearchResultsClickedEventHandler(object sender, MoreSearchResultsClickedEventArgs e);
        public event MoreSearchResultsClickedEventHandler MoreResultsClicked;

        #endregion

        #region Dependency Property

        public static readonly DependencyProperty ShowMoreSearchResultsVisibilityProperty = DependencyProperty.Register(
            "ShowMoreSearchResultsVisibility",
            typeof(Visibility),
            typeof(UserControl),
            new PropertyMetadata(Visibility.Collapsed)
        );

        #endregion

        #region Constructor

        public SearchResultsPanelControl(string keyword)
        {
            InitializeComponent();
            _keyword = keyword;
        }

        #endregion

        #region UI Events Handlers

        void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != TextLoaded)
            {
                TextLoaded(sender, e);
            }
        }

        void MoreResults_Click(object sender, RoutedEventArgs e)
        {
            if (null != MoreResultsClicked)
            {
                MoreSearchResultsClickedEventArgs args = new MoreSearchResultsClickedEventArgs { Keyword = _keyword };
                MoreResultsClicked(sender, args);
            }
        }

        void imgAvatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (null != AvatarClicked)
            {
                AvatarClicked(sender, e);
            }
        }

        #endregion

        #region Properties

        public IEnumerable ItemsSource
        {
            get
            {
                return icSearchResults.ItemsSource;
            }
            set
            {
                icSearchResults.ItemsSource = value;
            }
        }

        public Visibility ShowMoreSearchResultsVisibility
        {
            get
            {
                return (Visibility)GetValue(ShowMoreSearchResultsVisibilityProperty);
            }
            set
            {
                SetValue(ShowMoreSearchResultsVisibilityProperty, value);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            ItemsSource = null;
        }

        #endregion
    }
}
