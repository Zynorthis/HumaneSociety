using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace HumaneSociety
{
    public static class CSVConvertion
    {
        internal static void CSVInput()
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(@"..\..\..\animals.csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        Animal animal = new Animal();
                        var line = parser.ReadLine();
                        var values = line.Split(new[] { ',', '/', '"' }, StringSplitOptions.RemoveEmptyEntries);

                        if (values[1] != "null")
                        {
                            animal.Name = values[0];
                        }
                        if (values[2] != "null")
                        {
                            animal.CategoryId = Int32.Parse(values[1]);
                        }
                        if (values[3] != "null")
                        {
                            animal.Weight = Int32.Parse(values[2]);
                        }
                        if (values[4] != "null")
                        {
                            animal.Age = Int32.Parse(values[3]);
                        }
                        if (values[5] != "null")
                        {
                            animal.DietPlanId = Int32.Parse(values[4]);
                        }
                        if (values[7] != "null")
                        {
                            animal.Demeanor = values[5];
                        }
                        if (values[8] != "null")
                        {
                            bool booleanTest = false;
                            if(values[8] == "1")
                            {
                                booleanTest = false;
                            }
                            else if (values[8] == "0")
                            {
                                booleanTest = true;
                            }
                            animal.KidFriendly = booleanTest;
                        }
                        if (values[9] != "null")
                        {
                            bool booleanTest = false;
                            if (values[9] == "1")
                            {
                                booleanTest = false;
                            }
                            else if (values[9] == "0")
                            {
                                booleanTest = true;
                            }
                            animal.PetFriendly = booleanTest;
                        }
                        if (values[10] != "null")
                        {
                            animal.Gender = values[8];
                        }
                        if (values[12] != "null")
                        {
                            animal.AdoptionStatus = values[9];
                        }
                        if (values[13] != "null")
                        {
                            animal.EmployeeId = Int32.Parse(values[10]);
                        }
                        Query.AddAnimal(animal);
                    }
                }
            }
            catch
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                UserInterface.DisplayUserOptions("CSV Inport Error: could not read file stream.");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
        internal static void CSVOutput()
        {
            // setting up headers to the table
            var csv = new StringBuilder();
            using (var writer = new StringWriter())
            {
                // setting up headers to the table
                var header1 = "Employee ID";
                var header2 = "First Name";
                var header3 = "Last Name";
                var header4 = "Username";
                var header5 = "Employee Number";
                var header6 = "Email";
                var headerLine = string.Format("{0},{1},{2},{3},{4},{5}", header1, header2, header3, header4, header5, header6);
                csv.Append(headerLine);

                // importing data
                HumaneSocietyDataContext db = new HumaneSocietyDataContext();
                foreach (var item in db.Employees)
                {
                    var cell1 = item.EmployeeId.ToString();
                    var cell2 = item.FirstName;
                    var cell3 = item.LastName;
                    var cell4 = item.UserName;
                    var cell5 = item.EmployeeNumber.ToString();
                    var cell6 = item.Email;
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5}", cell1, cell2, cell3, cell4, cell5, cell6);
                    csv.Append(newLine);
                }
                writer.WriteLine();
                File.AppendAllText(@"..\..\..\Test.csv", csv.ToString());
            }
        }
    }
}
