using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

using System.Threading;
using System.IO.Compression;
using System.Security.Cryptography;



namespace httpMethodsApp

{




    /// <summary>
    /// Handles attempt to extract archive that is protected with password,
    /// by using wrong password.
    /// </summary>

    /// <summary>
    /// Invoked from all xxxxWithProgress functions whenever another 1 percent
    /// of the function is done.
    /// </summary>
    public delegate void PercentCompletedEventHandler();

    /// <summary>
    /// Implementing the Huffman shrinking algorithm.
    /// </summary>
    public class HuffmanAlgorithm : IDisposable
    {
        private const long bufferSize = 512 * 1024;
        private const string genomeToHuffmanHeader = "Genome_To_Huffman:" ; // 
        private string bitsToCompleteHeader = "Bits_To_Complete:";
        private RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private  Random rand = new Random();


        #region Internal classes
        //-------------------------------------------------------------------------------	
        /// <summary>
        /// <c>FrequencyTable</c> build from bytes and their repeatition in the stream.
        /// this is achieved by using  2 arrays with the of same size.
        /// </summary>
        [Serializable]
        internal class FrequencyTable
        {
            /// <summary>
            /// Saves all the varies types of bytes (up to 256 ) found in a stream.
            /// </summary>
            public Byte[] FoundBytes;
            /// <summary>
            /// Saves the amount of times each byte in the stream apears.
            /// </summary>
            public uint[] Frequency;
        }//end of FrequencyTable class

        public class GenomeHuffmanTable
        {
            /// <summary>
            /// Saves the amount of times each Genome char in the stream apears. A, B, C , D
            /// </summary>
           // public uint[] Frequency;
            public Dictionary<char, long> Frequency = new Dictionary<char, long>();
            public SortedDictionary<char, byte> GenomeToHuffman = new SortedDictionary<char, byte>();
            public Dictionary<byte, char> ReverseGenomeHuffmanTable = new  Dictionary<byte, char>();

            public GenomeHuffmanTable()
            {
                this.Frequency = new Dictionary<char, long>();
                this.Frequency.Add('A', 0);
                this.Frequency.Add('C', 0);
                this.Frequency.Add('G', 0);
                this.Frequency.Add('T', 0);

                this.GenomeToHuffman = new SortedDictionary<char, byte>();
                this.GenomeToHuffman.Add('A', 0);
                this.GenomeToHuffman.Add('C', 3);
                this.GenomeToHuffman.Add('G', 4);
                this.GenomeToHuffman.Add('T', 5);

                foreach (var keyValue in this.GenomeToHuffman)
                {
                    ReverseGenomeHuffmanTable.Add(keyValue.Value, keyValue.Key);

                }

            }
            public void writeToStream(Stream outputStream)
            {
                var items = GenomeToHuffman.Values.ToArray();
                string header = genomeToHuffmanHeader + items.Length + "\n";
                byte[] headerBuffer = Encoding.Default.GetBytes(header);

                List<byte> data = new List<byte>();
                data.AddRange(headerBuffer);
                data.AddRange(items);

                var buffer = data.ToArray();

                outputStream.Write(buffer, 0, buffer.Length);
      

            }
            public int readFromBuffer(byte[] dataBuffer , int atIndex)
            {
                var genomeToHuffmanHeaderByteCount = Encoding.Default.GetByteCount(genomeToHuffmanHeader + "\n") + 1; // one bye for the number of genome symobls i.e 4 
                if (dataBuffer.Length < atIndex + genomeToHuffmanHeaderByteCount + 4) 
                {
                    return -1;
                }
                var header = Encoding.Default.GetString(dataBuffer,atIndex, dataBuffer.Length - atIndex);
                var buffer = new byte[1];

                if (header.Contains(genomeToHuffmanHeader))
                {
                    var loc = header.IndexOf(":" );
                    long huffmanByteNum = long.Parse(header.Substring(loc +1,1));
                    buffer = new byte[huffmanByteNum];
                    header = genomeToHuffmanHeader + huffmanByteNum.ToString() + "\n";
                }
                else
                {

                    return 0;
                }
                var headerBytesNum = Encoding.Default.GetByteCount(header);
                var genomeTableOffest = atIndex + headerBytesNum ;
                Array.Copy(dataBuffer, genomeTableOffest, buffer, 0, buffer.Length);

                GenomeToHuffman = new SortedDictionary<char, byte>();
                byte code = 0;
                int codeIndex = 0;
                SortedSet<char> sortedSpecialChars = new SortedSet<char>();
                foreach (var charItem in FormatData.specialChars)
                {
                    sortedSpecialChars.Add(charItem);
                }
                foreach (var charItem in sortedSpecialChars)
                {
                    code = buffer[codeIndex];
                    GenomeToHuffman.Add(charItem, code);
                    codeIndex++;
                }
                ReverseGenomeHuffmanTable = new Dictionary<byte, char>();
                foreach (var keyValue in this.GenomeToHuffman)
                {
                    ReverseGenomeHuffmanTable.Add(keyValue.Value, keyValue.Key);

                }
                return headerBytesNum + buffer.Length;

            }

            public void readFromStream(Stream inputStream)
           
            {
                var currentPos = inputStream.Position;
                StreamReader textReader = new StreamReader(inputStream);

                var header = textReader.ReadLine();
                var buffer = new byte[1];

                if (header.Contains(genomeToHuffmanHeader))
                {
                    var keyAndValue = header.Split(':');
                    long huffmanByteNum = long.Parse(keyAndValue[1]);
                    buffer = new byte[huffmanByteNum];

                }
                else
                {

                    throw new IOException("The stream doesnt have any genome huffman header");
                }
                var headerBytesNum = Encoding.Default.GetByteCount(header) ;
                inputStream.Seek(currentPos + headerBytesNum + 1, SeekOrigin.Begin);
                inputStream.Read(buffer, 0, buffer.Length);

                GenomeToHuffman = new SortedDictionary<char, byte>();
                byte code = 0;
                int codeIndex = 0;
                SortedSet<char> sortedSpecialChars = new SortedSet<char>();
                foreach (var charItem in FormatData.specialChars)
                {
                    sortedSpecialChars.Add(charItem);
                }
                foreach (var charItem in sortedSpecialChars)
                {
                    code = buffer[codeIndex];
                    GenomeToHuffman.Add(charItem, code);
                    codeIndex++;
                }
                ReverseGenomeHuffmanTable = new Dictionary<byte, char>();
                foreach (var keyValue in this.GenomeToHuffman)
                {
                    ReverseGenomeHuffmanTable.Add(keyValue.Value, keyValue.Key);

                }

            }


            
        }//end of GenomeHuffmanTable class

        //-------------------------------------------------------------------------------
        /// <summary>
        /// This is a node that the <c>HuffmanTree</c> made of.
        /// It's used to translate bytes to bits sequence when archiving,
        /// and bits sequence to bytes when extracting.
        /// </summary>
        internal class TreeNode
        {
            #region Members

            public TreeNode
                /// <summary>Pointer to the left son.</summery>
                Lson = null,
                /// <summary>Pointer to the right son.</summery>
                Rson = null,
                ///<summery> Pointer to the parent of the node.</summery>
                Parent = null;

            /// <summary>
            /// The Byte value of a leaf, it is relevant only when the node is actualy a leaf.
            /// </summary>
            public Byte ByteValue;

            /// <summary>
            /// This is the frequency value of the node
            /// </summary>
            public ulong Value;

            #endregion
        }//End of TreeNode class
        //-------------------------------------------------------------------------------
        /// <summary>
        /// <c>HuffmanTree</c> is the iplementation of a Huffman algorithm tree.
        /// It's used to translate bytes to bits sequence when archiving,
        /// and bits sequence to bytes when extracting.
        /// </summary>
        internal class HuffmanTree
        {
            #region Members
            /// <summary>
            /// This array hold the value of a byte and it is as long as a frequency table.
            /// </summary>
            public readonly TreeNode[] Leafs;

            /// <summary>The frequency table to build the Huffman tree with.</summary>
            public readonly FrequencyTable FT;

            /// <summary>
            /// This holds nodes without parents;
            /// </summary>
            private ArrayList OrphanNodes = new ArrayList();
            /// <summary>
            /// The root node in the tree to be build;
            /// </summary>
            public readonly TreeNode RootNode;

            #endregion

