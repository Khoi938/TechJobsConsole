using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            // invoke to load data into AllJobs and return it
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();
            string lowerInput = value.ToLower();
            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column].ToLower();
                // for each dictionary 'row' in Alljobs. avalue = row[key]
                // if the key contains value the row is added to jobs
                if (aValue.Contains(lowerInput))
                {
                    jobs.Add(row);
                }
            }

            return jobs;
        }
        public static List<Dictionary<string, string>> FindByValue(string input)
        {
            LoadData();
            string lowerinput = input.ToLower();
            List<Dictionary<string, string>> listing = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> row in AllJobs)
            {
                foreach (KeyValuePair<string,string> entry in row)
                {
                    string lowerValue = entry.Value.ToLower();
                    if (lowerValue.Contains(lowerinput))
                    {
                        listing.Add(row);
                        break;
                    }
                }
            }
            return listing;       
        }
        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }
            // Create a List "rows" with data type String Array
            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                 // StreamReader.Peek check if there is another char to be read.else = -1
                 // if it found something then it used SR.ReadLine(). Move Pointer to next line.
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    // check to see if rowArray is empty.
                    // if not add the newsly formed arry to rows
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }
            // incase the importing the file failed.
            if (rows.Count < 1)
            {
                return;
            }
            // Remove index 0 of List<string[]> rows "Code OG"
            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            // using the value taken from rows[0] above
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();
                // There is a bug that crashed the program when some value is missing
                // Added Code to replaced missing Value
                if (headers.Length > row.Length)
                {
                    string[] newRow = new string[headers.Length];
                    for (int i = 0; i < row.Length; i++)
                    {
                        newRow[i] = row[i];
                    }
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (newRow[i]==null)
                        {
                            newRow[i] = "N/A";
                        }
                        rowDict.Add(headers[i], newRow[i]);
                    }
                }
                else // Unmodified code below
                {
                    // pairing the Key header[i] to value row[i]
                    for (int i = 0; i < headers.Length; i++)
                    {
                        rowDict.Add(headers[i], row[i]);
                    }
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                 // if char = ',' And isBetweenQuotes = False, StringBuilder to string and add 
                 // String array.
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    // if the char equal " then flip NOT(false) to True. Append Nothing and move on
                    // as in it is between quotes /vice versa.
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    // if char is does not equal to " append char to var a StringBuilder Object
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
