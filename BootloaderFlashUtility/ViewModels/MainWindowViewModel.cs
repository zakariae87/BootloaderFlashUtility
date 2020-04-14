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
using BootloaderFlashUtility.Models;
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

        /// <summary>
        /// The Selected baud rate Value
        /// </summary>
        private string _selectedBootCommands;

        /// <summary>
        /// Instance of the Target Flash Logic class
        /// </summary>
        private TargetCommunicationLogic _targetCommunicationLogic;

        /// <summary>
        /// Boolean for target connection established
        /// </summary>
        private bool _isTargetConnected = false;

        /// <summary>
        /// Whether the connect button is enabled or not 
        /// </summary>
        private bool _isTestButtonConnectEnabled = true;

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
        /// <summary>
        /// Instance of the Target Flash Logic class
        /// </summary>
        public TargetCommunicationLogic TargetCommunicationLogic
        {
            get { return _targetCommunicationLogic; }
            set { SetProperty(ref _targetCommunicationLogic, value); }
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
        /// List of the available baud rates
        /// </summary>
        public List<string> BootCommands { get; set; }
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
                TestConnectCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Selected Com Port
        /// </summary>
        public string SelectedBootCommands
        {
            get { return _selectedBootCommands; }
            set
            {
                SetProperty(ref _selectedBootCommands, value);
            }
        }
        #endregion
        #endregion

        #region UI Buttons
        /// <summary>
        /// Whether the connect button is enabled or not 
        /// </summary>
        public bool TestButtonConnectIsEnabled
        {
            get { return _isTestButtonConnectEnabled; }
            set
            {
                SetProperty(ref _isTestButtonConnectEnabled, value);
            }
        }

        /// <summary>
        /// Associate Delegate Command to "Testconnect" Button
        /// </summary>
        public DelegateCommand TestConnectCommand { get; private set; }
        #endregion


        #region Constructors
        public MainWindowViewModel() 
        {
            TargetCommunicationLogic = new TargetCommunicationLogic();
            ComPorts                 = TargetCommunicationLogic.GetPorts();
            BaudRates                = TargetCommunicationLogic.GetBaudRates();
            BootCommands             = TargetCommunicationLogic.GetCommand();

            #region Delegate Commands
            TestConnectCommand = new DelegateCommand(TestConnectCommandExecute, TestConnectCommandCanExecute);
            
            #endregion

        }
        #region
        private bool TestConnectCommandCanExecute()
        {
            if (string.IsNullOrEmpty(SelectedComPort))
                return false;

            return true;
        }

        private void TestConnectCommandExecute()
        {
            TargetCommunicationLogic.TestConnection(SelectedComPort, SelectedBaudRate);
        }
        #endregion

        #endregion

    }
}
