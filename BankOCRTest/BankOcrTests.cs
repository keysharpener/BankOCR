using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;

namespace BankOCRTest
{
    public class BankOcrTests
    {
        [TestCase("NumberFiles/Zero.txt", "0")]
        [TestCase("NumberFiles/One.txt", "1")]
        [TestCase("NumberFiles/Two.txt", "2")]
        [TestCase("NumberFiles/Three.txt", "3")]
        [TestCase("NumberFiles/Four.txt", "4")]
        [TestCase("NumberFiles/Five.txt", "5")]
        [TestCase("NumberFiles/Six.txt", "6")]
        [TestCase("NumberFiles/Seven.txt", "7")]
        [TestCase("NumberFiles/Eight.txt", "8")]
        [TestCase("NumberFiles/Nine.txt", "9")]
        public void Should_return_the_right_number_in_the_specified_file(string filePath, string expected)
        {
            var spiffyMachineOutput = File.ReadAllText(filePath);
            var spiffyDecoder = new LinesReader();
            var actual = spiffyDecoder.GetCorrespondingNumberFromASCII(spiffyMachineOutput);
            Check.That(actual.ReadNumbers[0]).IsEqualTo(expected);
        }

        [Test]
        public void Should_return_right_number_for_multiple_characters_test_case()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/123TestCase.txt"));
            Check.That(actual).ContainsExactly("123 ERR");
        }

        [Test]
        public void Should_validate_the_account_number_345882865()
        {
            Check.That(SpiffyNumberConverter.IsValidateAccountNumber(345882865)).IsTrue();
        }

        [Test]
        public void Should_replace_number_by_question_mark_when_unable_to_parse_number()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var fileContent = File.ReadAllLines("TestsInputs/1X3TestCase.txt");
            var actual = spiffyDecoder.Scan(fileContent);
            Check.That(actual).ContainsExactly("1?3 ILL");
        }

        [Test]
        public void Should_return_error_status_for_a_wrong_sum()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/WrongNumber.txt"));
            Check.That(actual).ContainsExactly("123456788 ERR");
        }
        [Test]
        public void Should_return_no_error_status_for_good_account_number()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/GoodNumber.txt"));
            Check.That(actual).ContainsExactly("123456789");
        }

        [Test]
        public void Should_return_ill_status_when_no_matches_found()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/NoMatches.txt"));
            Check.That(actual).ContainsExactly("?2345678? ILL");
        }

        [Test]
        public void Should_read_multiple_accounts_in_a_single_file()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/MultipleAccounts.txt"));
            var expected = new List<string>
            {
                "123456789",
                "123456788 ERR",
                "1?3 ILL"
            };
            Check.That(actual).ContainsExactly(expected);
        }
        [Test]
        public void Should_read_one_account_in_a_single_file_with_bad_checksum()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/ErrorNeedsToRespectChecksum.txt"));
            var expected = new List<string>
            {
                "133456788 ERR"
            };
            Check.That(actual).ContainsExactly(expected);
        }

        [Test]
        public void Should_read_one_account_in_a_single_file_with_one_more_pipe_and_one_more_underscore()
        {
            var spiffyDecoder = new SpiffyDecoder();
            var actual = spiffyDecoder.Scan(File.ReadAllLines("TestsInputs/GoodNumberModulo1.txt"));
            var expected = new List<string>
            {
                "123456789"
            };
            Check.That(actual).ContainsExactly(expected);
        }
    }
}