            /// <summary>Build a Huffman tree out of a frequency table.</summary>
            internal HuffmanTree(FrequencyTable FT)
            {
                ushort Length = (ushort)FT.FoundBytes.Length;
                this.FT = FT;
                Leafs = new TreeNode[Length];
                if (Length > 1)
                {
                    for (ushort i = 0; i < Length; ++i)
                    {
                        Leafs[i] = new TreeNode();
                        Leafs[i].ByteValue = FT.FoundBytes[i];
                        Leafs[i].Value = FT.Frequency[i];
                    }
                    OrphanNodes.AddRange(Leafs);
                    RootNode = BuildTree();
                }
                else
                {//No need to create a tree (only one node below rootnode)
                    TreeNode TempNode = new TreeNode();
                    TempNode.ByteValue = FT.FoundBytes[0];
                    TempNode.Value = FT.Frequency[0];
                    RootNode = new TreeNode();
                    RootNode.Lson = RootNode.Rson = TempNode;
                }
                OrphanNodes.Clear();
                OrphanNodes = null;

            }
            //-------------------------------------------------------------------------------
            /// <summary>
            /// This function build a tree from the frequency table
            /// </summary>
            /// <returns>The root of the tree.</returns>
            private TreeNode BuildTree()
            {
                TreeNode small, smaller, NewParentNode = null;
                /*stop when the tree is fully build( only one root )*/
                while (OrphanNodes.Count > 1)
                {
                    /*This will return the parent less nodes that thier value togather will
                     *be the smallest one and remove them from the ArrayList*/
                    FindSmallestOrphanNodes(out smaller, out  small);
                    NewParentNode = new TreeNode();
                    NewParentNode.Value = small.Value + smaller.Value;
                    NewParentNode.Lson = smaller;
                    NewParentNode.Rson = small;
                    smaller.Parent = small.Parent = NewParentNode;
                    OrphanNodes.Add(NewParentNode);
                }
                //returning the root of the tree (always the last new parent)
                return NewParentNode;
            }
            //-------------------------------------------------------------------------------
            /// <summary>
            /// Finds the smallest and the 2nd smallest value orphan nodes
            /// and removes them them from the arraylist.
            /// </summary>
            /// <param name="Smallest">The smallest node in the <c>OrphanNodes</c> list.</param>
            /// <param name="Small">The 2nd smallest node in the <c>OrphanNodes</c> list.</param>
            private void FindSmallestOrphanNodes(out TreeNode Smallest, out TreeNode Small)
            {
                Smallest = Small = null;
                //Scanning backward
                ulong Tempvalue = 18446744073709551614;
                TreeNode TempNode = null;
                int i, j = 0;
                int ArrSize = OrphanNodes.Count - 1;
                //scanning for the smallest value orphan node
                for (i = ArrSize; i != -1; --i)
                {
                    TempNode = (TreeNode)OrphanNodes[i];
                    if (TempNode.Value < Tempvalue)
                    {
                        Tempvalue = TempNode.Value;
                        Smallest = TempNode;
                        j = i;
                    }
                }
                OrphanNodes.RemoveAt(j);
                --ArrSize;

                Tempvalue = 18446744073709551614;
                //scanning for the second smallest value orphan node
                for (i = ArrSize; i > -1; --i)
                {
                    TempNode = (TreeNode)OrphanNodes[i];
                    if (TempNode.Value < Tempvalue)
                    {
                        Tempvalue = TempNode.Value;
                        Small = TempNode;
                        j = i;
                    }
                }
                OrphanNodes.RemoveAt(j);

            }

            //-------------------------------------------------------------------------------
        }//end of HuffmanTree class

        //-------------------------------------------------------------------------------
        /// <summary>
        /// This is a stack of 8 bits (1 byte)
        /// uses to manipulate the bits of a stream(when been extracted or archived).
        /// It's pushing and poping acts more like a queue then a stack.
        /// </summary>
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

        //-------------------------------------------------------------------------------		
        /// <summary>
        /// This is the file/stream header that attached to each archived file or stream at the begining.
        /// </summary>
        [Serializable]
        internal class FileHeader
        {

            /// <summary>The version of the archiving code.</summary>
            public readonly byte version;

            /// <summary>The frequency table of the archived data.</summary>
            public readonly FrequencyTable FT;

            /// <summary>The size of the data before archiving it.</summary>
            public readonly long OriginalSize;

            /// <summary>Number of extra bits added to the last  byte of the data.</summary>
            public readonly byte ComplementsBits;

            /// <summary>Security key to the archived stream\file.</summary>
            //public readonly ushort Key;

            /// <summary>
            /// Builds a new header that holds info about an archived file\stream.
            /// </summary>
            /// <param name="ver">The version of the archiving program.</param>
            /// <param name="T">The frequency table to rebuild the file from.</param>
            /// <param name="OrgSize">The size of the file\stream before archiving it.</param>
            /// <param name="BitsToFill">
            /// Number of extra bits added to the last byte in the archived file.
            /// </param>
            /// <param name="PasswordKey">Key to gain access to the file\stream later.</param>
            /// 
            /*
             * 
             public FileHeader(Byte ver, FrequencyTable T, ref long OrgSize, 
				byte BitsToFill, ushort PasswordKey)
			{
				version=ver; FT=T; OriginalSize=OrgSize; ComplementsBits=BitsToFill;
				Key = PasswordKey;
			}
		}
             */
            public FileHeader(Byte ver, FrequencyTable T, ref long OrgSize, byte BitsToFill)
            {
                version = ver; FT = T; OriginalSize = OrgSize; ComplementsBits = BitsToFill;

            }
        }
        #endregion
        //-------------------------------------------------------------------------------
        #region Members
        //-------------------------------------------------------------------------------
        /// <summary>
        /// This is a temporary array to sign  where it's location in the 
        /// <c>BuildFrequencyTable</c> function (the value is the location.
        /// </summary>
        private Byte[] ByteLocation = new Byte[256];
        /// <summary>
        /// This array indicated if the byte with the value that correspond
        /// to the index of the array (0-255) was found or not in the stream.
        /// </summary>
        private bool[] IsByteExist;

        /// <summary>Holds the bytes that where found.</summary>
        private List<byte> BytesList = new List<byte>();

        /// <summary>Holds the amount of repetitions of byte.</summary>
        private List<uint> AmountList = new List<uint>();

        /// <summary>I use this list to write the reverse path to a Byte.</summary>
        private List<bool> BitsList = new List<bool>();

        /// <summary>Uses to write and read the Headers to and from a stream.</summary>
        private BinaryFormatter BinFormat = new BinaryFormatter();

        /// <summary>This stack is used to write extracted and shrinked bytes.</summary>
        private BitsStack Stack = new BitsStack();
        public GenomeHuffmanTable genomeHuffmanTable = new GenomeHuffmanTable();

        /// <summary>
        /// Invoked whenever attempt to extract password protected file\stream, by
        /// using the wrong password. In case this event isn't handaled by the users
        /// an exeption will be thrown(in password error case).
        /// </summary>

        /// <summary>
        /// Invoked from all xxxxWithProgress functions whenever another 1 percent
        /// of the function is done.
        /// </summary>
        private PercentCompletedEventHandler OnPercentCompleted;

        #endregion
        //-------------------------------------------------------------------------------
        #region Public Functions
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Build a frequency table and Huffman tree and shrinking the stream data.
        /// </summary>
        /// <param name="Data">
        /// The data streem to shrink, it will start shrinking from the position of the given
        /// stream as it was given and in the end of the function it's position
        /// won't be at the end of the stream and it won't be closed.
        /// </param>
        /// <param name="Password">
        /// A password to add to the archive, to mark as "password less" assign null instead.
        /// </param>
        /// <returns>The archived stream, positioned at start.</returns>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero.
        /// </remarks>


        /// <summary>
        ///  Build a frequency huffman table based on sampling
        ///  

