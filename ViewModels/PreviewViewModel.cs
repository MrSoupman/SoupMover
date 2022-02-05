using LibVLCSharp.Shared;
using MimeTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;

namespace SoupMover.ViewModels
{
    public class PreviewViewModel : ViewModelBase
    {
        private HomeViewModel HVM;
        private readonly DispatcherTimer Time;
        private LibVLC Lib;
        private MediaPlayer _Media;
        public MediaPlayer Media
        {
            get
            {
                return _Media;
            }
            set
            {
                _Media = value;
                OnPropertyChanged(nameof(Media));
            }
        }

        private string _Gif;
        public string Gif
        {
            get 
            {
                return _Gif;
            }
            set
            {
                _Gif = value;
                OnPropertyChanged(nameof(Gif));
            }
        }

        private string _Image;
        public string Image
        {
            get
            {
                return _Image;
            }
            set
            {
                _Image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        #region Visibilities
        private bool _ImageVisible;
        public bool ImageVisible
        {
            get
            {
                return _ImageVisible;
            }
            set
            {
                _ImageVisible = value;
                OnPropertyChanged(nameof(ImageVisible));
            }
        }
        private bool _GifVisible;
        public bool GifVisible
        {
            get
            {
                return _GifVisible;
            }
            set
            {
                _GifVisible = value;
                OnPropertyChanged(nameof(GifVisible));
            }
        }
        private bool _PlayerVisible;
        public bool PlayerVisible
        {
            get
            {
                return _PlayerVisible;
            }
            set
            {
                _PlayerVisible = value;
                OnPropertyChanged(nameof(PlayerVisible));
            }
        }
        private bool _TextVisible;
        public bool TextVisible
        {
            get
            {
                return _TextVisible;
            }
            set
            {
                _TextVisible = value;
                OnPropertyChanged(nameof(TextVisible));
            }
        }
        private bool _ErrorVisible;
        public bool ErrorVisible
        {
            get
            {
                return _ErrorVisible;
            }
            set
            {
                _ErrorVisible = value;
                OnPropertyChanged(nameof(ErrorVisible));
            }
        }
        #endregion

        #region Slider
        private int _Minimum;
        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                _Minimum = value;
                OnPropertyChanged(nameof(Minimum));
            }
        }
        private long _Maximum;
        public long Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                _Maximum = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }
        private long _Value;
        public long Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private string _TimeLabel;
        public string TimeLabel
        {
            get 
            {
                return _TimeLabel;
            }
            set
            {
                _TimeLabel = TimeSpan.FromSeconds(Value).ToString(@"hh\:mm\:ss");
                OnPropertyChanged(nameof(TimeLabel));
            }
        }
        #endregion
        public PreviewViewModel()
        {
            HidePreviews();
            Time = new DispatcherTimer();
            Time.Tick += Time_Tick;
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            Minimum = 0;
            Maximum = Media.Length / 1000;
            Value = Media.Time / 1000;
        }

        private void HidePreviews()
        {
            ImageVisible = false;
            GifVisible = false;
            PlayerVisible = false;
            TextVisible = false;
            if (Media != null)
                Media.Stop();
        }

        public void InitVidPreview()
        {
            Core.Initialize();
            Lib = new LibVLC();
            Media = new MediaPlayer(Lib);
        }

        public void SetMediaTime()
        { 
            Media.Time = (long)TimeSpan.FromSeconds(Value).TotalMilliseconds;
        }

        public void SetHomeViewModel(HomeViewModel HVM)
        {
            this.HVM = HVM;
            HVM.PropertyChanged += HVM_PropertyChanged;
            
        }

        private void HVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HomeViewModel.SelectedFile))
            {
                
                if (File.Exists(HVM.SelectedFile))
                {
                    HidePreviews();
                    Uri uri = new Uri(HVM.SelectedFile);
                    if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("image"))
                    {
                        if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("gif"))
                        {
                            GifVisible = true;
                            Gif = HVM.SelectedFile;
                        }
                        else
                        {
                            ImageVisible = true;
                            Image = HVM.SelectedFile;
                        }

                    }
                    else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("video") || MimeTypeMap.GetMimeType(uri.ToString()).Contains("audio"))
                    {
                        PlayerVisible = true;
                        //previewGrid.BringIntoView();
                        Time.Start();
                        Media.Play(new Media(Lib, uri));
                        Media.Volume = 100;

                    }
                    else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("text"))
                    {
                        try
                        {
                            TextVisible = true;
                            Text = File.ReadAllText(HVM.SelectedFile);
                        }
                        catch (Exception exc)
                        {
                            ErrorVisible = true;
                        }
                    }
                    else
                    {
                        ErrorVisible = true;
                    }
                }
            }
        }
    }
}
