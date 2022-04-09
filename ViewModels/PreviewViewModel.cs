using LibVLCSharp.Shared;
using MimeTypes;
using SoupMover.Commands.PreviewCommands;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SoupMover.ViewModels
{
    public class PreviewViewModel : ViewModelBase
    {
        private HomeViewModel HVM;
        private readonly DispatcherTimer Time;
        private LibVLC Lib;
        private MediaPlayer _Media;
        public bool IsDragging { get; set; }
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

        //Leaving this as a string because it doesn't need to be moved
        private string _VolumeImage;
        public string VolumeImage
        {
            get
            {
                return _VolumeImage;
            }
            set
            {
                _VolumeImage = value;
                OnPropertyChanged(nameof(VolumeImage));
            }
        }

        private BitmapImage _Gif;
        public BitmapImage Gif
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

        private BitmapImage _Image;
        public BitmapImage Image
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
                TimeLabel = TimeSpan.FromSeconds(Value).ToString(@"hh\:mm\:ss");
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
                _TimeLabel = value;
                OnPropertyChanged(nameof(TimeLabel));
            }
        }
        #endregion

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand VolumeCommand { get; set; }

        public PreviewViewModel()
        {
            HidePreviews();
            //Image = new BitmapImage();
            //Gif = new BitmapImage();
            VolumeImage = @"../Images/volume-up.png";
            Time = new DispatcherTimer();
            Time.Interval = TimeSpan.FromSeconds(1);
            Time.Tick += Time_Tick;

            Core.Initialize();
            Lib = new LibVLC();
            Media = new MediaPlayer(Lib);
            PlayCommand = new PlayCommand(Media);
            PauseCommand = new PauseCommand(Media);
            VolumeCommand = new VolumeCommand(this, Media);
        }

        private void Time_Tick(object sender, EventArgs e)
        {
            if (!IsDragging)
            {
                Minimum = 0;
                Maximum = Media.Length / 1000; //The length of the media file in seconds
                //TODO: Media has a built in timechanged event; maybe try to use that instead of DispatcherTimer?
                Value = Media.Time / 1000;
            }

        }
        private BitmapImage LoadBitmapImage(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // just in case you want to load the image in another thread
                return bitmapImage;
            }
        }
        private void HidePreviews()
        {
            ImageVisible = false;
            GifVisible = false;
            PlayerVisible = false;
            TextVisible = false;
            if (Media != null)
                Media.Stop();
            ErrorVisible = false;
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
                if (HVM.SelectedFile == "") //In event we just removed a file(s)
                    HidePreviews();
                if (File.Exists(HVM.SelectedFile))
                {
                    HidePreviews();
                    Uri uri = new Uri(HVM.SelectedFile);
                    if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("image"))
                    {
                        if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("gif"))
                        {
                            GifVisible = true;
                            Gif = LoadBitmapImage(HVM.SelectedFile);
                        }
                        else
                        {
                            ImageVisible = true;
                            Image = LoadBitmapImage(HVM.SelectedFile);
                        }

                    }
                    else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("video") || MimeTypeMap.GetMimeType(uri.ToString()).Contains("audio"))
                    {
                        PlayerVisible = true;
                        var media = new Media(Lib, uri);
                        Maximum = media.Duration / 1000;
                        Value = 0;
                        TimeLabel = TimeSpan.FromSeconds(0).ToString(@"hh\:mm\:ss");
                        Time.Start();
                        Media.Volume = 100;
                        Media.Play(media);


                    }
                    else if (MimeTypeMap.GetMimeType(uri.ToString()).Contains("text"))
                    {
                        try
                        {
                            TextVisible = true;
                            Text = File.ReadAllText(HVM.SelectedFile);
                        }
                        catch (Exception)
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