        public void sampleDataAndMakeFrequencyTable(Stream inputStream , double samplingRatio , int filePartitions = 10)
        {

            GenomeHuffmanTable genomeHuffmanTable = new GenomeHuffmanTable();
           
            long currerntPosition = inputStream.Position;
            long DataSize = inputStream.Length - currerntPosition;

            long partitionSize =  DataSize / filePartitions;
            long SampleSizePerParition = (long)(samplingRatio * partitionSize);

            byte[] buffer = new byte[512 * 1024]; // 8 KByte
            int num = 0;
            long bytesToSample =  (long) ((inputStream.Length - currerntPosition) * samplingRatio);
            int index = 0;


            int partitionIndex = 0;

            while (partitionIndex < filePartitions)
            {
                long randomStart = (long)(0.5 * Math.Abs(rand.NextDouble()) * partitionSize );
                if( randomStart + SampleSizePerParition > partitionSize)
                {
                    randomStart = partitionSize - SampleSizePerParition;
                }
                long startingPosition = currerntPosition + partitionIndex * partitionSize + randomStart;
                inputStream.Seek(startingPosition, SeekOrigin.Begin);
                bytesToSample = SampleSizePerParition;
                do
                {
                    num = inputStream.Read(buffer, 0, (int)Math.Min(bytesToSample, buffer.Length));
                    bytesToSample -= num;
                    if (num == 0) break;
                    for (index = 0; index < num; index++)
                    {
                        char charItem = (char)buffer[index];
                        if (FormatData.specialChars.Contains(charItem))
                        {
                            genomeHuffmanTable.Frequency[charItem]++;
                        }
                    }


                } while (bytesToSample > 0);



                partitionIndex++;
            }
            
            
           

            buildHuffmanTableFromFrequency(ref genomeHuffmanTable);
            this.genomeHuffmanTable = genomeHuffmanTable;
            inputStream.Seek(currerntPosition, SeekOrigin.Begin);

        }
        public void sampleDataAndMakeFrequencyTable(Stream inputStream)
        {

            GenomeHuffmanTable genomeHuffmanTable = new GenomeHuffmanTable();

            long currerntPosition = inputStream.Position;

            byte[] buffer = new byte[8 * 1024]; // 8 KByte
            int num = 0;
            long offset = inputStream.Length / 10; // get about 10 samples each of about 100 chars
            if (offset < buffer.Length)
                offset = buffer.Length;
            int sampleSize = 100;
            do
            {
                num = inputStream.Read(buffer, 0, buffer.Length);
                if (num == 0) break;
                int bufferOffset = num / sampleSize;

                for (int index = 0; index < sampleSize; index++)
                {
                    char charItem = ' ';
                    int numOftries = 10; // try to find Genome chars within a try of ten times, if no try works then move to the next offset

                    do
                    {
                        int randLoc = rand.Next(bufferOffset) + bufferOffset * index;
                        byte dataItem = buffer[randLoc];
                        charItem = Encoding.Default.GetChars(new byte[1] { dataItem })[0];
                        numOftries--;
                    } while (!FormatData.specialChars.Contains(charItem) || numOftries > 0);

                    genomeHuffmanTable.Frequency[charItem]++;

                }
                long newPosition = inputStream.Seek(offset, SeekOrigin.Current);

            } while (num > 0);


            buildHuffmanTableFromFrequency(ref genomeHuffmanTable);
            this.genomeHuffmanTable = genomeHuffmanTable;
            inputStream.Seek(currerntPosition, SeekOrigin.Begin);

        }

        private void buildHuffmanTableFromFrequency( ref GenomeHuffmanTable genomeHuffmanTable)
        {
            SortedDictionary<long, char> sortedHuffmanFrequencyTable = new SortedDictionary<long, char>();
            List<Tuple<long, char>> sortedHuffmanList = new List<Tuple<long, char>>();

            

            foreach (var item in genomeHuffmanTable.Frequency)
            {
                sortedHuffmanList.Add(new Tuple<long, char>(item.Value, item.Key));

            }
            sortedHuffmanList.Sort((a, b) => b.Item1.CompareTo(a.Item1));

            byte[] variableHuffman = { 0, 3, 4, 5 }; // 0 11 100 100 101 

            int huffmanArrayIndex = 0;

            foreach (var item in sortedHuffmanList)
            {
                genomeHuffmanTable.GenomeToHuffman[item.Item2] = variableHuffman[huffmanArrayIndex];
                huffmanArrayIndex++;
            }
            genomeHuffmanTable.ReverseGenomeHuffmanTable = new Dictionary<byte, char>();
            foreach (var keyValue in genomeHuffmanTable.GenomeToHuffman)
            {
                genomeHuffmanTable.ReverseGenomeHuffmanTable.Add(keyValue.Value, keyValue.Key);

            }                                   
           
        }
        public void EncodeWithDefaultVariableHuffman(Stream inputStream, string outputFile)
        {

            FileStream outputStream = new FileStream(outputFile, FileMode.Create);

            EncodeWithVariableHuffman(inputStream, outputStream, doSampling: false, samplingRatio:0);

            outputStream.Flush();
            outputStream.Close();
        }


        public void EncodeWithVariableHuffman(Stream inputStream, string outputFile , double samplingRatio , int filePartitions = 10)
        {

            FileStream outputStream = new FileStream(outputFile, FileMode.Create);
            EncodeWithVariableHuffman(inputStream, outputStream, true, samplingRatio, filePartitions);

            outputStream.Flush();
            outputStream.Close();
        }

        public void EncodeWithVariableHuffman(Stream inputStream, Stream outputStream, bool doSampling, double samplingRatio, int filePartitions = 10)
        {


            StreamReader streamReader = new StreamReader(inputStream);

            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            // read the Genome file header, if there is one
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                outputStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);

            }
            if (doSampling)
            {
                sampleDataAndMakeFrequencyTable(inputStream, samplingRatio,filePartitions);
            }

            this.genomeHuffmanTable.writeToStream(outputStream);


            long DataSize = inputStream.Length;
            Byte Original; //the byte we read from the original stream

            int j;


            long bytesToRead = DataSize - getGenomeHeaderEnd;
            byte[] buffer = new byte[512 * 1024];
            List<byte> codingList = new List<byte>();
            BitsStack Stack = new BitsStack();

