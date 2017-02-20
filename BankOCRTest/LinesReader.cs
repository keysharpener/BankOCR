using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankOCRTest
{
    public class LinesReader
    {
        private readonly Dictionary<string, string> _numberDictionary;

        public LinesReader()
        {
            _numberDictionary = new Dictionary<string, string>();
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Zero.txt"), "0");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/One.txt"), "1");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Two.txt"), "2");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Three.txt"), "3");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Four.txt"), "4");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Five.txt"), "5");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Six.txt"), "6");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Seven.txt"), "7");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Eight.txt"), "8");
            _numberDictionary.Add(File.ReadAllText("NumberFiles/Nine.txt"), "9");
        }
        public List<List<SpiffyNumber>> ReadLines(string[] fileContent)
        {
            var readNumbers = new List<List<SpiffyNumber>>();
            for (int i = 0; i < fileContent.Count(); i = i + 4)
            {
                var currentLines = new string[] { fileContent[i], fileContent[i + 1], fileContent[i + 2], fileContent[i + 3] };
                var numbersCorrespondingToLine = Read(currentLines);
                readNumbers.Add(numbersCorrespondingToLine);
            }
            return readNumbers;
        }

        private List<SpiffyNumber> Read(string[] inputCharacters)
        {
            var firstLine = IsolateLine(inputCharacters, 0);
            var secondLine = IsolateLine(inputCharacters, 1);
            var thirdLine = IsolateLine(inputCharacters, 2);

            var result = new List<SpiffyNumber>();
            for (int i = 0; i < firstLine.Count; i++)
            {
                result.Add(GetCorrespondingNumberFromASCII(firstLine[i] + secondLine[i] + thirdLine[i]));
            }
            return result;
        }

        private static List<string> IsolateLine(string[] inputCharacters, int lineIndex)
        {
            List<string> parsedLine = new List<string>();
            for (int i = 0; i < inputCharacters[lineIndex].Length; i = i + 3)
            {
                var stringSubPart = inputCharacters[lineIndex].Substring(i, 3) + "\r\n";
                parsedLine.Add(stringSubPart);
            }
            return parsedLine;
        }

        public SpiffyNumber GetCorrespondingNumberFromASCII(string inputCharacters)
        {
            var readNumber = new SpiffyNumber();
            if (_numberDictionary.ContainsKey(inputCharacters))
                readNumber.ReadNumbers.Add(_numberDictionary[inputCharacters]);
            else
            {
                foreach (var asciiNumber in _numberDictionary.Keys)
                {
                    if (HasOnlyOneTolerableDifferenceWithNormalASCII(inputCharacters, asciiNumber))
                    {
                        readNumber.ReadNumbers.Add(_numberDictionary[asciiNumber]);
                    }
                }
                if (!readNumber.ReadNumbers.Any())
                {
                    readNumber.Status = Status.FatalError;
                }
                if (readNumber.ReadNumbers.Count > 1)
                {
                    readNumber.Status = Status.Ambiguous;
                }
            }
            return readNumber;
        }

        private bool HasOnlyOneTolerableDifferenceWithNormalASCII(string inputCharacters, string asciiNumber)
        {
            var tolerableDifferences = new List<char> { '_', '|', ' ' };
            int differences = 0;
            for (int i = 0; i < inputCharacters.Length - 1; i++)
            {
                if (inputCharacters[i] != asciiNumber[i])
                {
                    if (tolerableDifferences.Contains(inputCharacters[i]))
                        differences++;
                    else
                    {
                        return false;
                    }
                }
                if (differences > 1)
                    return false;
            }
            return true;
        }

    }
}
