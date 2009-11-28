using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace DigiFlare.DigiTweet.UI
{

    public class CategoryTabItem : TabItem, IDisposable
    {
        #region Instance Variables

        private string _name;
        private TweetsPanelControl _tweetsPanel;
        private Button _closeButton;

        #endregion

        #region Events

        public event RoutedEventHandler EditClick;
        public event RoutedEventHandler Close;

        #endregion

        #region Constructor

        public CategoryTabItem(string name, double headerWidth, double headerHeight, Brush headerTextBrush, TweetsPanelControl tweetsPanel)
            : base()
        {
            // keep references to useful fields
            _name = name;
            _tweetsPanel = tweetsPanel;

            // create tab's context menu
            ContextMenu = new ContextMenu();
            MenuItem edit = new MenuItem { Header = "Edit" };
            edit.Click += new RoutedEventHandler(OnEditClick);
            ContextMenu.Items.Add(edit);

            // setup tab header's grid
            Grid headerGrid = new Grid
            {
                Width = headerWidth,
                Height = headerHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent,
                ToolTip = "Right click to edit"
            };
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // close category name button
            _closeButton = new Button
            {
                Content = new TextBlock { Text = "X", Foreground = headerTextBrush },
                Width = 15,
                Height = 15,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Cursor = Cursors.Hand,
                ToolTip = "Remove Category"
            };
            _closeButton.Click += new RoutedEventHandler(OnClose);
            Grid.SetColumn(_closeButton, 1);

            // category name textblock
            TextBlock label = new TextBlock
            {
                Text = name,
                Foreground = headerTextBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(label, 0);

            // setup tab
            headerGrid.Children.Add(label);
            headerGrid.Children.Add(_closeButton);
            Header = headerGrid;
            Content = tweetsPanel;
        }

        #endregion

        #region Event Handlers

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (null != EditClick)
            {
                EditClick(this, e);
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            if (null != Close)
            {
                Close(this, e);
            }
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        public TweetsPanelControl TweetsPanel
        {
            get { return _tweetsPanel; }
        }

        public Style Style
        {
            get { return base.Style; }
            set { base.Style = value; }
        }

        public Style CloseButtonStyle
        {
            get { return _closeButton.Style; }
            set { _closeButton.Style = value; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _tweetsPanel.Dispose();
            _tweetsPanel = null;
        }

        #endregion
    }
}