            while (true)
            {
                int nums = inputStream.Read(buffer, 0, (int)Math.Min(512 * 1024, bytesToRead));
                if (nums == 0)
                {
                    break;
                }
                int index = 0;

                while (index < nums)
                {
                    Original = buffer[index];
                    if (FormatData.specialChars.Contains((char)Original))
                    {

                        char originalChar = (char)Original;
                        bool[] bits = charCodeToVarHuffman(originalChar);
                        bits.Reverse();
                        for (j = 0; j < bits.Length; ++j)
                        {
                            Stack.PushFlag(bits[j]);
                            if (Stack.IsFull())
                            {
                                codingList.Add(Stack.Container);
                                Stack.Empty();
                            }
                        }

                    }
                    index++;
                }


                bytesToRead -= nums;
                byte[] codedBytes = codingList.ToArray();
                outputStream.Write(codedBytes, 0, codedBytes.Length);
                codingList.Clear();

            }


           
            writeBitsComplement(outputStream, Stack);
            outputStream.Flush();
        }
        private void EncodeWithVariableHuffmanWithGZip(Stream inputStream, Stream outputStream, bool doSampling, double samplingRatio, int filePartitions = 10)
        {


            StreamReader streamReader = new StreamReader(inputStream);

            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            int bufferSize = 512 * 1024; ;// 512 K byte
            byte[] buffer = new byte[bufferSize];

            var compressedbuffer = memStream.ToArray();

            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                gzipStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);

            }

            if (doSampling)
            {
                sampleDataAndMakeFrequencyTable(inputStream, samplingRatio,filePartitions);
            }
            this.genomeHuffmanTable.writeToStream(gzipStream);
            long DataSize = inputStream.Length;
            Byte Original; //the byte we read from the original stream

            int j;


            long bytesToRead = DataSize - getGenomeHeaderEnd;
            List<byte> codingList = new List<byte>();
            BitsStack Stack = new BitsStack();

            while (true)
            {
                int nums = inputStream.Read(buffer, 0, (int)Math.Min(512 * 1024, bytesToRead));
                if (nums == 0)
                {
                    break;
                }
                int index = 0;

                while (index < nums)
                {
                    Original = buffer[index];
                    if (FormatData.specialChars.Contains((char)Original))
                    {

                        char originalChar = (char)Original;
                        bool[] bits = charCodeToVarHuffman(originalChar);
                        bits.Reverse();
                        for (j = 0; j < bits.Length; ++j)
                        {
                            Stack.PushFlag(bits[j]);
                            if (Stack.IsFull())
                            {
                                codingList.Add(Stack.Container);
                                Stack.Empty();
                            }
                        }

                    }
                    index++;
                }


                bytesToRead -= nums;
                byte[] codedBytes = codingList.ToArray();

                gzipStream.Write(codedBytes, 0, codedBytes.Length);
                long currentpos = memStream.Position;
                long length = memStream.Length;
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.SetLength(0);
                memStream.Capacity = 10;
                codingList.Clear();

            }


            writeBitsComplement(gzipStream, Stack);

            gzipStream.Flush();
            gzipStream.Close();

            compressedbuffer = memStream.ToArray();
            outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
            outputStream.Flush();
        }


        public void EncodeWithDefaultVariableHuffmanWithGZip(Stream inputStream, Stream outputStream)
        {
            EncodeWithVariableHuffmanWithGZip(inputStream, outputStream, false ,0 );
        }

        public void EncodeWithVariableHuffmanWithGZip(Stream inputStream, Stream outputStream, double samplingRatio, int filePartitions)
        {
            EncodeWithVariableHuffmanWithGZip(inputStream, outputStream, true, samplingRatio,filePartitions);
        }

        public void generateDataWithHuffmanAndSend(Stream outputStream, long dataSize)
        {
            
            generateDataWithHuffmanAndSend(outputStream, dataSize, false, null);
        }



        public void generateDataWithHuffmanAndSend( Stream outputStream, long dataSize , bool doSampling , SamplingPeriod samplingPeriods , int filePartition = 5  , Stream fileStream = null)
        {


            // need to change it so it does smapling in the first 2 minutes of so
            int bufferSize = 512 * 1025; // 512 KB
            long chunkNum = dataSize / bufferSize;
            int extraBytes = (int)dataSize % bufferSize;
            List<byte> data = new List<byte>();
            char[] randomCharBuffer = new char[1];
            initRandomCharBuffer(ref randomCharBuffer, ' ');
            
            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            var compressedbuffer = new byte[1];
            byte[] buffer = new byte[bufferSize];
            byte[] huffmanBuffer = new byte[bufferSize];
            int huffmanBufferIndex = 0;
            TimeSpan samplingPeriod = TimeSpan.Zero; ;
            TimeSpan samplingTime = TimeSpan.Zero;
            this.genomeHuffmanTable.writeToStream(gzipStream);

            List<byte> codingList = new List<byte>();
            BitsStack Stack = new BitsStack();
            Stopwatch timer = new Stopwatch();
            Stopwatch samplingTimer = new Stopwatch();

            GenomeHuffmanTable samplingBasedTable = new GenomeHuffmanTable();
            long totalGBytes = 0;
            char oneChar = ' ';
            bool samplingStarted = false;
            int samplingPeriodsIndex = 0;
            List<byte> dataFileBuffer = new List<byte>();
            if (doSampling)
            {
                timer.Stop();
                samplingPeriod = samplingPeriods.periodsAtTimes[samplingPeriodsIndex].Item1;
                samplingTime = samplingPeriods.periodsAtTimes[samplingPeriodsIndex].Item2;
                samplingTimer.Restart();
            }
            for (int index = 0; index < chunkNum; index++)
            {

                for (int byteIndex = 0; byteIndex < bufferSize; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    oneChar = randomCharBuffer[charIndex];
                    dataFileBuffer.Add((byte)oneChar);
                   
                    totalGBytes++;

                    
                        if (doSampling )
                        {
                            if (samplingTimer.Elapsed.CompareTo(samplingTime) >= 0 && samplingStarted == false)
                            {
                                samplingStarted = true;
                                timer.Restart();
                                samplingTimer.Stop();
                            }
                            if (samplingStarted)
                                samplingBasedTable.Frequency[oneChar]++;
  
                        }
                        // bits are reversed order : MSB comes first and LSB comes last
                        bool[] bits = charCodeToVarHuffman(oneChar);
                        for (int bitIndex = 0; bitIndex < bits.Length; bitIndex++)
                        {
                            Stack.PushFlag(bits[bitIndex]);
                            if (Stack.IsFull())
                            {
                                huffmanBuffer[huffmanBufferIndex] = Stack.Container;
                                huffmanBufferIndex++;
                                Stack.Empty();
                            }
                        }

                    

                }
    
                    gzipStream.Write(huffmanBuffer, 0, huffmanBufferIndex);
                    gzipStream.Flush();
                    compressedbuffer = memStream.ToArray();
                    outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.SetLength(0);
                    memStream.Capacity = 10;
                    huffmanBufferIndex = 0;
                    if (fileStream != null)
                    {
                        fileStream.Write(dataFileBuffer.ToArray(), 0, dataFileBuffer.ToArray().Length);
                        dataFileBuffer.Clear();
                    }
                    if (doSampling  && timer.Elapsed.CompareTo(samplingPeriod) >= 0)
                    {
                        timer.Stop();
                        writeBitsComplement(gzipStream, Stack);
                        Stack.Empty();
                        buildHuffmanTableFromFrequency(ref samplingBasedTable);
                        this.genomeHuffmanTable = samplingBasedTable;
                        this.genomeHuffmanTable.writeToStream(gzipStream);
                        Console.WriteLine("sampling another data at index {0}", samplingPeriodsIndex);
                        samplingPeriodsIndex++;
                        
                        if (samplingPeriodsIndex >= samplingPeriods.periodsAtTimes.Length)
                        {
                            doSampling = false;
                        }
                        else
                        {
                            samplingTime = samplingPeriods.periodsAtTimes[samplingPeriodsIndex].Item2;
                            samplingPeriod = samplingPeriods.periodsAtTimes[samplingPeriodsIndex].Item1;
                            samplingTimer.Restart();
                            timer.Stop();
                            samplingStarted = false;


                        }

                    }

                    continue;
            }

            if (extraBytes > 0)
            {
                for (int byteIndex = 0; byteIndex < extraBytes; byteIndex++)
                {
                    int charIndex = rand.Next() % randomCharBuffer.Length;
                    oneChar = randomCharBuffer[charIndex];
                    dataFileBuffer.Add((byte)oneChar);

                    totalGBytes++;

                    bool[] bits = charCodeToVarHuffman(oneChar);
                    for (int bitIndex = 0; bitIndex < bits.Length;bitIndex++)
                    {
                        Stack.PushFlag(bits[bitIndex]);
                        if (Stack.IsFull())
                        {
                            // codingList.Add(Stack.Container);
                            huffmanBuffer[huffmanBufferIndex] = Stack.Container;
                            huffmanBufferIndex++;
                            Stack.Empty();
                        }
                    }

                }


                    gzipStream.Write(huffmanBuffer, 0, huffmanBufferIndex);
                    compressedbuffer = memStream.ToArray();
                    outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.SetLength(0);
                    memStream.Capacity = 10;
                    codingList.Clear();
                    huffmanBufferIndex = 0;
                    if (fileStream != null)
                    {
                        fileStream.Write(dataFileBuffer.ToArray(), 0, dataFileBuffer.ToArray().Length);
                    }
               

            }

            //Writing the last byte if the stack wasn't compleatly full.
           
                
             writeBitsComplement(gzipStream, Stack);
             Stack.Empty();

            gzipStream.Flush();
            gzipStream.Close();

            compressedbuffer = memStream.ToArray();
            outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
            outputStream.Flush();
        }


        /// <summary>
        /// Initiate a buffer with genome symbols randomly
        /// </summary>
        /// <param name="randomCharBuffer"> the char buffer to be filled</param>
        /// <param name="exceptChar"> a genome symbol that will not be skipped</param>
        private void initRandomCharBuffer(ref char[] randomCharBuffer, char exceptChar = ' ')
        {
            List<char> _specialCharList = new List<char>();


            int index = 0;
            foreach (char item in FormatData.specialChars)
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
                rand.NextBytes(fourBytes);

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



        private void readBitsComplement(Stream outputStream)
        {

        }
        private void readBitsComplement(byte[] buffer)
        {

        }


       

        private void writeBitsComplement(Stream outputStream, BitsStack stack)
        {
            Byte bitsToComplete = (Byte)(8 - stack.NumOfBits());
            if (!stack.IsEmpty())
            {
                for (byte Count = 0; Count < bitsToComplete; Count++) //add extra zero bits to fill a byte
                    stack.PushFlag(false);
                outputStream.WriteByte(stack.Container);
            }
            string header = "\n" + bitsToCompleteHeader;

            byte[] buffer = Encoding.Default.GetBytes(header);
            Array.Resize(ref buffer, buffer.Length + 1);
            buffer[buffer.Length - 1] = bitsToComplete;

            outputStream.Write(buffer, 0, buffer.Length);
        }


        public void decodeWithVariableHuffman(string inputFileName, string outputFile)
        {
            FileStream outputStream = new FileStream(outputFile, FileMode.Create);
            FileStream inputStream = new  FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            decodeWithVariableHuffman(inputStream, outputStream);
            inputStream.Close();
            outputStream.Flush();
            outputStream.Close();
        }


        public void decodeWithVariableHuffman(Stream inputStream, string outputFile)
        {
            FileStream outputStream = new FileStream(outputFile, FileMode.Create);
            decodeWithVariableHuffman(inputStream, outputStream);

            outputStream.Flush();
            outputStream.Close();
        }

        public void decodeWithVariableHuffman(Stream inputStream, Stream outputStream)
        {

            StreamReader streamReader = new StreamReader(inputStream);

            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            // read the Genome file header, if there is one
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                outputStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);

            }

            this.genomeHuffmanTable.readFromStream(inputStream);
            long DataSize = inputStream.Length;
            Byte currentByte; //the byte we read from the original stream

            long currentPosition = inputStream.Position;

            long bytesToRead = DataSize - currentPosition;// getGenomeHeaderEnd - this.genomeHuffmanTable.GenomeToHuffman.Count;// -bitsToCompleteHeader.Length - 2; // two bytes for bits complement and the bytes itself
            byte[] buffer = new byte[512 * 1024];
            List<byte> codingList = new List<byte>();
            BitsStack Stack = new BitsStack();
            List<bool> bitsList = new List<bool>();
            int nums = 0;
            long totalBytes = 0;
            var headerLength = Encoding.Default.GetByteCount(bitsToCompleteHeader);
            while (true)
            {
                nums = inputStream.Read(buffer, 0, (int)Math.Min(512 * 1024, bytesToRead));
                bytesToRead -= nums;

                if (nums == 0 )
                {
                    break;
                }
                int index = 0;

                while (index < nums)
                {
                    currentByte = buffer[index];
                    
                    byte nextChar = 0;
                     if ((index + 1) < nums )
                    {
                        nextChar = buffer[index + 1];
                    }
                     if (nextChar == '\n')
                     {
                         // we have bits complement and then it is followed by another genome huffman table
                         // need to bits complement first and then genome huffman table
                         string getBitsToComplementHeader = "";
                         if (index + headerLength + 2 > nums)
                         {

                             // should read from the next buffer or wait until its available in case of read on fly
                             // change index to zero
                             int remainingBytes = nums - index -1; 
                             var tempBuffer = new byte[remainingBytes];
                             Array.Copy(buffer, index + 1, tempBuffer, 0, remainingBytes);
                             nums = inputStream.Read(buffer, remainingBytes, (int)Math.Min(512 * 1024, bytesToRead) - remainingBytes);
                             Array.Copy(tempBuffer, 0, buffer, 0, remainingBytes);
                             bytesToRead -= nums;
                             index = -1; 
                             nums += remainingBytes;
                             getBitsToComplementHeader = Encoding.Default.GetString(buffer, 1 , headerLength); // dont inculde \n as  it was copied 
                         }
                         else 
                         {
                             getBitsToComplementHeader = Encoding.Default.GetString(buffer, index + 2, headerLength);

                         }
                         if (bitsToCompleteHeader.Contains(getBitsToComplementHeader))
                         {
                             index += headerLength + 2; // \n + bitcomplementsheader + 1 to get the location of bitsToComplete

                             byte bitsToComplete = buffer[index ];
                             Stack.FillStack(currentByte);
                             if (bitsToComplete == 8)
                                 bitsToComplete = 0;
                             for (int bitIndex = 0; bitIndex < 8 - bitsToComplete; bitIndex++)
                             {
                                 bool bit = Stack.PopFlag();
                                 bitsList.Add(bit);
                                 var charCode = varHuffmanToChar(bitsList);
                                 if (charCode != ' ')
                                 {
                                     codingList.Add((byte)charCode);
                                     totalBytes++;
                                     bitsList.Clear();
                                 }
                             }
                             Stack.Empty();
                             index++;
                             if (index < nums)
                             {
                                 int offest = this.genomeHuffmanTable.readFromBuffer(buffer, index);
                                 if (offest == -1)
                                 {
                                     int remainingBytes = nums - index ;
                                     var tempBuffer = new byte[remainingBytes];
                                     Array.Copy(buffer, index, tempBuffer, 0, remainingBytes);
                                     nums = inputStream.Read(buffer, remainingBytes, (int)Math.Min(512 * 1024, bytesToRead) - remainingBytes);
                                     Array.Copy(tempBuffer, 0, buffer, 0, remainingBytes);
                                     bytesToRead -= nums;
                                     nums += remainingBytes;
                                     offest = this.genomeHuffmanTable.readFromBuffer(buffer, index);
                                     if (offest == 0)
                                     {
                                         continue;
                                     }

                                 }
                                 index += offest ;
                             }


                             continue;
                         }

                     }
                     
                     
                         Stack.FillStack(currentByte);

                         while (!Stack.IsEmpty())
                         {
                             bool bit = Stack.PopFlag();
                             bitsList.Add(bit);
                             var charCode = varHuffmanToChar(bitsList);
                             if (charCode != ' ')
                             {
                                 codingList.Add((byte)charCode);
                                 totalBytes++;
                                 bitsList.Clear();
                             }
                         }
                     
                    index++;
                }


                byte[] codedBytes = codingList.ToArray();
                outputStream.Write(codedBytes, 0, codedBytes.Length);
                codingList.Clear();

            }
           
            outputStream.Flush();
        }


        //-------------------------------------------------------------------------------
        /// <summary>
        /// Build a frequency table and Huffman tree and shrinking the stream data.
        /// This function version, calls the PercentComplete event handler
        /// When anothe 1 percent compleated.
        /// </summary>
        /// <param name="Data">
        /// The data streem to shrink, shrinking starts from the position of the given stream
        /// as it was given and in the end of the function it's position won't be at the end
        /// of the stream and it won't be closed.
        /// </param>
        /// <param name="Password">
        /// A password to add to the archive, to mark as "password less" assign null instead.
        /// </param>
        /// <returns>The archived stream, positioned at start.</returns>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero.
        /// </remarks>

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Build a frequency table and Huffman tree and shrinking the stream data to a new file.
        /// into a file.
        /// </summary>
        /// <param name="Data">
        /// The data streem to shrink, it will start shrinking from the position of the given
        /// stream as it was given and in the end of the function it's position
        /// won't be at the end of the stream and it won't be closed.
        /// </param>
        /// <param name="OutputFile">Path to a file to same the shrinked data in.</param>
        /// <param name="Password">
        /// A passward to add to the archive, to mark as "passward less" assign null instead.
        /// </param>
        /// <returns>The expanded stream, positioned at start.</returns>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero.
        /// </remarks>
        /// 




        public void Shrink(Stream Data, string OutputFile)
        {
            FileStream outputStream = new FileStream(OutputFile, FileMode.Create);
            Shrink(Data, outputStream);
            outputStream.Flush();
            outputStream.Close();
        }

        public void Shrink(Stream inputStream, Stream outputStream)
        {


            StreamReader streamReader = new StreamReader(inputStream);

            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                outputStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);

            }
            //Generating the header data from the stream and creating a HuffmanTree

            HuffmanTree HT = new HuffmanTree(BuildFrequencyTable(inputStream));
            //Writing  header
            WriteHeader(outputStream, HT.FT, inputStream.Length, 11, GetComplementsBits(HT));
            long DataSize = inputStream.Length;
            TreeNode TempNode = null;
            Byte Original; //the byte we read from the original stream

            short j; int k;


            // start tweaking 
            long bytesToRead = DataSize - getGenomeHeaderEnd;
            byte[] buffer = new byte[512 * 1024];
            List<byte> codingList = new List<byte>();
            long bytesRead = 0;
            while (true)
            {
                int nums = inputStream.Read(buffer, 0, (int)Math.Min(512 * 1024, bytesToRead));
                if (nums == 0)
                {
                    break;
                }
                int index = 0;

                while (index < nums)
                {
                    Original = buffer[index];
                    if (FormatData.specialChars.Contains((char)Original))
                    {
                    
                    
                        TempNode = HT.Leafs[ByteLocation[Original]];
                        while (TempNode.Parent != null)
                        {
                            //If I'm left sone of my parent push 1 else push 0
                            BitsList.Add(TempNode.Parent.Lson == TempNode);
                            TempNode = TempNode.Parent;//Climb up the tree.
                        }
                        BitsList.Reverse();
                        k = BitsList.Count;
                        for (j = 0; j < k; ++j)
                        {
                            Stack.PushFlag(BitsList[j]);
                            if (Stack.IsFull())
                            {
                                //outputStream.WriteByte(Stack.Container);
                                codingList.Add(Stack.Container);
                                Stack.Empty();
                            }
                        }
                       
                    }
                    BitsList.Clear();
                    index++;
                    bytesRead++;
                }


                bytesToRead -= nums;
                byte[] codedBytes = codingList.ToArray();
                outputStream.Write(codedBytes, 0, codedBytes.Length);
                codingList.Clear();

            }

          
            //Writing the last byte if the stack wasn't compleatly full.
            if (!Stack.IsEmpty())
            {
                Byte BitsToComplete = (Byte)(8 - Stack.NumOfBits());
                for (byte Count = 0; Count < BitsToComplete; ++Count)//complete to full 8 bits
                    Stack.PushFlag(false);
                outputStream.WriteByte(Stack.Container);
                Stack.Empty();
            }
            outputStream.Flush();
            //outputStream.Seek(0, SeekOrigin.Begin);
        }


        // ----------

        // Shrink With Gzip Compression directly 

        public void ShrinkWithGzipCompression(Stream inputStream, Stream outputStream)
        {

            StreamReader streamReader = new StreamReader(inputStream);

            MemoryStream memStream = new MemoryStream();
            GZipStream gzipStream = new GZipStream(memStream, CompressionMode.Compress, true);
            int bufferSize = 512 * 1024; ;// 512 K byte
            byte[] buffer = new byte[bufferSize];

            var compressedbuffer = memStream.ToArray();
            
              

            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                gzipStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);

            }
            //Generating the header data from the stream and creating a HuffmanTree

            HuffmanTree HT = new HuffmanTree(BuildFrequencyTable(inputStream));
            //Writing  header
            WriteHeader(gzipStream, HT.FT, inputStream.Length, 11, GetComplementsBits(HT));
            long DataSize = inputStream.Length;
            TreeNode TempNode = null;
            Byte Original; //the byte we read from the original stream

            short j; int k;


            // start tweaking 
            long bytesToRead = DataSize - getGenomeHeaderEnd;
            List<byte> codingList = new List<byte>();
            long bytesRead = 0;
            while (true)
            {
                int nums = inputStream.Read(buffer, 0, (int)Math.Min(512 * 1024, bytesToRead));
                if (nums == 0)
                {
                    break;
                }
                int index = 0;

                while (index < nums)
                {
                    Original = buffer[index];
                    if (FormatData.specialChars.Contains((char)Original))
                    {


                        TempNode = HT.Leafs[ByteLocation[Original]];
                        while (TempNode.Parent != null)
                        {
                            //If I'm left sone of my parent push 1 else push 0
                            BitsList.Add(TempNode.Parent.Lson == TempNode);
                            TempNode = TempNode.Parent;//Climb up the tree.
                        }
                        BitsList.Reverse();
                        k = BitsList.Count;
                        for (j = 0; j < k; ++j)
                        {
                            Stack.PushFlag(BitsList[j]);
                            if (Stack.IsFull())
                            {
                                //outputStream.WriteByte(Stack.Container);
                                codingList.Add(Stack.Container);
                                Stack.Empty();
                            }
                        }

                    }
                    BitsList.Clear();
                    index++;
                    bytesRead++;
                }

                bytesToRead -= nums;
                byte[] codedBytes = codingList.ToArray();
                gzipStream.Write(codedBytes, 0, codedBytes.Length);
                long currentpos = memStream.Position;
                long length = memStream.Length;
                compressedbuffer = memStream.ToArray();
                outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                memStream.SetLength(0);
                memStream.Capacity = 10;
                codingList.Clear();

            }


            //Writing the last byte if the stack wasn't compleatly full.
            if (!Stack.IsEmpty())
            {
                Byte BitsToComplete = (Byte)(8 - Stack.NumOfBits());
                for (byte Count = 0; Count < BitsToComplete; ++Count)//complete to full 8 bits
                    Stack.PushFlag(false);
                gzipStream.WriteByte(Stack.Container);
                Stack.Empty();
            }


            gzipStream.Flush();
            gzipStream.Close();

            compressedbuffer = memStream.ToArray();
            outputStream.Write(compressedbuffer, 0, compressedbuffer.Length);
            outputStream.Flush();
            //outputStream.Seek(0, SeekOrigin.Begin);
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Build a frequency table and Huffman tree and shrinking the stream data to a new file.
        /// into a file.
        /// This function version, calls the PercentComplete event handler
        /// When anothe 1 percent compleated.
        /// </summary>
        /// <param name="Data">
        /// The data streem to shrink, shrinking starts from the position of the given stream
        /// as it was given and in the end of the function it's position won't be at the end
        /// of the stream and it won't be closed.
        /// </param>
        /// <param name="OutputFile">Path to a file to same the shrinked data in.</param>
        /// <param name="Password">
        /// A passward to add to the archive, to mark as "passward less" assign null instead.
        /// </param>
        /// <returns>The expanded stream, positioned at the start.</returns>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero.
        /// </remarks>
        public  void ShrinkWithProgress(Stream Data, string OutputFile)
        {
            //Generating the header data from the stream and creating a HuffmanTree
            HuffmanTree HT = new HuffmanTree(BuildFrequencyTable(Data));
            //Creating temporary file
            FileStream outputStream = new FileStream(OutputFile, FileMode.Create);
            //Writing  header 
            WriteHeader(outputStream, HT.FT, Data.Length, 11, GetComplementsBits(HT));

            long DataSize = Data.Length;
            TreeNode TempNode = null;
            Byte Original; //the byte we read from the original stream

            short j; int k; float ProgressRatio = 0;
            for (long i = 0; i < DataSize; ++i)
            {
                Original = (Byte)Data.ReadByte();
                TempNode = HT.Leafs[ByteLocation[Original]];
                while (TempNode.Parent != null)
                {
                    //If I'm left sone of my parent push 1 else push 0
                    BitsList.Add(TempNode.Parent.Lson == TempNode);
                    TempNode = TempNode.Parent;//Climb up the tree.
                }
                BitsList.Reverse();
                k = BitsList.Count;
                for (j = 0; j < k; ++j)
                {
                    Stack.PushFlag(BitsList[j]);
                    if (Stack.IsFull())
                    {
                        outputStream.WriteByte(Stack.Container);
                        Stack.Empty();
                    }
                }
                BitsList.Clear();

                if (((float)(i)) / DataSize - ProgressRatio > 0.01)
                {
                    ProgressRatio += 0.01f;
                    if (OnPercentCompleted != null) OnPercentCompleted();
                }
            }

            //Writing the last byte if the stack wasn't compleatly full.
            if (!Stack.IsEmpty())
            {
                Byte BitsToComplete = (Byte)(8 - Stack.NumOfBits());
                for (byte Count = 0; Count < BitsToComplete; ++Count)//complete to full 8 bits
                    Stack.PushFlag(false);
                outputStream.WriteByte(Stack.Container);
                Stack.Empty();
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            outputStream.Close();
        }

        //-------------------------------------------------------------------------------



        /// <summary>
        /// Build a frequency table and Huffman tree and extract the archive to a new file.
        /// </summary>
        /// <param name="inputStream">The data streem to shrink</param>
        /// <param name="OutputFile">Path to to save the extracted data.</param>
        /// <param name="Password">
        /// A Key to open the archive with, to mark as "passward less" assign null instead.
        /// </param>
        /// <returns>
        /// flag that indicates if extraction went well or not: true = successful,
        /// false = wrong password error occured (the WrongPassword event will take place). 
        /// </returns>
        /// <exception cref="Exception">
        /// On attempt to extract data that wasn't coded with the <c>HuffmanAlgorithm</c> class, 
        /// Or on attempt to extract a password protected stream\file with the wrong password.
        /// (If the <c>WrongPassword</c> event is been handeled by the user the 2nd exception is'nt
        /// relevant).
        /// </exception>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero. The given stream must be archived stream of the right type.
        /// </remarks>
        /// 
        // To read from file 
        public void Extract(Stream inputStream, string OutputFile)
        {
            FileStream outputStream = new FileStream(OutputFile, FileMode.Create);
            StreamReader streamReader = new StreamReader(inputStream);
            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                outputStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }
            else
            {
                getGenomeHeaderEnd = 0;
                inputStream.Seek(0, SeekOrigin.Begin);

            }
            FileHeader Header;

            //Reading the header data from the stream
            if (!IsArchivedStream(inputStream)) throw new Exception("The given stream is't my Huffman algorithm type.");
            Header = (FileHeader)BinFormat.Deserialize(inputStream);

            //Gernerating Huffman tree out of the frequency table in the header
            HuffmanTree HT = new HuffmanTree(Header.FT);
            //Creating temporary file
            BitsStack Stack = new BitsStack();
            long DataSize = inputStream.Length - inputStream.Position;
            // if (Header.ComplementsBits == 0) DataSize += 1;
            TreeNode TempNode = null;


            long bytesToRead = DataSize;
            byte[] buffer = new byte[512 * 1024];


            int nums = inputStream.Read(buffer, 0, buffer.Length);
            int index = 0;
            bytesToRead -= nums;

            long bytesWritten = 0;

            List<byte> decodingList = new List<byte>();


            while (true)
            {
                TempNode = HT.RootNode;

                //As long it's not a leaf, go down the tree
                while (TempNode.Lson != null && TempNode.Rson != null)
                {
                    //If the stack is empty refill it.
                    if (Stack.IsEmpty())
                    {
                        byte mByte = buffer[index];
                        index++;
                        Stack.FillStack(mByte);
                        if (index >= nums)
                        {
                            var decodingBytes = decodingList.ToArray();
                            outputStream.Write(decodingBytes, 0, decodingBytes.Length);
                            nums = inputStream.Read(buffer, 0, buffer.Length);
                            bytesToRead -= nums;
                            index = 0;
                            decodingList.Clear();
                        }
                        if (nums == 0)
                        {
                            goto AlmostDone;
                        }
                    }
                    //Going left or right according to the bit
                    TempNode = Stack.PopFlag() ? TempNode.Lson : TempNode.Rson;
                }
                //By now reached for a leaf and writes it's data.
                //  outputStream.WriteByte(TempNode.ByteValue);
                bytesWritten++;
                decodingList.Add(TempNode.ByteValue);
            }//end of while

            //To this lable u can jump only from the while loop (only one byte left).
        AlmostDone:

            short BitsLeft = (Byte)(Stack.NumOfBits() - Header.ComplementsBits);
            // something wrong happens when numofbits = 8 and header.complementsBits = 8 too
            //Writing the rest of the last byte.
            //  BitsLeft = 8;

            // need to fix it 
            if (BitsLeft != 0 )
            {
                bool Test = TempNode.Lson == null && TempNode.Rson == null;
                while (BitsLeft > 0)
                {
                    //If at itteration, TempNode not done going down the huffman tree.
                    if (Test) TempNode = HT.RootNode;
                    while (TempNode.Lson != null && TempNode.Rson != null)
                    {
                        //Going left or right according to the bit
                        TempNode = Stack.PopFlag() ? TempNode.Lson : TempNode.Rson;
                        --BitsLeft;
                    }
                    //By now reached for a leaf and writes it's data.
                    outputStream.WriteByte(TempNode.ByteValue);
                    bytesWritten++;

                    Test = true;
                }
            }
            outputStream.Flush();
            outputStream.Close();
            return; // true;
        }

         public void Extract(string inputFileName, string OutputFileName)
         {
             FileStream inputStream;
             inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read);

             Extract(inputStream, OutputFileName);
             inputStream.Flush();
             inputStream.Close();
         
         }
        //


        //-------------------------------------------------------------------------------

        /// <summary>
        /// Build a frequency table and Huffman tree and extract the archive to a new file.
        /// This function version, calls the PercentComplete event handler
        /// When anothe 1 percent compleated.
        /// </summary>
        /// <param name="inputStream">The data streem to shrink</param>
        /// <param name="OutputFile">Path to to save the extracted data.</param>
        /// <param name="Password">
        /// A Key to open the archive with, to mark as "passward less" assign null instead.
        /// </param>
        /// <returns>
        /// flag that indicates if extraction went well or not: true = successful,
        /// false = wrong password error occured (the WrongPassword event will take place). 
        /// </returns>
        /// <exception cref="Exception">
        /// On attempt to extract data that wasn't coded with the <c>HuffmanAlgorithm</c> class, 
        /// Or on attempt to extract a password protected stream\file with the wrong password.
        /// (If the <c>WrongPassword</c> event is been handeled by the user the 2nd exception is'nt
        /// relevant).
        /// </exception>
        /// <remarks>
        /// The given stream must be readable, seekable and it's length
        /// must exceed zero. The given stream must be archived stream of the right type.
        /// </remarks>
        public bool ExtractWithProgress(Stream inputStream, string OutputFile, char[] Password)
        {
            FileStream outputStream = new FileStream(OutputFile, FileMode.Create);
            StreamReader streamReader = new StreamReader(inputStream);
            char[] charBuffer = new char[1024 * 4];
            var charNums = streamReader.Read(charBuffer, 0, charBuffer.Length);
            var getGenomeHeaderEnd = FormatData.getFileHeaderEnd(charBuffer);
            if (getGenomeHeaderEnd > 0)
            {
                getGenomeHeaderEnd++;
                byte[] genomeHeader = Encoding.Default.GetBytes(charBuffer, 0, (int)getGenomeHeaderEnd);
                outputStream.Write(genomeHeader, 0, genomeHeader.Length);
                inputStream.Seek(getGenomeHeaderEnd, SeekOrigin.Begin);
            }

            //intputStream.Seek(0, SeekOrigin.Begin);
            FileHeader Header;

            //Reading the header data from the stream
            if (!IsArchivedStream(inputStream)) throw new Exception("The given stream is't my Huffman algorithm type.");
            Header = (FileHeader)BinFormat.Deserialize(inputStream);


            //Gernerating Huffman tree out of the frequency table in the header
            HuffmanTree HT = new HuffmanTree(Header.FT);
            //Creating temporary file
            BitsStack Stack = new BitsStack();
            long DataSize = inputStream.Length - inputStream.Position;

           // if (Header.ComplementsBits == 0) DataSize += 1;
            TreeNode TempNode = null;
            long DataSize2 = DataSize; float ProgressRatio = 0;//Needed to calculate progress.

            while (true)
            {
                TempNode = HT.RootNode;
                //As long it's not a leaf, go down the tree
                while (TempNode.Lson != null && TempNode.Rson != null)
                {
                    //If the stack is empty refill it.
                    if (Stack.IsEmpty())
                    {
                        Stack.FillStack((Byte)inputStream.ReadByte());
                        if ((--DataSize) == 0)
                            goto AlmostDone;
                    }
                    //Going left or right according to the bit
                    TempNode = Stack.PopFlag() ? TempNode.Lson : TempNode.Rson;
                }
                //By now reached for a leaf and writes it's data.
                outputStream.WriteByte(TempNode.ByteValue);

                if (((float)(DataSize2 - DataSize)) / DataSize2 - ProgressRatio > 0.01)
                {
                    ProgressRatio += 0.01f;
                    if (OnPercentCompleted != null) OnPercentCompleted();
                }

            }//end of while

            //To this lable u can jump only from the while loop (only one byte left).
        AlmostDone:

            short BitsLeft = (Byte)(Stack.NumOfBits() - Header.ComplementsBits);

            //Writing the rest of the last byte.
            if (BitsLeft != 8)
            {
                bool Test = TempNode.Lson == null && TempNode.Rson == null;
                while (BitsLeft > 0)
                {
                    //If at itteration, TempNode not done going down the huffman tree.
                    if (Test) TempNode = HT.RootNode;
                    while (TempNode.Lson != null && TempNode.Rson != null)
                    {
                        //Going left or right according to the bit
                        TempNode = Stack.PopFlag() ? TempNode.Lson : TempNode.Rson;
                        --BitsLeft;
                    }
                    //By now reached for a leaf and writes it's data.
                    outputStream.WriteByte(TempNode.ByteValue);
                    Test = true;
                }
            }

            outputStream.Close();
            return true;
        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Checks if a data stream is archived.
        /// </summary>
        /// <param name="inputStream">The stream to test.</param>
        /// <returns>true if the stream is archive, false if not.</returns>
        public bool IsArchivedStream(Stream inputStream)
        {
           // Data.Seek(0, SeekOrigin.Begin);
            long currentPosition = inputStream.Position;
            bool test = true;
            try
            {
                FileHeader Header = (FileHeader)BinFormat.Deserialize(inputStream);
                Header = null;
            }
            catch (Exception)
            {
                //if header wasn't found
                test = false;
            }
            finally
            {
                inputStream.Seek(currentPosition, SeekOrigin.Begin);
            }
            return test;
        }


        //-------------------------------------------------------------------------------
        /// <summary>
        /// This function calculates the the archiving ratio of a given archived stream. 
        /// </summary>
        /// <param name="Data">Archived stream to calculate archiving ratio to.</param>
        /// <returns>The archiving ratio.</returns>
        /// <exception cref="Exception">
        /// When the given stream isn't correct Huffman archived stream or has been corrupted.
        /// </exception>
        public float GetArchivingRatio(Stream Data)
        {
            Data.Seek(0, SeekOrigin.Begin);
            float Result;
            try
            {
                FileHeader Header = (FileHeader)BinFormat.Deserialize(Data);
                Result = (100f * Data.Length) / Header.OriginalSize;
                Header = null;
            }
            catch (Exception)
            {
                //if header wasn't found
                throw new Exception("Error, the given stream isn't Huffman archived or corrupted.");
            }
            finally
            {
                Data.Seek(0, SeekOrigin.Begin);
            }
            return Result;
        }

        #endregion
      
        
        
        //-------------------------------------------------------------------------------
        #region Public events


        //-------------------------------------------------------------------------------
        /// <summary>
        /// This is Asynchronic event and invoked only from xxxxWithProgress functions.
        /// Invoked whenever another 1 percent of the function is done.
        /// </summary>
        [Description("This is Asynchronic event and invoked only from xxxxWithProgress " +
             "functions. Invoked whenever another 1 percent of the function is done.")
        ]
        [Category("Action")]
        public event PercentCompletedEventHandler PercentCompleted
        {
            add { OnPercentCompleted += value; }
            remove { OnPercentCompleted -= value; }
        }

        #endregion
        //-------------------------------------------------------------------------------
        #region Private Functions
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Scanning for repeated bytes and according to them build frequency table.
        /// </summary>
        /// <param name="DataSource">The stream to build <c>FrequencyTable</c> for.</param>
        /// <returns>The generated frequency table.</returns>
        /// <remarks>
        /// DataSource must be readable and seekable stream.
        /// Don't try to extract somthing smaller then 415 bytes(it's not worth it)
        /// </remarks>
        private FrequencyTable BuildFrequencyTable(Stream DataSource)
        {
            long OriginalPosition = DataSource.Position;
            FrequencyTable FT = new FrequencyTable();
            IsByteExist = new bool[256]; //false by default

            Byte bTemp;
            //Counting bytes and saving them
            // tweak this to read chunk of data 
            long bytesToRead = DataSource.Length - OriginalPosition ;
            byte[] buffer = new byte[512 * 1024];// 4Kbytes
             int nums = 0;
            while (true)
            {
               nums = DataSource.Read(buffer, 0, (int)Math.Min(bytesToRead, 512 * 1024));

                if (nums == 0) break;
                int index = 0;
                while (index < nums)
                {
                    bTemp = buffer[index];
                    char currentChar = (char)bTemp;
                    if (FormatData.specialChars.Contains(currentChar))
                    {
                        if (IsByteExist[bTemp]) //If the byte was found before increase the repeatition
                            AmountList[ByteLocation[bTemp]]++;// = (uint)AmountList[ByteLocation[bTemp]] + 1;
                        else/*If new byte*/
                        {
                            IsByteExist[bTemp] = true; //Mark as found
                            ByteLocation[bTemp] = (Byte)BytesList.Count; //Save the new location of the byte in the bouth ArrayLists
                            AmountList.Add(1u); //Marking that one was found
                            BytesList.Add(bTemp);
                        }
                    }
                    index++;
                }
                bytesToRead -= nums;
            }



            int ArraySize = BytesList.Count;
           
            //Copy the list to arrays;
            
            FT.FoundBytes = BytesList.ToArray();
            FT.Frequency = AmountList.ToArray();
            //sort the arrays according to the Frequency
            SortArrays(FT.Frequency, FT.FoundBytes,(short) ArraySize);

            //Cleaning resources
            IsByteExist = null;
            BytesList.Clear();
            AmountList.Clear();
            DataSource.Seek(OriginalPosition, SeekOrigin.Begin);
            return FT;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// This function takes a password cstring and converts it to a ushort number
        /// that's fit the header of a shrinked file.
        /// </summary>
        /// <param name="Password">
        /// Password to a shrinked file (8 chars tops), is it's null, no password will
        /// take place in the file (zero value).
        /// </param>
        /// <returns>A numeric representation of the given password.</returns>
        /// <exception cref="Exception">
        /// When a given password is longer then 8 characters.
        /// </exception>
        private ushort PasswordGen(char[] Password)
        {
            if (Password == null) return 0;

            if (Password.Length > 8)
                throw new Exception("Password's is 8 chars length tops, you've entered " +
                    Password.Length + " bytes.");
            Byte Size = (byte)Password.Length;
            ushort Result = 0;
            for (Byte i = 0; i < Size; ++i)
                Result += (ushort)((Password[i] + 1) * i);

            return Result;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Bouble sort <c>FrequencyTable</c>( according frequency level )
        /// and making the same changes on the corresponding array.
        /// </summary>
        /// <param name="SortTarget">The array to sort by.</param>
        /// <param name="TweenArray">
        /// The array that will take the same swaping as the target array.
        /// </param>
        private void SortArrays(uint[] SortTarget, Byte[] TweenArray, short size)
        {
            --size;
            bool TestSwitch = false;
            Byte BTemp;
            uint uiTemp;
            short i, j;
            for (i = 0; i < size; ++i)
            {
                for (j = 0; j < size; ++j)
                {
                    if (SortTarget[j] < SortTarget[j + 1])
                    {
                        TestSwitch = true;//Making switch action
                        uiTemp = SortTarget[j];
                        SortTarget[j] = SortTarget[j + 1];
                        SortTarget[j + 1] = uiTemp;
                        //Doing same to corresponding array
                        BTemp = TweenArray[j];
                        TweenArray[j] = TweenArray[j + 1];
                        TweenArray[j + 1] = BTemp;
                    }
                }//end of for
                if (!TestSwitch) break;//if no switch action in this round, no need for more.
                TestSwitch = false;
            }//end of for
            for (i = 0; i < SortTarget.Length; ++i)
                ByteLocation[TweenArray[i]] = (Byte)i;

        }

        //-------------------------------------------------------------------------------
        /// <summary>
        /// Write a header to the stream. This header is vital when extracting the data.
        /// </summary>
        /// <param name="St">The output stream.</param>
        /// <param name="FT">The frequency table of the data.</param>
        /// <param name="OriginalSize">The original of the data before shrinking.</param>
        /// <param name="version">The version of the shrinking code.</param>
        /// <param name="ComplementsBits">Number of extra bits added to the last byte of the data.</param>
        /// <param name="Password">A key to gain access to the archived file\stream.</param>
        private void WriteHeader(Stream St, FrequencyTable FT, long OriginalSize,
            Byte version, Byte ComplementsBits)
        {
            FileHeader Header = new FileHeader(version, FT, ref OriginalSize,
                ComplementsBits);
            BinFormat.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            BinFormat.Serialize(St, Header);
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Calculates the amount of complements bits, needed for the last byte writing.
        /// </summary>
        /// <param name="HT">The huffman tree of the stream to be archived.</param>
        /// <returns>Amount of complements bits</returns>
        private Byte GetComplementsBits(HuffmanTree HT)
        {
            //Getting the deapth of each leaf in the huffman tree
            short i = (short)HT.Leafs.Length;
            ushort[] NodesDeapth = new ushort[i];
            long SizeInOfBits = 0;
            while (--i != -1)
            {
                TreeNode TN = HT.Leafs[i];
                while (TN.Parent != null)
                {
                    TN = TN.Parent;
                    ++NodesDeapth[i];
                }
                SizeInOfBits += NodesDeapth[i] * HT.FT.Frequency[i];
            }
            byte complementBits = (byte)(8 - SizeInOfBits % 8);
            return complementBits == 8 ? (byte)0 : complementBits;
        }

        #endregion
        //-------------------------------------------------------------------------------
        #region IDisposable Members

        public void Dispose()
        {
            BytesList = null;
            IsByteExist = null;
            AmountList = null;
            BinFormat = null;
            BitsList = null;
            ByteLocation = null;
            OnPercentCompleted = null;
        }

        #endregion
        //-----------------------------------------------------------------------------
     




        private  char varHuffmanToChar(List<bool> bits)
        {

            byte data = 0;

            for (int index = 0; index < bits.Count; index++)
            {
                data <<= 1;
                if (bits[index])
                {
                    data |= 1;
                }
            }


           
           
           
            if (this.genomeHuffmanTable.ReverseGenomeHuffmanTable.ContainsKey(data))
            {
                return this.genomeHuffmanTable.ReverseGenomeHuffmanTable[data];
            }
           

            return ' '; // 


        }


        private  bool[] charCodeToVarHuffman(char code)
        {
            var byteCode = genomeHuffmanTable.GenomeToHuffman[code];
            var result = new bool[1] { false };

            if (byteCode == 0)
            {
                 result = new bool[1] { false }; //0
            }
            else if (byteCode == 3)
            {
                 result = new bool[2] { true, true }; //11
            }
            else if (byteCode == 4)
            {
                 result = new bool[3] { true,false ,false }; //100 
            }
            else if (byteCode == 5)
            {
                 result = new bool[3] { true, false, true }; //101
            }
            return result;


           
        }

    
    
    }




}
