using JetBrains.Annotations;
using NPOI.SS.Formula.PTG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PracticalControls.Demo.Views
{
    /// <summary>
    /// Interaction logic for Waiting.xaml
    /// </summary>
    public partial class Waiting : Window
    {
        int dotNumber = 0;
        bool isStopping = false;

        public Waiting()
        {
            InitializeComponent();
            this.Loaded += Waiting_Loaded;
        }

        private void Waiting_Loaded(object sender, RoutedEventArgs e)
        {
            StartDotting();
        }

        private void StartDotting()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(300);
                    if (isStopping)
                        return;

                    dotNumber++;
                    dotNumber %= 5;
                    string dotStr = new string('.', dotNumber);
                    this.Dispatcher.Invoke(() =>
                    {
                        this.tbDot.Text = $"{dotStr}";
                    });
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        private void StopDotting()
        {
            isStopping = true;
        }

        #region Static 


        private static Thread StatusThread = null;
        private static Waiting PopupWin = null;

        private static SynchronizationContext MainSyncContext;
        private static WindowInteropHelper InteropHelper;
        private static IntPtr MainWinPtr;
        private static Dispatcher MainDispatcher;


        public static void Start()
        {
            InteropHelper = new WindowInteropHelper(Application.Current.MainWindow);
            MainWinPtr = InteropHelper.Handle;
            MainDispatcher = Application.Current.MainWindow.Dispatcher;
            MainSyncContext = SynchronizationContext.Current;

            Application.Current.MainWindow.CaptureMouse();
            var point = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
            double left = point.X;
            double top = point.Y;
            double width = Application.Current.MainWindow.ActualWidth;
            double height = Application.Current.MainWindow.ActualHeight;

            void SetPosition(Window win)
            {
                win.Left = left;
                win.Top = top;
                win.Width = width;
                win.Height = height;
            }

            //create the thread with its ThreadStart method
            StatusThread = new Thread(() =>
            {
                try
                {
                    PopupWin = new Waiting();
                    SetPosition(PopupWin);
                    PopupWin.Show();
                    PopupWin.Focusable = false;
                    PopupWin.Activated += PopupWin_Activated;
                    PopupWin.Closed += (lsender, le) =>
                    {
                        //when the window closes, close the thread invoking the shutdown of the dispatcher
                        PopupWin.Dispatcher.InvokeShutdown();
                        PopupWin = null;
                        StatusThread = null;
                        MainSyncContext = null;
                        InteropHelper = null;
                    };

                    //this call is needed so the thread remains open until the dispatcher is closed
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            });

            //run the thread in STA mode to make it work correctly
            StatusThread.IsBackground = true;
            StatusThread.SetApartmentState(ApartmentState.STA);
            StatusThread.Priority = ThreadPriority.Normal;
            StatusThread.Start();
        }

        private static void PopupWin_Activated(object sender, EventArgs e)
        {
            Window win = sender as Window;

            //MainDispatcher.BeginInvoke(new Action(() =>
            //{

            //}));

            //win.Dispatcher.Invoke(() =>
            //{
            //    Win32.User32.SetForegroundWindow(MainWinPtr);
            //    Win32.User32.SetActiveWindow(MainWinPtr);
            //    //Win32.User32.SetWindowPos(MainWinPtr, new IntPtr(0), 0, 0, 0, 0, Win32.User32.SWP_NOMOVE | Win32.User32.SWP_NOSIZE | Win32.User32.SWP_SHOWWINDOW | Win32.User32.SWP_NOACTIVATE);

            //});
        }

        public static void Stop()
        {
            if (PopupWin != null)
            {
                //need to use the dispatcher to call the Close method, because the window is created in another thread, and this method is called by the main thread
                PopupWin.Dispatcher.BeginInvoke(new Action(() =>
                {
                    PopupWin.StopDotting();
                    PopupWin.Close();
                }));
            }
        }

        #endregion

    }
}
