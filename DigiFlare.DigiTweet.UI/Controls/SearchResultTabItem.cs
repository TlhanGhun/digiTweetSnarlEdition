using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace DigiFlare.DigiTweet.UI
{
    public class SearchResultTabItem : TabItem, IDisposable
    {
        #region Instance Variables

        private string _name;
        private Button _closeButton;
        private SearchResultsPanelControl _searchResultsPanel;

        #endregion

        #region Events

        public event RoutedEventHandler Close;

        #endregion

        #region Constructor

        public SearchResultTabItem(string name, double headerWidth, double headerHeight, Brush headerTextBrush, SearchResultsPanelControl searchResultsPanel)
            : base()
        {
            // keep references to useful fields
            _name = name;
            _searchResultsPanel = searchResultsPanel;

            // setup tab header's grid
            Grid headerGrid = new Grid
            {
                Width = headerWidth,
                Height = headerHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
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
                ToolTip = "Close Search"
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
            Content = searchResultsPanel;
        }

        #endregion

        #region Event Handlers

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

        public SearchResultsPanelControl SearchResultsPanel
        {
            get { return _searchResultsPanel; }
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
            _searchResultsPanel.Dispose();
            _searchResultsPanel = null;
        }

        #endregion
    }
}
