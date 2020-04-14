using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootloaderFlashUtility.Models
{
    public class TargetCommunicationLogic : BindableBase
    {

        #region Private Field
        /// <summary>
        /// Boolean for target connection
        /// </summary>  
        private bool _isTargetConnected;
        /// <summary>
        /// Logger class for simple logging
        /// </summary>
        private Logger _logger;
        /// <summary>
        /// Serial port oject to connect to target via serial port
        /// </summary>
        private SerialPort _serialPort;
   

        #endregion

        #region Public Enumeration
        public enum BootloaderCommand
        {
            COMMAND_BL_GET_VER               = 0x51,
            COMMAND_BL_GET_HELP              = 0x52,
            COMMAND_BL_GET_CID               = 0x53,
            COMMAND_BL_GET_RDP_STATUS        = 0x54,
            COMMAND_BL_GO_TO_ADDR            = 0x55,
            COMMAND_BL_FLASH_ERASE           = 0x56,
            COMMAND_BL_MEM_WRITE             = 0x57,
            COMMAND_BL_EN_R_W_PROTECT        = 0x58,
            COMMAND_BL_MEM_READ              = 0x59,
            COMMAND_BL_READ_SECTOR_P_STATUS  = 0x5A,
            COMMAND_BL_OTP_READ              = 0x5B,
            COMMAND_BL_DIS_R_W_PROTECT       = 0x5C,
        };

        public enum BootloaderCommandsLenght
        {
            COMMAND_BL_GET_VER_lEN              = 6,
            COMMAND_BL_GET_HELP_LEN             = 6,
            COMMAND_BL_GET_CID_LEN              = 6,
            COMMAND_BL_GET_RDP_STATUS_LEN       = 6,
            COMMAND_BL_GO_TO_ADDR_LEN           = 10,
            COMMAND_BL_FLASH_ERASE_LEN          = 8,
            COMMAND_BL_EN_R_W_PROTECT_LEN       = 6,
            COMMAND_BL_READ_SECTOR_P_STATUS_LEN = 6,
            COMMAND_BL_DIS_R_W_PROTECT_LEN      = 6,
        };
        #endregion
        #region Private Enumeration
        /// <summary>
        /// Possible responses from the target
        /// </summary>
        private enum TargetResponse
        {
            ACK                 = 0xA5,
            NACK                = 0x7F,
            FLASH_HAL_OK        = 0x00,
            FLASH_HAL_ERROR     = 0x01,
            FLASH_HAL_BUSY      = 0x02,
            FLASH_HAL_TIMEOUT   = 0x03,
            FLASH_HAL_INV_ADDR  = 0x04

        };

        #endregion

        #region Public Fields
        /// <summary>
        /// Boolean for target connection
        /// </summary>
        public bool IsTargetConnected
        {
            get { return _isTargetConnected; }
            set { _isTargetConnected = value; }
        }

        /// <summary>
        /// Logger class for simple logging
        /// </summary>
        public Logger Logger
        {
            get { return _logger; }
            set { SetProperty(ref _logger, value); }
        }
        #endregion


        #region Public Functions
        /// <summary>
        /// Funtion to test connection to target
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baud"></param>

        public void TestConnection(string portName, int baud)
        {
            Logger.Clear();
            Logger.Log("Testing connection...");
            Logger.Log($"Port: {portName}    Baud: {baud}");

            TargetConnect(portName, baud);
            TargetDisconnect();
        }

        /// <summary>
        /// Returns a list of port names for the serial port
        /// </summary>
        /// <returns></returns>
        public List<string> GetPorts()
        {
            return new List<string>(SerialPort.GetPortNames().ToList());
        }

        /// <summary>
        /// Returns a list of the available bauds to connect to the target
        /// </summary>
        public List<int> GetBaudRates()
        {

            return new List<int>()
            {
                9600,
                115200
            };
        }

        /// <summary>
        /// Returns a list of the available bauds to connect to the target
        /// </summary>
        public List<string> GetCommand()
        {

            //return BootloaderCommand.GetValue(elem => nameof(elem));
            return new List<string>()
            {
               nameof(BootloaderCommand.COMMAND_BL_GET_VER),
               nameof(BootloaderCommand.COMMAND_BL_GET_HELP),
               nameof(BootloaderCommand.COMMAND_BL_GET_CID),
               nameof(BootloaderCommand.COMMAND_BL_GET_RDP_STATUS),
               nameof(BootloaderCommand.COMMAND_BL_GO_TO_ADDR),
               nameof(BootloaderCommand.COMMAND_BL_FLASH_ERASE),
               nameof(BootloaderCommand.COMMAND_BL_MEM_WRITE),
               nameof(BootloaderCommand.COMMAND_BL_EN_R_W_PROTECT),
               nameof(BootloaderCommand.COMMAND_BL_MEM_READ),
               nameof(BootloaderCommand.COMMAND_BL_READ_SECTOR_P_STATUS),
               nameof(BootloaderCommand.COMMAND_BL_OTP_READ),
               nameof(BootloaderCommand.COMMAND_BL_DIS_R_W_PROTECT)
            };
        }
        #endregion

        #region Constructor

        public TargetCommunicationLogic()
        {
            _serialPort = new SerialPort()
            {
                Parity = Parity.None,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = false
            };

            Logger = Logger.Instance;


        }


        #region Private Functions
        /// <summary>
        /// Connects to the target
        /// </summary>
        private void TargetConnect(string portName, int baud)
        {
            TargetDisconnect();
            _serialPort.PortName = portName;
            _serialPort.BaudRate = baud;
            _currentState = ProcessState.Connect;

            try
            {
                _serialPort.Open();
                _serialPort.DiscardInBuffer();
                _serialPort.DiscardOutBuffer();
                Logger.Log("Target connected");
                IsTargetConnected = true;
                _command = Command.Next_Sucess;
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to connect to target.");
                _command = Command.Next_Fail;
            }
        }

        /// <summary>
        /// Disconnects from the target device
        /// </summary>
        private void TargetDisconnect()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
                IsTargetConnected = false;
                Logger.Log("Disconnected from target.");
            }
        }
        #endregion

        #region State Machine Related
        /// <summary>
        /// Defines the states of the state machine
        /// </summary>
        private enum ProcessState
        {
            Connect,
            Hookup,
            Erase,
            Write,
            Check,
            Disconnect_Sucess,
            Disconnect_Failure,
        }

        /// <summary>
        /// Defines the possible state processes for the state machines
        /// </summary>
        private enum Command
        {
            Next_Sucess,   // Move to next state with success result
            Next_Fail,   // Move to the next state with failed result
        }

        /// <summary>
        /// The current state
        /// </summary>
        private ProcessState _currentState;
        /// <summary>
        /// Command to be executed 
        /// </summary>
        private Command _command;

        /// <summary>
        /// Table for FSM
        /// </summary>
        private Action[,] _stateAction;

        private void ExecuteState(Command command)
        {
            _stateAction[(int)_currentState, (int)command].Invoke();
        }
        #endregion
    }

    #endregion
}

