using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO.Compression;
namespace httpMethodsApp
{
    public delegate void StartedListening(bool hasStarted, string errorMessage);

    /// <summary>
    /// The HttpProcessor class is used to process http request
    /// </summary>
    public class HttpProcessor
    {
        public TcpClient clientSocket;
        public Server server;
        private const int BUF_SIZE = 4096;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient client, Server server)
        {
            this.clientSocket = client;
            this.server = server;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(clientSocket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            //outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            outputStream = new StreamWriter(new BufferedStream(clientSocket.GetStream()));

            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
                outputStream.Flush();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            //flush any remaining output
            outputStream.BaseStream.Flush();
            inputStream = null;
            outputStream = null;
            clientSocket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

        }

        public void readHeaders()
        {
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            server.handleGETRequest(this);
        }


        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            //// Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            //// Console.WriteLine("get post data end");
            server.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeStandardHeaders()
        {

            outputStream.WriteLine("Content-Encoding: gzip");
            outputStream.WriteLine("Content-Language: en");
            outputStream.WriteLine("Content-Range: bytes");
            string HttpDate = DateTime.Now.ToUniversalTime().ToString("r");
            outputStream.WriteLine("Date: " + HttpDate);
            outputStream.WriteLine("Transfer-Encoding: gzip");
            outputStream.WriteLine("ETag: " + 5.ToString());
            outputStream.WriteLine("Last-Modified: " + 5.ToString());

        }

        public void writeSuccess(string content_type = "text/html")
        {

            outputStream.WriteLine("HTTP/1.1 200 OK");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("");
        }
        public void writeSuccess(string content_type, long contentLength, bool isGzipCompressed, bool usingStandardHeaders = true)
        {

            outputStream.WriteLine("HTTP/1.1 200 OK");
            if (contentLength > 0)
            {
                outputStream.WriteLine("Content-Length: " + contentLength.ToString());
            }

            outputStream.WriteLine("Content-Type: " + content_type);

            if (usingStandardHeaders)
            {
                outputStream.WriteLine("Content-Language: en");
                outputStream.WriteLine("Content-Range: bytes");
                string HttpDate = DateTime.Now.ToUniversalTime().ToString("r");
                outputStream.WriteLine("Date: " + HttpDate);
                outputStream.WriteLine("ETag: " + 5.ToString());
                outputStream.WriteLine("Last-Modified: " + 5.ToString());
            }

            if (isGzipCompressed)
            {
                
                outputStream.WriteLine("Transfer-Encoding: gzip");
                outputStream.WriteLine("Content-Encoding: gzip");


            }




            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.1 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");

        }
    }

    public abstract class Server
    {
        protected int port;
        TcpListener listener;
        bool is_active = true;
        long counter = 0;
        public StartedListening startedListeningCallback = null;

        List<Thread> threads = new List<Thread>();
       
         /// <summary>
        /// The designate class constructor. </summary>
        public Server(int port)
        {
            this.port = port;
        }



        public void listen()
        {
            is_active = true;
            listener = new TcpListener(IPAddress.Any, port);
            try
            {

                listener.Start();
                if (startedListeningCallback != null)
                {
                    startedListeningCallback(true, "");
                }
                while (is_active)
                {
                    try
                    {
                        TcpClient tcpClient = listener.AcceptTcpClient();
                        HttpProcessor processor = new HttpProcessor(tcpClient, this);
                        Thread thread = new Thread(new ThreadStart(processor.process));
                        counter++;
                        thread.Name = "TCPListener" + counter.ToString();
                        threads.Add(thread);
                        thread.Start();
                        Thread.Sleep(1);
                    }

                    catch (SocketException ex)
                    {
                        Console.WriteLine("TCPListner has stoped " + ex.Message);
                    }

                    catch (ThreadAbortException ex)
                    {
                        Console.WriteLine("Thread abort inside listener " + ex.Message);
                    }
                }
            }

            catch (System.Net.Sockets.SocketException ex)
            {
                is_active = false;

                // the port is already opened
                if (ex.ErrorCode == 10048)
                {
                    if (startedListeningCallback != null)
                    {
                        startedListeningCallback(false, "The port is not available");
                    }
                }
                else
                {
                    startedListeningCallback(false, ex.Message);

                }
            }

        }

