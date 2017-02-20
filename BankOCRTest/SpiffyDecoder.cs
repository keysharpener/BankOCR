using System.Collections.Generic;
using System.IO;

namespace BankOCRTest
{
    public class SpiffyDecoder
    {
        public List<string> Scan(string[] fileContent)
        {
            var result = new List<string>();
            var readNumbers = new LinesReader().ReadLines(fileContent);
            foreach (List<SpiffyNumber> spiffyNumbers in readNumbers)
            {
                var accountOuput = SpiffyNumberConverter.Convert(spiffyNumbers);
                result.Add(accountOuput);
            }
            return result;
        }
    }


    public class SpiffyNumber
    {
        public List<string> ReadNumbers { get; set; } = new List<string>();
        public Status Status { get; set; } = Status.Success;

    }

    public enum Status
    {
        Ambiguous,
        FatalError,
        Success
    }
}