using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace httpMethodsApp
{
    public  enum RatioTypeEnum
    {
        Automatic, Manual
    }
    public static class   FormatData
    {

        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static Random rand = new Random();

        public static string specialChars = "ACGT";
        public static string headerStartingString = ">";
        public static string bitsToCompleteHeader = "Bits_To_Complete:";


        public static long getFileHeaderEnd(char[] buffer)
        {
           
            long headerIndex = -1;
            string headerString;
            headerString = new string(buffer, 0, headerStartingString.Length);

            if (headerString == headerStartingString)
            {
                 headerIndex = (long)Array.IndexOf<char>(buffer, '\n');

              
            }
            return headerIndex ;
        }

        private static long findFirstSepcialChar(char[] buffer, long startAt)
        {

            long location = -1;
            location = (long)Array.FindIndex<char>(buffer,(int) startAt, x => specialChars.Contains(x));
            return location;
        }

        private static void initRandomCharBuffer(ref char[] randomCharBuffer, char exceptChar = ' ')
        {
            List<char> _specialCharList = new List<char>();


            int index = 0;
            foreach (char item in specialChars)
            {
                if (item != exceptChar)
                {
                    _specialCharList.Add(item);
                    index++;
                }
            }
            byte[] fourBytes = new byte[4];


            rng.GetBytes(fourBytes);

            int arraySize = fourBytes[0] % 32 + fourBytes[1] % 32 + fourBytes[2] % 32 + fourBytes[3] % 32;
            if (arraySize < 10)
            {
                arraySize += 10; 
            }
            randomCharBuffer = new char[arraySize];
            // fill the char array with ACGT randomly
            for (index = 0; index < arraySize; index++)
            {
                rng.GetBytes(fourBytes);

                for (int charIndex = 0; charIndex < _specialCharList.Count; charIndex++)
                {
                    int specialCharIndex = fourBytes[charIndex] % _specialCharList.Count;

                    randomCharBuffer[index] = _specialCharList[specialCharIndex];
                    if (charIndex != _specialCharList.Count - 1)
                        index++;
                    if (index >= arraySize)
                        break;
                }

            }
        }

        private static  Dictionary<char, long> generateGenomeFile(Stream outputStream, long fileSize, char preChar = ' ', double preCharRatio = 0.25)
        {
            StreamWriter textWriter = new StreamWriter(outputStream);
            char[] buffer = new char[1]; //  just to initilize it 
            long charsToSave = fileSize;
            long writenChars = 0;
            long progress = 0;
            Dictionary<char, long> charCounts = new Dictionary<char, long>();
            {
                charsToSave -= buffer.LongLength;
                writenChars = fileSize - charsToSave;
                progress = 100 * writenChars / fileSize;
            };
            foreach (char item in specialChars)
            {
                if (item != preChar)
                    charCounts.Add(item, 0);

            }
            char[] randomCharBuffer = new char[4];//

            if (specialChars.Contains(preChar))
            {
                long preCharNum = (long)(fileSize * preCharRatio);
                writeCharToFile(preChar, textWriter, preCharNum);
                charsToSave -= preCharNum;
                charCounts[preChar] = preCharNum;
            }
            initRandomCharBuffer(ref randomCharBuffer, preChar);

            while (charsToSave > 0)
            {
                generateCharsRandomley(randomCharBuffer, ref buffer, charsToSave, ref charCounts);
                textWriter.Write(buffer);
                charsToSave -= buffer.LongLength;
                writenChars = fileSize - charsToSave;
                progress = 100 * writenChars / fileSize;
            }
            textWriter.Flush();
            textWriter.Close();
            return charCounts;


        }
        private static void generateCharsRandomley(char[] randomCharBuffer, ref char[] data, long dataSize, ref  Dictionary<char, long> charCounts)
        {

            byte[] fourBytes = new byte[4];
            int bufferSize = (int)Math.Min(dataSize, 1024 * 1024);
            data = new char[bufferSize];

            // Convert to int 32.

            for (int index = 0; index < bufferSize; index++)
            {

                int charIndex = 0;
                fourBytes = new byte[4];

                // Fill buffer.
                rand.NextBytes(fourBytes);

                // Convert to int 32.
                int value = BitConverter.ToInt32(fourBytes, 0);

                value = Math.Abs(value);
                rand.NextBytes(fourBytes);
                charIndex = value % randomCharBuffer.Length;

                char key = randomCharBuffer[charIndex];
                data[index] = key;
                charCounts[key]++;

            }


        }

        private static void writeCharToFile(char charToWrite, StreamWriter stream, long dataSize)
        {
            int bufferSize = (int)Math.Min(dataSize, 512 * 1024);
            string buffer;//= new char[bufferSize];// 512 K chars
            long numOfWrittenChars = 0;
            while (numOfWrittenChars < dataSize)
            {

                bufferSize = (int)Math.Min(dataSize, 512 * 1024);
                buffer = new string(charToWrite, bufferSize);
                stream.Write(buffer);
                numOfWrittenChars += bufferSize;
            }

        }


        public static void generateRawDataAndSend(Stream outputStream, long dataSize)
        {

            int bufferSize = 512 * 1025; // 512 KB
            long chunkNum = dataSize / bufferSize;
            int extraBytes = (int)dataSize % bufferSize;
            List<byte> data = new List<byte>();


            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            var compressedbuffer = new byte[1];
            char[] randomCharBuffer = new char[1];
            initRandomCharBuffer(ref randomCharBuffer, ' ');

            for (long index = 0; index < chunkNum; index++)
            {
                for (int byteIndex = 0; byteIndex < bufferSize; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    data.Add((byte)randomCharBuffer[charIndex]);

                }
                byte[] buffer = data.ToArray();
                data.Clear();

                gzipStream.Write(buffer, 0, (int)buffer.Length);
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.SetLength(0);
                memStream.Capacity = 10;

            }

            if (extraBytes > 0)
            {
                for (int byteIndex = 0; byteIndex < extraBytes; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    data.Add((byte)randomCharBuffer[charIndex]);
                }
                byte[] buffer = data.ToArray();
                data.Clear();
                gzipStream.Write(buffer, 0, (int)buffer.Length);
                long currentpos = memStream.Position;
                long length = memStream.Length;

            }

            gzipStream.Flush();
            gzipStream.Close();
            compressedbuffer = memStream.ToArray();
            outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);


        }
        public static double[] randomRatios(int length)
        {
            double[] ratios = new double[length];
            double charCountRatioSum = 0;
            for (int index = 0; index < length; index++)
            {
                double ratio = rand.NextDouble();
                if (ratio < 0.001)
                {
                    ratio = 0.1;
                }
                ratios[index] = ratio;
                charCountRatioSum += ratio;
            }


            //Normalizing the ratios to be summed to one
            for (int index = 0; index < length; index++)
            {

                ratios[index] /= charCountRatioSum;
            }
            return ratios;
        }


        public static void generateTwoBitDataAndSend(Stream outputStream, long dataSize)
        {


            int bufferSize = 512 * 1025; // 512 KB
            long chunkNum = dataSize / bufferSize;
            int extraBytes = (int)dataSize % bufferSize;
            List<byte> data = new List<byte>();


            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            var compressedbuffer = new byte[1];

            int bitIndex = 0;
            int oneByte = 0;
            char[] randomCharBuffer = new char[1];
            initRandomCharBuffer(ref randomCharBuffer, ' ');
            for (int index = 0; index < chunkNum; index++)
            {

                for (int byteIndex = 0; byteIndex < bufferSize; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    char oneChar = randomCharBuffer[charIndex];
                    oneByte = oneByte | FormatData.charCodeToByte(oneChar) << bitIndex;
                    bitIndex += 2;
                    if (bitIndex == 8)
                    {
                        data.Add((byte)oneByte);
                        bitIndex = 0;
                        oneByte = 0;
                    }

                }

                byte[] buffer = data.ToArray();
                data.Clear();
                gzipStream.Write(buffer, 0, (int)buffer.Length);
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.SetLength(0);
                memStream.Capacity = 10;

             
            }

            if (extraBytes > 0)
            {
                for (int byteIndex = 0; byteIndex < extraBytes; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    char oneChar = randomCharBuffer[charIndex];
                    oneByte = oneByte | FormatData.charCodeToByte(oneChar) << bitIndex;
                    bitIndex += 2;
                    if (bitIndex == 8)
                    {
                        data.Add((byte)oneByte);
                        bitIndex = 0;
                        oneByte = 0;
                    }

                }

                if (bitIndex > 0)
                {
                    data.Add((byte)oneByte);

                }
                else
                {
                    bitIndex = 8;
                }
                byte[] buffer = data.ToArray();
                data.Clear();
                gzipStream.Write(buffer, 0, (int)buffer.Length);
                writeBitsComplement(gzipStream, bitIndex);

                gzipStream.Flush();
                gzipStream.Close();
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);

               

            }

          

        }


        public static void convertToTwoBitsFormatAndCompress(string inputFileName, Stream outputStream)
        {

            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader charReader = new BinaryReader(inputStream);
            int data = 0;
            char[] buffer;
            int bufferSize = 8 * 1024;// 4 Mega byte
            long byteToReads = fileIno.Length;
            int bitIndex = 0;
            buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));

            long stdHeaderLength = getFileHeaderEnd(buffer);
            List<byte> bytesToSave = new List<byte>();
            if (stdHeaderLength > -1)
            {

                byte[] byteData;
                stdHeaderLength++;
                byteData = Encoding.Default.GetBytes(buffer, 0, (int)stdHeaderLength);
               // outputStream.Write(byteData, 0, byteData.Length);
                bytesToSave.AddRange(byteData);
                var tempBuffer = new char[buffer.Length - stdHeaderLength];
                Array.Copy(buffer, stdHeaderLength, tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;
                byteToReads -= (stdHeaderLength);
            }


            while (buffer.Length > 0)
            {

                byteToReads -= buffer.Count();

               

                foreach (char item in buffer)
                {
                    if (specialChars.Contains(item))
                    {

                        data = data | charCodeToByte(item) << bitIndex;

                        bitIndex += 2;
                        if (bitIndex == 8)
                        {
                            byte codeData = (byte)data;
                            bytesToSave.Add(codeData);
                            bitIndex = 0;
                            data = 0;
                        }

                    }
                }
                if (bytesToSave.Count > 0)
                {
                    outputStream.Write(bytesToSave.ToArray(), 0, bytesToSave.Count);
                }

                buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));
                bytesToSave.Clear();

            }

            if (bitIndex > 0)
            {
                byte codeData = (byte)data;
                outputStream.WriteByte(codeData);
                writeBitsComplement(outputStream, bitIndex);
                bitIndex = 0;
            }
            else
            {
                writeBitsComplement(outputStream, 8);

            }
           // outputStream.Close();
            inputStream.Close();
        }

        public static void convertToTwoBitsFormat(string inputFileName , string outputFileName)
        {

            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader charReader = new BinaryReader(inputStream);
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create);
            int data = 0;
            char[] buffer;
            int bufferSize =  8 * 1024;// 4 Mega byte
            long byteToReads = fileIno.Length;
            int bitIndex = 0;
            buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));

            long stdHeaderLength = getFileHeaderEnd(buffer);
            if (stdHeaderLength > -1)
            {

                byte[] byteData;
                stdHeaderLength++;
                byteData = Encoding.Default.GetBytes(buffer, 0, (int)stdHeaderLength  );
                outputStream.Write(byteData, 0, byteData.Length);

                var tempBuffer = new char[buffer.Length - stdHeaderLength ];
                Array.Copy(buffer, stdHeaderLength , tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;
                byteToReads -= (stdHeaderLength );
            }


            while (buffer.Length > 0) 
            {

                byteToReads -= buffer.Count();

                List<byte> bytesToSave = new List<byte>();

                foreach (char item in buffer)
                {
                    if (specialChars.Contains(item))
                    {

                        data = data | charCodeToByte(item) << bitIndex;

                        bitIndex += 2;
                        if (bitIndex == 8)
                        {
                            byte codeData = (byte)data;
                            bytesToSave.Add(codeData);
                            bitIndex = 0;
                            data = 0;
                        }

                    }
                }
                if (bytesToSave.Count > 0)
                {
                    outputStream.Write(bytesToSave.ToArray(), 0, bytesToSave.Count);
                }

                buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));

            } 

            if (bitIndex > 0)
            {
                byte codeData = (byte)data;
                outputStream.WriteByte(codeData);
                bitIndex = 0;
            }
            outputStream.Close();
            inputStream.Close();
        }
        public static void convertToTwoBitsFormatWithCompressionToStream(string inputFileName, Stream outputStream)
        {

            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader charReader = new BinaryReader(inputStream);
            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            int data = 0;
            char[] buffer;
            int bufferSize = 8 * 1024;// 4 Mega byte
            long byteToReads = fileIno.Length;
            int bitIndex = 0;
            buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));

            long stdHeaderLength = getFileHeaderEnd(buffer);
            List<byte> bytesToSave = new List<byte>();
            if (stdHeaderLength > -1)
            {

                byte[] byteData;
                stdHeaderLength++;
                byteData = Encoding.Default.GetBytes(buffer, 0, (int)stdHeaderLength);
                bytesToSave.AddRange(byteData);
                var tempBuffer = new char[buffer.Length - stdHeaderLength];
                Array.Copy(buffer, stdHeaderLength, tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;
                byteToReads -= (stdHeaderLength);
            }

            byte[] compressedBytes;
            while (buffer.Length > 0)
            {

                byteToReads -= buffer.Count();



                foreach (char item in buffer)
                {
                    if (specialChars.Contains(item))
                    {

                        data = data | charCodeToByte(item) << bitIndex;

                        bitIndex += 2;
                        if (bitIndex == 8)
                        {
                            byte codeData = (byte)data;
                            bytesToSave.Add(codeData);
                            bitIndex = 0;
                            data = 0;
                        }

                    }
                }
                if (bytesToSave.Count > 0)
                {
                    gzipStream.Write(bytesToSave.ToArray(), 0, bytesToSave.Count);
                    compressedBytes = memStream.ToArray();
                    outputStream.Write(compressedBytes, 0, compressedBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.SetLength(0);
                }

                buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));
                bytesToSave.Clear();

            }

            if (bitIndex > 0)
            {
                byte codeData = (byte)data;
                gzipStream.WriteByte(codeData);
                writeBitsComplement(gzipStream, bitIndex);
                bitIndex = 0;
            }
            else
            {
                writeBitsComplement(gzipStream, 8);

            }
            gzipStream.Close();
            compressedBytes = memStream.ToArray();
            outputStream.Write(compressedBytes, 0, compressedBytes.Length);
            inputStream.Close();
        }


        private static  void writeBitsComplement(Stream outputStream, int NumOfBits)
        {
            Byte bitsToComplete = (Byte)(8 - NumOfBits);

            string header = "\n" + bitsToCompleteHeader;

            byte[] buffer = Encoding.Default.GetBytes(header);
            Array.Resize(ref buffer, buffer.Length + 1);
            buffer[buffer.Length - 1] = bitsToComplete;
            outputStream.Write(buffer, 0, buffer.Length);
        }


        public static Dictionary<char, long> getStatisticFromGenomeFile(String inputFileName)
        {

            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader bytesReader = new BinaryReader(inputStream);

            Dictionary<char, long> itemCounts = new Dictionary<char, long>();
            foreach (char item in specialChars)
            {
                itemCounts.Add(item, 0);
            }
            long bytesToRead = fileIno.Length;
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesNum = buffer.Length;

            buffer = bytesReader.ReadBytes((int)(Math.Min(bytesToRead, bufferSize)));

            char[] charBuffer = Encoding.Default.GetChars(buffer);
            long stdHeaderLength = getFileHeaderEnd(charBuffer);
            // check if the file contains standard genome header 
            if (stdHeaderLength > -1)
            {
                stdHeaderLength++;
                string header = new string(charBuffer, 0, (int)stdHeaderLength);
                int headerBytesCount = Encoding.Default.GetByteCount(charBuffer, 0, (int)stdHeaderLength);
                var tempBuffer = new byte[buffer.Length - headerBytesCount];
                Array.Copy(buffer, headerBytesCount, tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;
                bytesToRead -= headerBytesCount;

            }

           

            while (buffer.Length > 0)
            {


                bytesToRead -= buffer.Length;


                for (int index = 0; index < buffer.Length; index++)
                {
                    byte item = buffer[index];
                    char charItem;
                    charItem = (char)item;
                    if (specialChars.Contains(charItem))
                    itemCounts[charItem]++;

                }

                buffer = bytesReader.ReadBytes((int)(Math.Min(bytesToRead, bufferSize)));
            }
          
            bytesReader.Close();
            inputStream.Close();
            return itemCounts;
        }



        public static void decodeTwoBitsFormatFile(String inputFileName , string outputFileName)
        {
            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create);
            StreamWriter textStream = new StreamWriter(outputStream, Encoding.Default);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader bytesReader = new BinaryReader(inputStream);

            string code = "";
          

            long bytesToRead = fileIno.Length;
            int bufferSize = 4*1024*1024;
            byte[] buffer = new byte[bufferSize];
            int bytesNum = buffer.Length;
            int bitsCompleteHeaderln = Encoding.Default.GetByteCount("\n" + bitsToCompleteHeader) + 2; // one coded byte and one one byte for bits to complete one byte
            bytesToRead -= bitsCompleteHeaderln;

            buffer = bytesReader.ReadBytes((int)(Math.Min(bytesToRead, bufferSize)));
           // bytesNum = inputStream.Read(buffer, 0, (int)(Math.Min(bytesToRead, bufferSize)));

            char[] charBuffer = Encoding.Default.GetChars(buffer);
            long stdHeaderLength = getFileHeaderEnd(charBuffer);
            // check if the file contains standard genome header 
            if (stdHeaderLength > -1)
            {
                stdHeaderLength++;
                string header = new string(charBuffer, 0, (int) stdHeaderLength );
                textStream.Write(header);
                int headerBytesCount=  Encoding.Default.GetByteCount(charBuffer, 0, (int)stdHeaderLength );
                var tempBuffer = new byte[buffer.Length - headerBytesCount];
                Array.Copy(buffer, headerBytesCount, tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;

                bytesToRead -= headerBytesCount;
                
            }

           
           
            while (buffer.Length > 0) 
            {


                bytesToRead -= buffer.Length;


                for (int index = 0; index < buffer.Length; index++)
                {
                    byte item = buffer[index];
                    code += byteCodeToChar((byte)(item & 0x3));
                    code += byteCodeToChar((byte)((item >> 2) & 0x3));
                    code += byteCodeToChar((byte)((item >> 4) & 0x3));
                    code += byteCodeToChar((byte)((item >> 6) & 0x3));
                    textStream.Write(code);
                    code = "";

                }

                buffer = bytesReader.ReadBytes((int)(Math.Min(bytesToRead, bufferSize)));
            }
            buffer = bytesReader.ReadBytes((int)(Math.Min(bitsCompleteHeaderln, bufferSize)));
            byte lastEncodedByte = buffer[0];
            byte bitsToComplete = buffer[buffer.Length - 1];
            var getBitsCompleteHeader = Encoding.Default.GetString(buffer, 2, buffer.Length - 3); // without "\n" 
            code = "";
            if (getBitsCompleteHeader == bitsToCompleteHeader)
            {
                for (int index = 0; index < 8 - bitsToComplete; index +=2)
                {
                    code += byteCodeToChar((byte)(lastEncodedByte & 0x3));

                    lastEncodedByte = (byte)(lastEncodedByte >> 2);
   

                }
            }
            textStream.Write(code);
            textStream.Close();
            outputStream.Close();
            bytesReader.Close();
            inputStream.Close();
        }

        public static void convertByEncodingDublicates(String inputFileName, string outputFileName)
        {
            FileInfo fileIno = new FileInfo(inputFileName);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            BinaryReader charReader = new BinaryReader(inputStream);
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create);
            int data = 0;
            int sequence = 1;
            char[] buffer;
            int bufferSize = 4 * 1024 * 1024;// 4 Mega byte
            long byteToReads = fileIno.Length ;



            buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));

            long stdHeaderLength = getFileHeaderEnd(buffer);
            if (stdHeaderLength > -1)
            {

                byte[] byteData;
                stdHeaderLength++;
                byteData = Encoding.Default.GetBytes(buffer, 0, (int)stdHeaderLength);
                outputStream.Write(byteData, 0, byteData.Length);
            }

            else
            {
                stdHeaderLength = 0;
            }

            long firstSpecialCharLoc = findFirstSepcialChar(buffer, stdHeaderLength);

           var sequenceCode = buffer[firstSpecialCharLoc];

           firstSpecialCharLoc++;
           var tempBuffer = new char[buffer.Length - firstSpecialCharLoc];
           Array.Copy(buffer, firstSpecialCharLoc, tempBuffer, 0, tempBuffer.LongLength);
            buffer = tempBuffer;
            byteToReads -= firstSpecialCharLoc ;

            while (byteToReads > 0)
            {
                byteToReads = byteToReads - buffer.Count();

                foreach (char item in buffer)
                {
                    if (specialChars.Contains(item))
                    {
                        if (item == sequenceCode)
                        {
                            sequence++;
                        }

                        else
                        {
                            if (sequence > 1 && sequence < 64) // we got repeated sequence
                            {
                                // processRepeatedLetter(ref sequence, item, ref sequenceCode, ref data, ref bitIndex ,outputStream);
                                data = sequence << 2 | charCodeToByte(sequenceCode);
                                sequence = 1;

                            }
                            else // no repeated sequence for the last code
                            {
                                data = charCodeToByte(sequenceCode); // represent the previous read char
                                sequenceCode = item;
                            }

                            outputStream.WriteByte((byte)data);
                            sequenceCode = item;

                        }
                    }
                }
                buffer = charReader.ReadChars((int)Math.Min(bufferSize, byteToReads));


            }

            if (sequence > 1) // we got repeated letter at the end of the text so process it here
            {
                data = sequence << 2 | charCodeToByte(sequenceCode);
                outputStream.WriteByte((byte)data);

            }
            else
            {
                data = charCodeToByte(sequenceCode);
                outputStream.WriteByte((byte)data);
            }

            inputStream.Close();
            outputStream.Close();
        }

        public static void compressAndSaveData(string inputFileName, string outputFileName)
        {
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create);
            GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress, true);
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            FileInfo inputFileInfo = new FileInfo(inputFileName);
            
            int bufferSize =  8 * 1024;// 4 K byte
            byte[] buffer = new byte[bufferSize];

            long totalBytes = inputFileInfo.Length;
            long bytesToRead = totalBytes;
           /*
            while (bytesToRead > 0)
            {
               int bytesNum = inputStream.Read(buffer ,0, (int) Math.Min(bytesToRead , bufferSize));
               bytesToRead -= bytesNum;
               gzipStream.Write(buffer, 0, bytesNum);
            }
            */
            inputStream.CopyTo(gzipStream);
            gzipStream.Flush();
            gzipStream.Close();
            outputStream.Close();
            inputStream.Close();
        }


        public static void uncompressAndSaveData(string inputFileName, string outputFileName)
        {
            FileStream inputStream = new FileStream(inputFileName, FileMode.Open);
            FileStream outputStream = new FileStream(outputFileName, FileMode.Create);
            GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, true);


            const int size = 4096;
            byte[] buffer = new byte[size];

            int count = 0;
            
            do
            {
                count = gzipStream.Read(buffer, 0, size);
                if (count > 0)
                {
                    outputStream.Write(buffer, 0, count);
                }
            }
            while (count > 0);
             
           
            gzipStream.Close();
            inputStream.Close();
            outputStream.Close();
        }


        public static void decodeDublicateEncodedData(string inputFileName , string outputFileName)
        {
            byte[] buffer;
            int bufferSize = 4 * 1024 * 1024;


            FileInfo inputFInfo = new FileInfo(inputFileName);
            var bytesToRead = inputFInfo.Length;
            var inputFileStream = new FileStream(inputFileName, FileMode.Open);
            var outputFileStream = new FileStream(outputFileName, FileMode.Create);
            var inputReader = new BinaryReader(inputFileStream);
            var outputWritter = new StreamWriter(outputFileStream);
            


            buffer = inputReader.ReadBytes((int)(Math.Min(bytesToRead, bufferSize)));
            // bytesNum = inputStream.Read(buffer, 0, (int)(Math.Min(bytesToRead, bufferSize)));

            char[] charBuffer = Encoding.Default.GetChars(buffer);
            long stdHeaderLength = getFileHeaderEnd(charBuffer);
            // check if the file contains standard genome header 
            if (stdHeaderLength > -1)
            {
                stdHeaderLength++;
                string header = new string(charBuffer, 0, (int)stdHeaderLength);
                outputWritter.Write(header);
                outputWritter.Flush();
                int headerBytesCount = Encoding.Default.GetByteCount(charBuffer, 0, (int)stdHeaderLength);
                var tempBuffer = new byte[buffer.Length - headerBytesCount];
                Array.Copy(buffer, headerBytesCount, tempBuffer, 0, tempBuffer.LongLength);
                buffer = tempBuffer;
                bytesToRead -= headerBytesCount;


            }



            while (bytesToRead > 0)
            {
                bytesToRead -= buffer.LongLength;
                foreach (byte item in buffer)
                {

                    if (item > 3)
                    {
                        var byteCode = (byte)(item & 0x3);
                        var repetition = (byte)(item >> 2);
                        var charCode = byteCodeToChar(byteCode);
                        var textData = new string(charCode, repetition);
                        outputWritter.Write(textData);

                    }
                    else
                    {
                        var charCode = byteCodeToChar(item);
                        outputWritter.Write(charCode);
                    }
                }
                buffer = inputReader.ReadBytes((int)Math.Min(bufferSize, bytesToRead));

            }

            inputFileStream.Close();
            outputWritter.Close();
            outputFileStream.Close();




        }


        public static void streamCopyToWithGzipCompression(Stream inputStream, Stream outputStream)
        {

            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            int bufferSize = 512 * 1024; ;// 4 K byte
            byte[] buffer = new byte[bufferSize];


            var compressedbuffer = memStream.ToArray();
            int num = 0;
            do
            {
                num = inputStream.Read(buffer, 0, (int)bufferSize);
                gzipStream.Write(buffer, 0, (int)num);
                long currentpos = memStream.Position;
                long length = memStream.Length;
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.SetLength(0);
                memStream.Capacity = 10;


            } while (num != 0);
            gzipStream.Flush();
            gzipStream.Close();

            compressedbuffer = memStream.ToArray();
            outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
            outputStream.Flush();



        }


        internal struct BitsStack
        {
            /// <summary>
            /// The unit to write and read from a stream.
            /// </summary>
            public Byte Container;
            private byte Amount;

            /// <summary>
            /// Indicated if the stack unit is full with 8 bits.
            /// </summary>
            /// <returns>true if full, false if not.</returns>
            public bool IsFull()
            {
                return Amount == 8;
            }
            /// <summary>
            /// Indicated if the stack unit is empty(0 bits).
            /// </summary>
            /// <returns>true if empty, false if not.</returns>
            public bool IsEmpty()
            {
                return Amount == 0;
            }
            /// <summary>
            /// Get the number of bits, currently located in the stack.
            /// </summary>
            /// <returns>Number of bits located in the stack.</returns>
            public Byte NumOfBits()
            {
                return Amount;
            }

            /// <summary>
            /// This function removes all the bits from the stack.
            /// </summary>
            public void Empty() { Amount = Container = 0; }

            /// <summary>
            /// Push a bit to the left of the stack (Most significant bit).
            /// </summary>
            /// <remarks>The stack must have at least 1 free bit slot.</remarks>
            /// <param name="Flag">The bit to add the stack(true = 1, false = 0)</param>
            /// <exception cref="Exception">
            /// When attempting to push a bit from a full stack.
            /// </exception>
            public void PushFlag(bool Flag)
            {
                if (Amount == 8) throw new Exception("Stack is full");
                Container >>= 1;
                if (Flag) Container |= 128;
                ++Amount;
            }

            /// <summary>
            /// Pops a bit from the right of the stack (Least significant bit).
            /// </summary>
            /// <returns></returns>
            /// <remarks>The stack must'nt be empty this function called.</remarks>
            /// <exception cref="Exception">
            /// When attempting to pop a bit from an empty stack.
            /// </exception>
            public bool PopFlag()
            {
                if (Amount == 0) throw new Exception("Stack is empty");
                bool t = (Container & 1) != 0;
                --Amount;
                Container >>= 1;
                return t;
            }

            /// <summary>
            /// Fill the stack with 8 bits. If the stack is full, the given byte will
            /// override the old bits.
            /// </summary>
            /// <param name="Data">Byte(8 bits) to put in the current stack.</param>
            public void FillStack(Byte Data)
            {
                Container = Data;
                Amount = 8;
            }

        }

        public static char byteCodeToChar(byte data)
        {
            char charCode;
            switch (data)
            {

                case 0:
                    charCode = 'A';
                    break;
                case 1:
                    charCode = 'C';
                    break;
                case 2:
                    charCode = 'G';
                    break;
                case 3:
                    charCode = 'T';
                    break;
                default:
                    charCode = ' ';
                    break;

            }
            return charCode;
        }

     
        public static int charCodeToByte(char code)
        {
            int data = 0;
            switch (code)
            {

                case 'A':
                    data = 0;
                    break;
                case 'C':
                    data = 1;
                    break;
                case 'G':
                    data = 2;
                    break;
                case 'T':
                    data = 3;
                    break;
                default:
                    break;

            }
            return data;
        }
    }
}
