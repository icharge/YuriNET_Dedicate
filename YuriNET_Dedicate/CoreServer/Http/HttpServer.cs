﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using YuriNET.Common.Utils;

namespace YuriNET.CoreServer.Http {

    /// <summary>
    /// HttpServer
    /// </summary>
    public abstract class HttpServer {
        protected int port;
        private TcpListener listener;
        private bool is_active = true;
        private Thread listening;

        public HttpServer(int port) {
            this.port = port;
        }

        /// <summary>
        /// Listening incoming connections and wait.
        /// </summary>
        public void listen() {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (is_active) {
                try {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Start();
                    Thread.Sleep(1);
                } catch (InvalidOperationException) {
                } catch (SocketException) {
                }
            }
        }

        /// <summary>
        /// Listening incoming connections.
        /// </summary>
        public void bind() {
            listening = new Thread(new ThreadStart(listen));
            listening.Start();
        }

        public virtual void stop() {
            Logger.info("Shutting down HTTP Server... (Waiting for in-queue requeset finished)");
            is_active = false;
            listener.Stop();
            listening.Abort();
        }

        internal bool isActive() {
            return is_active;
        }

        public abstract void handleGETRequest(HttpProcessor p);

        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
}