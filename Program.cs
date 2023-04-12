// See https://aka.ms/new-console-template for more information


using System;
using System.Data;
using Npgsql;

class Sample
{
    static void Main(string[] args)
    {
        // Connect to a PostgreSQL database
        NpgsqlConnection conn = new NpgsqlConnection("Server=127.0.0.1:(Your servers here);User Id=postgres; " +
           "Password=(Your password here);Database=prods;");
        conn.Open();

        // Define a query returning a single row result set
        NpgsqlCommand product = new NpgsqlCommand("SELECT * FROM product", conn);
        NpgsqlCommand customer = new NpgsqlCommand("SELECT * FROM customer", conn);


        NpgsqlDataReader reader = product.ExecuteReader();
        DataTable product_table = new DataTable();
        product_table.Load(reader);

        NpgsqlDataReader reads = customer.ExecuteReader();
        DataTable customer_table = new DataTable();
        customer_table.Load(reads);
        problem_7(product_table);
        Console.WriteLine("----------------");
        problem_20(customer_table);
        Console.WriteLine("----------------");
        version2_of20(customer_table);
        Console.WriteLine("----------------");
        version3_of_20(customer_table);
        conn.Close();

        static void print_results(DataTable data)
        {
            Console.WriteLine();
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in data.Columns)
            {
                Console.Write(col.ColumnName);
                var maxLabelSize = data.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++) Console.Write(" ");
            }

            Console.WriteLine();

            foreach (DataRow dataRow in data.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    Console.Write(dataRow.ItemArray[j]);
                    for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 14; i++) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        static void version3_of_20(DataTable data)
        {
            Console.WriteLine("problem 20.3");
            // Group the data by rep_id and calculate the sum of cust_balance for each group
            var results = data.AsEnumerable()
                              .GroupBy(r => {
                                  int rep_id;
                                  int.TryParse(r.Field<string>("rep_id"), out rep_id);
                                  return rep_id;
                              })
                              .Select(g => new
                              {
                                  rep_id = g.Key,
                                  Total_balance = g.Sum(r => r.Field<decimal>("cust_balance"))
                              });
            // Filter the results based on the sum of cust_balance and representative id, and sort by rep_id
            var filteredResults = results.Where(r => r.Total_balance > 12000 && r.rep_id > 0)
                                          .OrderBy(r => r.rep_id);
            // Print the results
            foreach (var result in filteredResults)
            {
                Console.WriteLine("Rep ID: {0}, Total Balance: {1}", result.rep_id, result.Total_balance);
            }
        }

        static void version2_of20(DataTable data)
        {
            Console.WriteLine("problem 20.2");
            Dictionary<int, int> rep_balances = new Dictionary<int, int>();
            foreach (DataRow dataRow in data.Rows)
            {
                int rep_id = Convert.ToInt32(dataRow["rep_id"]);
                int cust_balance = Convert.ToInt32(dataRow["cust_balance"]);

                if (rep_balances.ContainsKey(rep_id))
                {
                    int previous_balance = rep_balances[rep_id];
                    rep_balances[rep_id] = previous_balance + cust_balance;
                }
                else
                {
                    rep_balances.Add(rep_id, cust_balance);
                }
            }

            DataTable filteredDataTable = data.Clone();

            foreach (KeyValuePair<int, int> kvp in rep_balances)
            {
                if (kvp.Value > 12000)
                {
                    DataRow[] filteredRows = data.Select("rep_id = " + kvp.Key);
                    foreach (DataRow row in filteredRows)
                    {
                        filteredDataTable.ImportRow(row);
                    }
                }
            }

            print_results(filteredDataTable);

        }

        static void problem_7(DataTable data)
        {
            Console.WriteLine("problem 7");
            // Define the filter condition
            string filterCondition = "prod_quantity > 12 AND prod_quantity < 30";

           //Select the rows that match the filter condition
            DataRow[] filteredRows = data.Select(filterCondition);

            //Create a new DataTable with only the filtered rows
            DataTable filteredDataTable = data.Clone();
            foreach (DataRow row in filteredRows)
            {
                filteredDataTable.ImportRow(row);
            }
            print_results(filteredDataTable);
        }
        //This is Kelby's version as a base for what I needed to alter to make a more concise method
        static void problem_20(DataTable data)
        {
            Console.WriteLine("Problem 20.1");
            Dictionary<string, int> colWidths = new Dictionary<string, int>();
            foreach (DataColumn col in data.Columns)
            {
                if ((col.ColumnName).ToString() == "rep_id")
                {
                    //Displaying rep_id
                    Console.Write(col.ColumnName);
                    var maxLabelSize = data.Rows.OfType<DataRow>()
                            .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                            .OrderByDescending(m => m).FirstOrDefault();

                    colWidths.Add(col.ColumnName, maxLabelSize);
                    for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 14; i++)
                    {
                        Console.Write(" ");
                    }


                    //Displaying balance_sum
                    Console.Write("balance_sum");
                    colWidths.Add("balance_sum", maxLabelSize);
                    for (int i = 0; i < maxLabelSize - "balance_sum".Length + 14; i++)
                    {
                        Console.Write(" ");
                    }
                }

                Console.Write("");
            }
            Console.WriteLine();

            //Creating a dicitonary of rep_id > cust_balance values so that we can create a sum of values
            Dictionary<int, int> rep_balances = new Dictionary<int, int>();



            //Obtaining row data
            foreach (DataRow dataRow in data.Rows)
            {
                int rep_id = Convert.ToInt16(dataRow.ItemArray[8]);
                int currCustBalance = Convert.ToInt16(dataRow.ItemArray[6]);

                if (rep_balances != null && rep_balances.ContainsKey(rep_id))
                {
                    int previous_value = rep_balances[rep_id];
                    rep_balances[rep_id] = previous_value + currCustBalance;
                }
                else if (rep_balances != null && !rep_balances.ContainsKey(rep_id))
                {
                    rep_balances.Add(rep_id, currCustBalance);
                }

            }

            foreach (KeyValuePair<int, int> pair in rep_balances)
            {
                if (pair.Value > 12000)
                {
                    Console.Write(pair.Key.ToString() + "              " + pair.Value);
                    Console.WriteLine();
                }
            }
        }
    }
}