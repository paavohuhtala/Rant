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
            var rant = new RantEngine("resources/");
            var dict = (RantDictionary) rant.Dictionary;
            var entries = dict.GetTables().First().GetEntries().SelectMany(entry => entry.Terms).Select(term => term.Value).ToList();

            Assert.AreEqual(entries.Count, 6);

            Assert.Contains("berry", entries);
            Assert.Contains("berries", entries);

            Assert.Contains("raspberry", entries);
            Assert.Contains("raspberries", entries);

            Assert.Contains("strawberry", entries);
            Assert.Contains("strawberries", entries);
        }
    }
}