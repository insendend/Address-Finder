using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using AddressFinderClient.Models.Enums;
using Classes.Client;
using Classes.Client.Settings;
using Classes.Parser;
using Newtonsoft.Json;

namespace AddressFinderClient.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Fields

        private IClient _client;
        private AddressClientSettings _settings;

        #endregion

        #region Binding properties

        public ObservableCollection<string> Countries { get; set; }

        private string _ip;
        public string Ip
        {
            get => _ip;
            set
            {
                if (_ip == value) return;
                _ip = value;
                OnPropertyChanged("Ip");
            }
        }

        private int _port;
        public string Port
        {
            get => _port.ToString();
            set
            {
                if (_port.ToString() == value)
                    return;

                if (int.TryParse(value, out _port))
                    OnPropertyChanged("Port");

            }
        }

        public ServerState ServerState =>
            IsAvailable is true
                ? ServerState.Available
                : IsAvailable is false
                    ? ServerState.Unavailable
                    : ServerState.Unknown;

        private bool? _isAvailable;
        public bool? IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable == value) return;
                _isAvailable = value;
                OnPropertyChanged("IsAvailable");
                OnPropertyChanged("ServerState");
            }
        }

        private string _selectedCountry;
        public string SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry == value) return;
                _selectedCountry = value;
                OnPropertyChanged("SelectedCountry");
            }
        }

        private bool _isReady;
        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (_isReady == value) return;
                _isReady = value;
                OnPropertyChanged("IsReady");
            }
        }

        public ObservableCollection<Address> Addresses { get; set; }

        private int _postCode;
        public string PostCode
        {
            get => _postCode.ToString();
            set
            {
                if (_postCode.ToString() == value)
                    return;

                if (int.TryParse(value, out _postCode))
                    OnPropertyChanged("PostCode");
            }
        }

        public ICommand CheckCommand { get; }

        public ICommand FindCommand { get; }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            Countries = new ObservableCollection<string>(new []{ "Ukraine" });
            Addresses = new ObservableCollection<Address>();
            CheckCommand = new Command(CheckForServerAvailability);
            FindCommand = new Command(FindAddresses);
            InitDefaultSettings();
        }

        #endregion

        #region Methods

        private void InitDefaultSettings()
        {
            Ip = "127.0.0.1";
            Port = 3333.ToString();
        }

        private void CheckForServerAvailability()
        {
            IsReady = IsValidServer();
        }

        private bool IsValidServer()
        {
            _settings = new AddressClientSettings
            {
                Ip = _ip,
                Port = _port
            };

            _client = new AddressClient(_settings);

            try
            {
                IsAvailable = _client.Connect();
            }
            catch (Exception)
            {
                IsAvailable = false;
            }

            return IsAvailable ?? false;
        }

        private void FindAddresses()
        {
            try
            {
                IsReady = IsValidServer();

                if (!IsReady) return;

                Application.Current.Dispatcher.Invoke(() => Addresses?.Clear());

                var msgBytes = Encoding.UTF8.GetBytes(PostCode);

                _client.Send(msgBytes, 0, msgBytes.Length);

                var resBytes = _client.Receive();

                if (resBytes is null || resBytes.Length == 0)
                    throw new ArgumentException(nameof(PostCode));

                var resString = Encoding.UTF8.GetString(resBytes);
                var addresses = JsonConvert.DeserializeObject<List<Address>>(resString);

                if (addresses.Count == 0)
                {
                    MessageBox.Show($"Not found any information for {PostCode}");
                    return;
                }

                foreach (var address in addresses)
                    Application.Current.Dispatcher.Invoke(() => Addresses.Add(address));

                SaveToFile();
            }
            catch (ArgumentException)
            {
                MessageBox.Show(
                    $"Information for post code: '{PostCode}' not found",
                    "Not Found",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                _client?.Dispose();
            }
        }

        private void SaveToFile()
        {
            if (Addresses is null || Addresses.Count == 0)
                return;

            var filename = $"{_selectedCountry}[{PostCode}].xml";

            try
            {
                using (var stream = File.OpenWrite(filename))
                {
                    var serializer = new XmlSerializer(typeof(ObservableCollection<Address>));
                    serializer.Serialize(stream, Addresses);
                }

                MessageBox.Show(
                    $"Saved into the file: '{filename}'",
                    "Saving to xml-file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}
