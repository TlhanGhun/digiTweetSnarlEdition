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
    /// Interaction logic for TweetsPanelControl.xaml
    /// </summary>
    public partial class TweetsPanelControl : UserControl, IDisposable
    {
        #region Dependency Properties

        public static readonly DependencyProperty IsShowMoreButtonProperty = DependencyProperty.Register(
            "IsShowMoreButton",
            typeof(Visibility),
            typeof(TweetsPanelControl),
            new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Events

        public event MouseButtonEventHandler AvatarClicked;
        public event RoutedEventHandler TweetTextLoaded;
        public event RoutedEventHandler ReplyClicked;
        public event RoutedEventHandler DirectMessageClicked;
        public event RoutedEventHandler RetweetClicked;
        public event RoutedEventHandler DeleteClicked;
        public event RoutedEventHandler MoreClicked;
        //public event RoutedEventHandler FavouriteChecked;
        //public event RoutedEventHandler FavouriteUnchecked;
        public event RoutedEventHandler FavouriteClicked;

        #endregion

        #region Constructor

        public TweetsPanelControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public IEnumerable ItemsSource
        {
            get
            {
                return icTweets.ItemsSource;
            }
            set
            {
                icTweets.ItemsSource = value;
            }
        }

        public Visibility IsShowMoreButton
        {
            get
            {
                return (Visibility)GetValue(IsShowMoreButtonProperty);
            }
            set
            {
                SetValue(IsShowMoreButtonProperty, value);
            }
        }

        #endregion

        #region UI Events

        void Avatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (null != AvatarClicked)
            {
                AvatarClicked(sender, e);
            }
        }

        void TweetText_Load(object sender, RoutedEventArgs e)
        {
            if (null != TweetTextLoaded)
            {
                TweetTextLoaded(sender, e);
            }
        }

        void Reply_Click(object sender, RoutedEventArgs e)
        {
            if (null != ReplyClicked)
            {
                ReplyClicked(sender, e);
            }
        }

        void DirectMessage_Click(object sender, RoutedEventArgs e)
        {
            if (null != DirectMessageClicked)
            {
                DirectMessageClicked(sender, e);
            }
        }

        void Retweet_Click(object sender, RoutedEventArgs e)
        {
            if (null != RetweetClicked)
            {
                RetweetClicked(sender, e);
            }
        }

        void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (null != DeleteClicked)
            {
                DeleteClicked(sender, e);
            }
        }

        void More_Click(object sender, RoutedEventArgs e)
        {
            if (null != MoreClicked)
            {
                MoreClicked(sender, e);
            }
        }

        //void Favourite_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (null != FavouriteChecked)
        //    {
        //        FavouriteChecked(sender, e);
        //    }
        //}

        //void Favourite_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    if (null != FavouriteUnchecked)
        //    {
        //        FavouriteUnchecked(sender, e);
        //    }
        //}

        void Favourite_Click(object sender, RoutedEventArgs e)
        {
            if (null != FavouriteClicked)
            {
                FavouriteClicked(sender, e);
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