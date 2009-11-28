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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        #region Instance Variables

        DoubleAnimationUsingKeyFrames _animation;

        #endregion

        #region Constructor

        public AlertWindow()
        {
            InitializeComponent();

            // init animation
            SplineDoubleKeyFrame frame1 = new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.Parse("0:0:0")));
            SplineDoubleKeyFrame frame2 = new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Parse("0:0:1")));
            SplineDoubleKeyFrame frame3 = new SplineDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.Parse("0:0:1.5")));
            _animation = new DoubleAnimationUsingKeyFrames();
            _animation.KeyFrames.Add(frame1);
            _animation.KeyFrames.Add(frame2);
            _animation.KeyFrames.Add(frame3);
            _animation.AutoReverse = true;
            _animation.FillBehavior = FillBehavior.Stop;
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(AlertWindow),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
            "User",
            typeof(User),
            typeof(AlertWindow),
            new PropertyMetadata(null)
        );

        #endregion

        #region Properties

        public string Message
        {
            get
            {
                return GetValue(MessageProperty) as string;
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public string AvatarUrl
        {
            set
            {
                imgAvatar.Source = new BitmapImage(new Uri(value));
            }
        }

        public User User
        {
            get
            {
                return GetValue(UserProperty) as User;
            }
            set
            {
                SetValue(UserProperty, value);
            }
        }

        #endregion

        #region UI Events

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            base.Hide();
        }

        #endregion

        #region Public Methods

        new public void Show()
        {
            base.Show();
            BeginAnimation(OpacityProperty, _animation);
        }

        #endregion
    }
}
