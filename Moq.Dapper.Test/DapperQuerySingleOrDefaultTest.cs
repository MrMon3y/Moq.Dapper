using Dapper;
using NUnit.Framework;
using System;
using System.Data;

namespace Moq.Dapper.Test
{
    public class DapperQuerySingleOrDefaultTest
    {
        [Test]
        public void QuerySingleOrDefaultInterface()
        {
            var connection = new Mock<IDbConnection>();

            var expected = 7;

            connection.SetupDapper(c =>
                c.QuerySingleOrDefault<int>(It.IsAny<string>(), null, null, null, null)).Returns(expected);

            var actual = connection.Object.QuerySingleOrDefault<int>("");

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void QuerySingleOrDefaultInterfaceComplexType()
        {
            var connection = new Mock<IDbConnection>();

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

            connection.SetupDapper(c =>
                    c.QuerySingleOrDefault<ComplexType>(It.IsAny<string>(), null, null, null, null))
                      .Returns(expected);

            var actual = connection.Object
                .QuerySingleOrDefault<ComplexType>("");

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