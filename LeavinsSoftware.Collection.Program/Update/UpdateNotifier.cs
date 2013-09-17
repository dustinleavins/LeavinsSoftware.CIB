// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LeavinsSoftware.Collection.Program.Update
{
    /// <summary>
    /// Manages update notification back-end work.
    /// </summary>
    public sealed class UpdateNotifier
    {
        public UpdateNotifier(ProgramOptions options,
            Version clientVersion,
            Uri serverVersionUri)
        {
            Options = options;
            ClientVersion = clientVersion;
            ServerVersionUri = serverVersionUri;
        }

        public ProgramOptions Options { get; set; }

        public Version ClientVersion { get; private set; }

        public Uri ServerVersionUri { get; private set; }

        public void GetServerVersionAsync(Action<Version> callbackDelegate)
        {
            Task.Factory.StartNew(() =>
            {
                var version = GetServerVersion();
                callbackDelegate(version);
            });
        }

        private Version GetServerVersion()
        {
            lock (serverVersionLock)
            {
                if (cachedServerVersion != null)
                {
                    return cachedServerVersion;
                }

                Stream webStream = null;

                try
                {
                    var request = WebRequest.Create(ServerVersionUri);

                    if (Options.UseProxyServer)
                    {
                        request.Proxy = new WebProxy(Options.ProxyServerAddress,
                            Options.ProxyServerPort);
                    }

                    request.Timeout = 5000;
                    webStream = request.GetResponse().GetResponseStream();

                    using (var reader = XmlReader.Create(webStream))
                    {
                        webStream = null;

                        reader.ReadStartElement("version");
                        cachedServerVersion = new Version(reader.Value);
                    }
                }
                // It is bad form to silence exceptions.
                // However, there are specific exceptions that are not
                // important to functionality, so those can be ignored.
                catch (UriFormatException) // Bad proxy server address
                {
                }
                catch (WebException) // cannot access site
                {
                }
                catch (System.Security.SecurityException) // cannot access site
                {
                }
                catch (XmlException) // retrieved bad XML
                {
                }
                catch (ArgumentException) // problem w/ creating Version
                {
                }
                catch (OverflowException) // problem w/ creating Version
                {
                }
                catch (FormatException) // problem w/ creating Version
                {
                }
                finally
                {
                    if (webStream != null)
                    {
                        webStream.Dispose();
                        webStream = null;
                    }
                }

                return cachedServerVersion;
            }
        }

        private object serverVersionLock = new object();
        private Version cachedServerVersion;
    }
}
