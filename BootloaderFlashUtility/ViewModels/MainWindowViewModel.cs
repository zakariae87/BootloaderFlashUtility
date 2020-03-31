using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Prism.Commands;
using System;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
//using BootloaderFlashUtility.Models;
using System.Windows.Data;
using log4net;

namespace BootloaderFlashUtility.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Private Members
        /// <summary>
        /// Title of the main window
        /// </summary>
        private string _title = " Bootloader Flash Utility ";
        /// <summary>
        /// The Selected Comport Value
        /// </summary>
        private string _selectedComPort;

        /// <summary>
        /// The Selected baud rate Value
        /// </summary>
        private int _selectedBaudRate;

        #endregion

        #region Public Fields
        /// <summary>
        /// Title of the main window
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        #region Com Port and Baud Rate and Bootloader Commands
        /// <summary>
        /// List of the available com ports
        /// </summary>
        public List<string> ComPorts { get; set; }
        /// <summary>
        /// List of the available baud rates
        /// </summary>
        public List<int> BaudRates { get; set; }
        /// <summary>
        /// Selected Baud Rate
        /// </summary>
        public int SelectedBaudRate
        {
            get { return _selectedBaudRate; }
            set
            {
                SetProperty(ref _selectedBaudRate, value);
            }
        }
              
        /// <summary>
        /// Selected Com Port
        /// </summary>
        public string SelectedComPort
        {
            get { return _selectedComPort; }
            set
            {
                SetProperty(ref _selectedComPort, value);
            }
        }
        #endregion

        #endregion

    }
}
