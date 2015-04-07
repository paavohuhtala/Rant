using System.IO;
using System.Linq;
using NUnit.Framework;
using Rant.Vocabulary;

namespace Rant.Tests
{
    [TestFixture]
    public class DictionaryRef
    {
        [Test]
        public void Basic()
        {
            var dict = RantDictionary.FromFile("resources/references.dic", NsfwFilter.Allow);
            var entries = dict.GetTables().First().GetEntries().SelectMany(entry => entry.Terms).Select(term => term.Value).ToList();

            Assert.AreEqual(entries.Count, 12);

            Assert.Contains("berry", entries);
            Assert.Contains("berries", entries);

            Assert.Contains("raspberry", entries);
            Assert.Contains("raspberries", entries);

            Assert.Contains("strawberry", entries);
            Assert.Contains("strawberries", entries);

            Assert.Contains("entryA_A entryB_A", entries);
            Assert.Contains("entryA_B entryB_B", entries);
        }

        [Test]
        [ExpectedException(typeof(InvalidDataException))]
        public void Invalid()
        {
            var dict = RantDictionary.FromFile("resources/references-invalid.dic", NsfwFilter.Allow);
        }
    }
}