using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleSoccer.Net
{
    class ParameterFileBase
    {
        private string _fileBuffer;
        private bool _eof;
        private StringReader _reader;

        public ParameterFileBase(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Invalid parameter file.", filename);
            }

            FileStream stream = File.Open(filename, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            _fileBuffer = reader.ReadToEnd();
            reader.Close();
            stream.Close();
            
            // set eof flag accordingly - if empty file
            _eof = (_fileBuffer.Length == 0);

            _reader = new StringReader(_fileBuffer);
        }

        protected T GetNextParameter<T>()
        {
            string parameterValue = readNextParameter();

            T returnValue = default(T);
            if (!string.IsNullOrEmpty(parameterValue))
            {
                try
                {
                    returnValue = (T)Convert.ChangeType(parameterValue, typeof(T));
                }
                catch (InvalidCastException)
                {
                    returnValue = default(T);
                }
            }

            return returnValue;
        }
        
        /// <summary>
        ///  searches the text file for the next valid parameter. Discards any comments
        ///  and returns the value as a string
        /// 
        /// </summary>
        /// <returns></returns>
        private string readNextParameter()
        {

            //this will be the string that holds the next parameter
            string line = getNextLine();

            if (line == null )
                line = string.Empty;

            line = stripComments(line);

            //if the line is of zero length, get the next line from
            //the file
            if (line.Length == 0)
            {
                if (!_eof)
                {
                    return readNextParameter();
                }
                else
                {
                    return string.Empty;
                }
            }

            return getParameterValue(line);
        }

        private string getNextLine()
        {
            string line = string.Empty;
            if (!_eof)
            {
                if ((line = _reader.ReadLine()) != null)
                {
                    // strip cr/lf
                    line.Replace("/r", "");
                    line.Replace("/n", "");
                }
                else
                {
                    _eof = true;
                }
            }
            return line;
        }

        private string stripComments(string line)
        {
            //search for any comment and remove
            int idx = line.IndexOf("//");

            if (idx != -1)
            {
                //cut out the comment
                line = line.Substring(0, idx);
            }

            return line;
        }

        private string getParameterValue(string line)
        {
            //find beginning of parameter description
            int begIdx = 0, endIdx = 0;

            //define some delimiters
            char[] delimiterArray = new char[] { ' ', '\\', ';', '=', ',' };

            List<char> delims = new List<char>();
            delims.AddRange(delimiterArray);

            begIdx = findFirstNotOf(line, delims, 0);

            //find the end of the parameter description
            if (begIdx != -1)
            {
                endIdx = line.IndexOfAny(delimiterArray, begIdx);

                //end of word is the end of the line
                if (endIdx == -1)
                {
                    endIdx = line.Length;
                }
            }

            //find the beginning of the parameter value
            begIdx = findFirstNotOf(line, delims, endIdx);

            //find the end of the parameter value
            if (begIdx != -1)
            {
                endIdx = line.IndexOfAny(delimiterArray, begIdx);

                //end of word is the end of the line
                if (endIdx == -1)
                {
                    endIdx = line.Length;
                }
            }

            return line.Substring(begIdx, endIdx - begIdx);
        }

        private int findFirstNotOf(string line, List<char> delims, int startIndex)
        {
            int foundIndex = -1;

            int currentIndex = startIndex;
            while (currentIndex < line.Length && foundIndex == -1)
            {
                if (!delims.Contains(line[currentIndex]))
                {
                    // found it so bail
                    foundIndex = currentIndex;
                }

                currentIndex++;
            }

            return foundIndex;
        }
    }
}
