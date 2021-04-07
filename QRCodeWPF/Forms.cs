using Microsoft.Win32;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QRCodeWPF
{
    public class Forms : INotifyPropertyChanged
    {
        public Forms()
        {
            Type = new ObservableCollection<string> { "URL", "Text" };
            Item = Type[0];
        }
        public ObservableCollection<string> Type { get; set; }
        public string Item { get; set; }

        public string InputData { get; set; }

        private bool Image = false;
        public bool ImageEnabled
        {
            get => Image;
            set
            {
                Image = value;
                OnPropertyChanged("ImageEnabled");
            }
        }

        public ImageSource QrCode;
        public ImageSource QRCode
        {
            get => QrCode;
            set
            {
                QrCode = value;
                OnPropertyChanged();
            }
        }

        private Bitmap _QRCode;

        private ImageSource Generate(string text)
        {
            if (text.Length == 0) return null;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData;

            switch (Item)
            {
                case "URL":
                    PayloadGenerator.Url generator = new PayloadGenerator.Url(text);
                    qrCodeData = qrGenerator.CreateQrCode(generator.ToString(), QRCodeGenerator.ECCLevel.Q);
                    break;
                case "Text":
                    qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    break;
                default:
                    qrCodeData = null;
                    break;
            }

            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            _QRCode = qrCodeImage;
            var handle = qrCodeImage.GetHbitmap();

            ImageEnabled = true;

            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void SaveQRCode()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JPEG Image (.jpeg)|*.jpeg "
            };

            if (dialog.ShowDialog() == null) return;

            _QRCode.Save(dialog.FileName, ImageFormat.Jpeg);
        }
        public RelayCommand GenerateButtonClick => new RelayCommand(obj => { QRCode = Generate(InputData); });
        public RelayCommand SaveQRCodeButton => new RelayCommand(obj => { SaveQRCode(); });

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
