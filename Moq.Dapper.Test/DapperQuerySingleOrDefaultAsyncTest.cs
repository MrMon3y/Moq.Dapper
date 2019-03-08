using Dapper;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;

namespace Moq.Dapper.Test
{
    [TestFixture]
    public class DapperQuerySingleOrDefaultAsyncTest
    {
        [Test]
        public void QuerySingleOrDefaultAsyncGeneric()
        {
            var connection = new Mock<DbConnection>();

            var expected = 7;

            connection.SetupDapperAsync(c =>
                c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), null, null, null, null)).ReturnsAsync(expected);

            var actual = connection.Object.QuerySingleOrDefaultAsync<int>("").GetAwaiter().GetResult();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void QuerySingleOrDefaultAsyncInterface()
        {
            var connection = new Mock<IDbConnection>();

            var expected = 7;

            connection.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(expected);

            var actual = connection.Object.QuerySingleOrDefaultAsync<int>("").GetAwaiter().GetResult();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void QuerySingleOrDefaultAsyncGenericComplexType()
        {
            var connection = new Mock<DbConnection>();

            var expected = new ComplexType
            {
                StringProperty = "String1",
                IntegerProperty = 7,
                GuidProperty = Guid.Parse("CF01F32D-A55B-4C4A-9B33-AAC1C20A85BB"),
                DateTimeProperty = new DateTime(2000, 1, 1),
                NullableDateTimeProperty = new DateTime(2000, 1, 1),
                NullableIntegerProperty = 9,
                ByteArrayPropery = new byte[] { 7 }
            };

            connection.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<ComplexType>(It.IsAny<string>(), null, null, null, null))
                      .ReturnsAsync(expected);

            var actual = connection.Object
                                   .QuerySingleOrDefaultAsync<ComplexType>("")
                                   .GetAwaiter()
                                   .GetResult();

            var match = actual.StringProperty == expected.StringProperty &&
                        actual.IntegerProperty == expected.IntegerProperty &&
                        actual.GuidProperty == expected.GuidProperty &&
                        actual.DateTimeProperty == expected.DateTimeProperty &&
                        actual.NullableIntegerProperty == expected.NullableIntegerProperty &&
                        actual.NullableDateTimeProperty == expected.NullableDateTimeProperty &&
                        actual.ByteArrayPropery == expected.ByteArrayPropery;

            Assert.True(match);
        }
    }
}