// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace LeavinsSoftware.Collection.Models
{
    /// <summary>
    /// Program options.
    /// </summary>
    public sealed class ProgramOptions :
        ValidatableBase, INotifyPropertyChanged, IEquatable<ProgramOptions>
    {
        public const string DefaultProxyServerAddress = "127.0.0.1";
        public const int DefaultProxyServerPort = 80;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor with default options.
        /// </summary>
        public ProgramOptions()
        {
            IsFirstRun = true;
            CheckForProgramUpdates = false;
            UseProxyServer = false;
            ProxyServerAddress = DefaultProxyServerAddress;
            ProxyServerPort = DefaultProxyServerPort;
        }

        /// <summary>
        /// Is this the first time that the user has launched the program?
        /// </summary>
        public bool IsFirstRun
        {
            get
            {
                return isFirstRun;
            }
            set
            {
                if (isFirstRun != value)
                {
                    isFirstRun = value;
                    OnPropertyChanged("IsFirstRun");
                }
            }
        }

        /// <summary>
        /// Should CIB check online for program updates?
        /// </summary>
        public bool CheckForProgramUpdates
        {
            get
            {
                return checkForUpdates;
            }
            set
            {
                if (checkForUpdates != value)
                {
                    checkForUpdates = value;
                    OnPropertyChanged("CheckForProgramUpdates");
                }
            }
        }

        public bool UseProxyServer
        {
            get
            {
                return useProxyServer;
            }
            set
            {
                if (useProxyServer != value)
                {
                    useProxyServer = value;
                    OnPropertyChanged("UseProxyServer");
                }
            }
        }

        [Required]
        public string ProxyServerAddress
        {
            get
            {
                return serverAddress;
            }
            set
            {
                if (!string.Equals(serverAddress, value, StringComparison.Ordinal))
                {
                    serverAddress = value;
                    OnPropertyChanged("ProxyServerAddress");
                }
            }
        }

        [Range(1, 65535)]
        public int ProxyServerPort
        {
            get
            {
                return serverPort;
            }
            set
            {
                if (serverPort != value)
                {
                    serverPort = value;
                    OnPropertyChanged("ProxyServerPort");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProgramOptions);
        }

        public bool Equals(ProgramOptions other)
        {
            if (other == null)
            {
                return false;
            }

            return this.IsFirstRun == other.IsFirstRun &&
                this.CheckForProgramUpdates == other.CheckForProgramUpdates &&
                this.UseProxyServer == other.UseProxyServer &&
                string.Equals(this.ProxyServerAddress, other.ProxyServerAddress, StringComparison.Ordinal) &&
                this.ProxyServerPort == other.ProxyServerPort;
        }

        public override int GetHashCode()
        {
            int hashCode = 19;

            unchecked
            {
                hashCode = (hashCode * 23) + IsFirstRun.GetHashCode();
                hashCode = (hashCode * 23) + CheckForProgramUpdates.GetHashCode();
                hashCode = (hashCode * 23) + UseProxyServer.GetHashCode();

                if (ProxyServerAddress != null)
                {
                    hashCode = (hashCode * 23) + ProxyServerAddress.GetHashCode();
                }

                hashCode = (hashCode * 23) + ProxyServerPort.GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(ProgramOptions lhs, ProgramOptions rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            else if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }
            else
            {
                return lhs.Equals(rhs);
            }
        }

        public static bool operator !=(ProgramOptions lhs, ProgramOptions rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Triggers PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool isFirstRun;
        private bool checkForUpdates;
        private bool useProxyServer;

        private string serverAddress;
        private int serverPort;
    }
}