        public void stop()
        {
            listener.Stop();
            is_active = false;
            foreach (Thread thread in threads)
            {
                if (thread.IsAlive)
                {
                    try
                    {
                        thread.Abort();
                    }
                    catch (ThreadAbortException ex)
                    {
                        Console.WriteLine("thread has aborted inside listener stop function" + ex.Message);
                    }
                }
            }
            threads = new List<Thread>();

        }
        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class HttpServer : Server
    {
        private string _includingPath = "";
        public bool useStandardHeaders = true;
        public int PortNumber
        {
            get { return this.port; }
            set { this.port = value;}

        }
       

        public HttpServer(int port)
            : base(port)
        {
            this.PortNumber = port;
        }

        public string includingPath
        {


            get
            {
                return _includingPath;
            }

            set
            {
                _includingPath = value;
            }

        }
       

        private string[] getTextFiles()
        {
            string appPath = Directory.GetCurrentDirectory();
            string[] textFilesAtServerDir = Directory.GetFiles(appPath, "*.txt");
            List<string> allFiles = new List<string>();
            allFiles.AddRange(textFilesAtServerDir);

            try
            {
                string[] textFilesAtIncludedDir = Directory.GetFiles(_includingPath, "*.txt");
                allFiles.AddRange(textFilesAtIncludedDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("No such folder {0}", ex.Message);

            }

            return allFiles.ToArray();

        }

        public override void handleGETRequest(HttpProcessor p)
        {

            bool shouldCompressData = false;
            if ((String)p.httpHeaders["Accept-Encoding"] == "gzip")
            {
                shouldCompressData = true;

            }

            if (p.http_url.Contains(".txt"))
            {


                string[] allTextFiles = getTextFiles();

                string findFile = Path.GetFileName(p.http_url);

                bool precoded = false;
                encodingType dataEncoding = encodingType.Origin;

                if (findFile.Contains("Preconfig"))
                {
                    findFile = findFile.Remove(0, "Preconfig".Length);
                    precoded = true;
                }

                else
                {

                    if (findFile.Contains("Raw"))
                    {

                        int fileTypeLoc = findFile.ToLower().IndexOf("raw");
                        findFile = findFile.Remove(fileTypeLoc, "Raw".Length);
                        dataEncoding = encodingType.Origin;

                    }
                }
                int strNumber = 0;
                int strIndex = -1;

                for (strNumber = 0; strNumber < allTextFiles.Length; strNumber++)
                {
                    strIndex = allTextFiles[strNumber].ToLower().IndexOf(findFile.ToLower());
                    if (strIndex >= 0)
                        break;
                }
                // do the server have the file 
                if (strIndex < 0)
                {
                    // the server cant find the required file 
                    p.writeFailure();
                    return;
                }

                var textFileName = allTextFiles[strNumber];
                var outputFileName = textFileName; // if it was encoded and compressed, it will be ready to be send

                Stream fs = null;
                try
                {

                    if (!precoded)
                    {

                        outputFileName = Path.GetTempFileName();



                        if (dataEncoding == encodingType.Origin)
                        {
                            outputFileName = textFileName;

                            if (shouldCompressData)
                            {
                                fs = new FileStream(outputFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                                FileInfo inputFileInfo = new FileInfo(outputFileName);
                                p.writeSuccess("text/plain", 0, shouldCompressData, useStandardHeaders);
                                p.outputStream.Flush();
                                FormatData.streamCopyToWithGzipCompression(fs, p.outputStream.BaseStream);
                                p.outputStream.Flush();
                                p.outputStream.BaseStream.Flush();

                            }


                            else
                            {
                                fs = File.Open(textFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                                FileInfo fileInfo = new FileInfo(outputFileName);
                                p.writeSuccess("text/plain", fileInfo.Length, shouldCompressData, useStandardHeaders);
                                p.outputStream.Flush();
                                fs.CopyTo(p.outputStream.BaseStream);
                            }




                        }



                    }


                    else // precoded and compressed 
                    {

                        fs = File.Open(outputFileName, FileMode.Open);
                        FileInfo fileInfo = new FileInfo(outputFileName);
                        p.writeSuccess("text/plain", fileInfo.Length, shouldCompressData, useStandardHeaders);
                        p.outputStream.Flush();
                        fs.CopyTo(p.outputStream.BaseStream);

                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine("An error has occured {0}", ex.Message);

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error has occured {0}", ex.Message);

                }
                finally
                {

                    p.outputStream.BaseStream.Flush();
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }

            }
            // if the client asks for autogenerated data 
            else if (p.http_url.Contains("Auto"))
            {


                double[] ratios = null;
                if (p.httpHeaders["RatioType"].ToString() == "Manual")
                {
                    ratios = new double[4];
                    ratios[0] = double.Parse(p.httpHeaders["A"].ToString());
                    ratios[1] = double.Parse(p.httpHeaders["C"].ToString());
                    ratios[2] = double.Parse(p.httpHeaders["G"].ToString());
                    ratios[3] = double.Parse(p.httpHeaders["T"].ToString());


                }
                long dataSize = long.Parse(p.httpHeaders["DataSize"].ToString());

                if (p.http_url.Contains("Raw"))
                {
                    // auto data generation
                    p.writeSuccess("text/plain", 0, shouldCompressData, useStandardHeaders);
                    p.outputStream.Flush();
                    FormatData.generateRawDataAndSend(p.outputStream.BaseStream, dataSize, shouldCompressData, ratios);
                    p.outputStream.BaseStream.Flush();
                    p.outputStream.Flush();

                    return;

                }

                else
                {
                    p.writeFailure();

                }


            }




            else
            {

                p.writeFailure();

            }

        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {

            Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);


        }


    }



    /// <summary>
    /// Controls the start and stop of the http server
    /// </summary>
    public class HttpServerController
    {
        public HttpServer _httpServer;
        private bool _isRunning = false;
        private Thread _serverThread;


       


        public HttpServerController(int port)
        {
            this._httpServer = new HttpServer(port);
        }




        public void start()
        {
            if (_httpServer != null)
            {
                _serverThread = new Thread(new ThreadStart(_httpServer.listen));
                _serverThread.Start();
                _isRunning = true;
            }
        }

        public void stop()
        {
            if (_isRunning)
            {
                try
                {
                    _httpServer.stop();
                    _serverThread.Abort();
                    _isRunning = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception inside server listener stop function " + ex.Message);
                }

            }
        }

    }

}

