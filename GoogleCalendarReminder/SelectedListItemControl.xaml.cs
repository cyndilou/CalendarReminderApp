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

namespace GoogleCalendarReminder
{
    /// <summary>
    /// Interaction logic for SelectedListItemControl.xaml
    /// </summary>
    public partial class SelectedListItemControl : UserControl
    {
        #region Events

        public static readonly RoutedEvent ItemChangedEvent = EventManager.RegisterRoutedEvent("ItemChanged"
            , RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SelectedListItemControl));

        public event RoutedEventHandler ItemChanged
        {
            add { AddHandler(ItemChangedEvent, value); }
            remove { RemoveHandler(ItemChangedEvent, value); }
        }

        void RaiseItemChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SelectedListItemControl.ItemChangedEvent);
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region Properties

        public CalendarEvent CalendarEvent
        {
            get { return DataContext as CalendarEvent; }
            set { DataContext = value; }
        }

        #endregion

        #region Constructor

        public SelectedListItemControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
