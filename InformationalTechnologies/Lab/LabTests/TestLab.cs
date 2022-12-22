namespace LabTests;
using Lab;

public class TestLab
{
    public class TestValidation
    {
        private Column _column;

        public TestValidation(Column column)
        {
            _column = column;
        }

        [Fact]
        public void TestIntColumnValidation1()
        {
            _column = new IntColumn("");
            Assert.True(_column.Validate("42"));
        }

        [Fact]
        public void TestIntColumnValidation2()
        {
            _column = new IntColumn("");
            Assert.False(_column.Validate("str"));
        }

        [Fact]
        public void TestRealColumnValidation1()
        {
            _column = new RealColumn("");
            Assert.True(_column.Validate("234.79"));
        }

        [Fact]
        public void TestRealColumnValidation2()
        {
            _column = new RealColumn("");
            Assert.False(_column.Validate("234d79"));
        }

        [Fact]
        public void TestCharColumnValidation1()
        {
            _column = new CharColumn("");
            Assert.True(_column.Validate("C"));
        }

        [Fact]
        public void TestCharColumnValidation2()
        {
            _column = new CharColumn("");
            Assert.False(_column.Validate("jkg"));
        }

        [Fact]
        public void TestStringColumnValidation()
        {
            _column = new StringColumn("");
            Assert.True(_column.Validate("str"));
        }

        [Fact]
        public void TestIntIntervalColumnValidation1()
        {
            _column = new IntIntervalColumn("");
            Assert.True(_column.Validate("2, 5"));
        }

        [Fact]
        public void TestIntIntervalColumnValidation2()
        {
            _column = new IntIntervalColumn("");
            Assert.False(_column.Validate("6, 3"));
        }

        [Fact]
        public void TestIntIntervalColumnValidation3()
        {
            _column = new IntIntervalColumn("");
            Assert.False(_column.Validate("2.4, 3"));
        }
    }

    public class TestProjection
    {
        [Fact]
        private void TestProjection1()
        {
            var database = new Database("");
            var table = new Table("");
            var column1 = new IntColumn("C1");
            var column2 = new StringColumn("C2");
            var column3 = new CharColumn("C3");
            var row1 = new Row
            {
                Values = new List<string> { "1", "abc", "A" }
            };
            var row2 = new Row
            {
                Values = new List<string> { "42", "qq", "J" }
            };

            database.Tables.Add(table);
            database.Tables[0].Columns.Add(column1);
            database.Tables[0].Columns.Add(column2);
            database.Tables[0].Columns.Add(column3);
            database.Tables[0].Rows.Add(row1);
            database.Tables[0].Rows.Add(row2);

            var projection = new Table("")
            {
                Columns = new List<Column> { column1, column3 },
                Rows = new List<Row>
                {
                    new Row { Values = new List<string> { "1", "A" } },
                    new Row { Values = new List<string> { "42", "J" } }
                }
            };

            DatabaseManager.Instance.Database = database;
            var testProjection = DatabaseManager.Instance.Project(0, new int[] { 0, 2 });
            Assert.Equal(projection.Columns[0], testProjection.Columns[0]);
            Assert.Equal(projection.Columns[1], testProjection.Columns[1]);
            Assert.Equal(projection.Rows[0].Values[0], testProjection.Rows[0].Values[0]);
            Assert.Equal(projection.Rows[0].Values[1], testProjection.Rows[0].Values[1]);
            Assert.Equal(projection.Rows[1].Values[0], testProjection.Rows[1].Values[0]);
            Assert.Equal(projection.Rows[1].Values[1], testProjection.Rows[1].Values[1]);
        }
    }
}