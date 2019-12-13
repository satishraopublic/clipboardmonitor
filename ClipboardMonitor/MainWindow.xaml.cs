using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClipboardMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler ClipboardUpdated;
        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private IntPtr hWnd = IntPtr.Zero;




        public ObservableCollection<ClipboardContent> ClipboardContents
        {
            get { return (ObservableCollection<ClipboardContent>)GetValue(ClipboardContentsProperty); }
            set { SetValue(ClipboardContentsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClipboardContents.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClipboardContentsProperty =
            DependencyProperty.Register("ClipboardContents", typeof(ObservableCollection<ClipboardContent>), typeof(MainWindow), null);




        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Init();
        }

        private void Init()
        {
            ClipboardContents = new ObservableCollection<ClipboardContent>();
            WindowInteropHelper h = new WindowInteropHelper(this);
            hWnd = h.EnsureHandle();
            HwndSource.FromHwnd(hWnd)?.AddHook(WndProc);
            Start();
        }

        public IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if(msg == WM_CLIPBOARDUPDATE)
            {
                if (Clipboard.ContainsAudio())
                {
                    var audio = Clipboard.GetAudioStream();
                    ClipboardContents.Add(new ClipboardContent("Audio Content", audio));
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    var fileList = Clipboard.GetFileDropList();
                    ClipboardContents.Add(new ClipboardContent("File list Dropped", fileList));
                }
                else if (Clipboard.ContainsImage())
                {
                    //IDataObject ob = Clipboard.GetDataObject();
                    //var formats = ob.GetFormats();
                    BitmapSource img = Clipboard.GetImage();
                    ClipboardContents.Add(new ClipboardContent("Image", img));
                    //ClipboardContents.Add(new ClipboardContent("Image", image));
                }
                else if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText();
                    ClipboardContents.Add(new ClipboardContent("Text", text));
                }
                else
                {
                    //ClipboardContents.Add(new ClipboardContent("Data", new object()));
                }
            }
            handled = false;
            return IntPtr.Zero;
        }

         
     
    private BitmapImage BitmapFromSource(BitmapSource bitmapsource)
        {
            BitmapImage bitmap = new BitmapImage();
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                outStream.Position = 0;
                bitmap.BeginInit();
                bitmap.StreamSource = outStream;
                bitmap.EndInit();
            }
            return bitmap;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        public void Start()
        {
            NativeMethods.AddClipboardFormatListener(hWnd);
        }
        public void Stop()
        {
            NativeMethods.RemoveClipboardFormatListener(hWnd);
        }
    }
}
