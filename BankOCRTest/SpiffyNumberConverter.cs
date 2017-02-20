using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOCRTest
{
    public class SpiffyNumberConverter
    {
        public static string Convert(List<SpiffyNumber> readNumbers)
        {
            if (AccountWasReadWithoutAmbiguity(readNumbers))
            {
                return AggregateEachNumberAndCalculateChecksum(readNumbers);
            }
            if (AccountContainsUnrecognizedCharacters(readNumbers))
            {
                return AggregateNumbersAndQuestionMarks(readNumbers);
            }
            if (NumberContainsAmbiguousNumbers(readNumbers))
            {
                return GetAccountStatusForMatches(readNumbers);
            }
            return "";
        }

        private static string GetAccountStatusForMatches(List<SpiffyNumber> readNumbers)
        {
            var matchingNumbers = DetectPotentialMatches(readNumbers);
            var validChecksumAccounts = matchingNumbers.Where(IsValidateAccountNumber).ToList();
            if (validChecksumAccounts.Count == 1)
                return validChecksumAccounts[0];
            if (validChecksumAccounts.Count > 1)
                return validChecksumAccounts[0] + " AMB";
            if (!validChecksumAccounts.Any())
                return matchingNumbers[0] + " ERR";
            return "";
        }
        private static bool NumberContainsAmbiguousNumbers(List<SpiffyNumber> readNumbers)
        {
            return readNumbers.Any(num => num.Status == Status.Ambiguous);
        }

        private static string AggregateNumbersAndQuestionMarks(List<SpiffyNumber> readNumbers)
        {
            string result = "";
            foreach (var spiffyNumber in readNumbers)
            {
                if (spiffyNumber.Status == Status.FatalError)
                    result += "?";
                else
                    result += spiffyNumber.ReadNumbers[0];
            }
            return result + " ILL";
        }

        private static bool AccountContainsUnrecognizedCharacters(List<SpiffyNumber> readNumbers)
        {
            return readNumbers.Any(num => num.Status == Status.FatalError);
        }

        private static string AggregateEachNumberAndCalculateChecksum(List<SpiffyNumber> readNumbers)
        {
            string result = "";
            foreach (var spiffyNumber in readNumbers)
            {
                result += spiffyNumber.ReadNumbers[0];
            }
            if (!IsValidateAccountNumber(result))
            {
                result += " ERR";
            }
            return result;
        }

        private static bool AccountWasReadWithoutAmbiguity(List<SpiffyNumber> readNumbers)
        {
            return readNumbers.All(num => num.Status == Status.Success);
        }

        private static List<string> DetectPotentialMatches(List<SpiffyNumber> readNumbers)
        {
            var result = new List<string> {""};

            string accountTemplate = "";
            foreach (var readNumber in readNumbers)
            {
                if (readNumber.Status == Status.Success)
                {
                    result= (ConcatStringToList(result, readNumber.ReadNumbers[0]));
                }
                else if (readNumber.Status == Status.Ambiguous)
                {
                    var newResults = new List<List<string>>();
                    foreach (var potentialNumber in readNumber.ReadNumbers)
                    {
                        var newListWithPotentialNumber = ConcatStringToList(result, potentialNumber);
                        newResults.Add(newListWithPotentialNumber);
                    }
                    result.Clear();
                    newResults.ForEach(result.AddRange);
                }
            }
            return result;
        }



        private static List<string> ConcatStringToList(List<string> inputList, string stringToAdd)
        {
            var result = new List<string>();
            foreach (var element in inputList)
            {
                result.Add(element + stringToAdd);
            }
            return result;
        }

        public static bool IsValidateAccountNumber(int accountNumber)
        {
            int checksum = CalculateChecksum(accountNumber);
            return checksum == 0;
        }

        private static int CalculateChecksum(int accountNumber)
        {
            int checksum = 0;
            var accountNumberCharacters = accountNumber.ToString().ToCharArray().Reverse();
            int i = 1;
            foreach (var numberCharacter in accountNumberCharacters)
            {
                checksum += Int32.Parse(numberCharacter.ToString()) * i;
                i += 1;
            }
            return checksum % 11;
        }
        private static bool IsValidateAccountNumber(string accountNumber)
        {
            return IsValidateAccountNumber(Int32.Parse(accountNumber));
        }
    }
}