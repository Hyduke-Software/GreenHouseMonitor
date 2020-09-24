using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
namespace ConsoleApp1
{

    static class Configuration
    {

        // Default folder    
        public static string rootFolder = @"C:\Temp\";
        //Default file. MAKE SURE TO CHANGE THIS LOCATION AND FILE PATH TO YOUR FILE   
        static public string textFile = @"C:\Temp\sensorConfig.txt";
        public static Dictionary<string, bool> configValuesType = new Dictionary<string, bool>(); //method of marking the expected value as int or char. TRUE = INT, FALSE =CHAR
        public static Dictionary<string, string> configValues = new Dictionary<string, string>();



        public static void getConfig()
        {
            //default vaulues
            configValuesType.Add("Baudrate:", true);
            configValuesType.Add("Comport:", false);
            configValuesType.Add("Server:", false);
            configValuesType.Add("Username:", false);
            configValuesType.Add("Password:", false);
            configValuesType.Add("Pollrate:", true);

            configValues.Add("Baudrate:", "9600");      //0
            configValues.Add("Comport:", "COM1");       //1
            configValues.Add("Server:", "Server1");     //2
            configValues.Add("Username:", "Username");  //3
            configValues.Add("Password:", "Password");  //4
            configValues.Add("Pollrate:", "4");         //5




            Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            String[] config = new String[6];

            Console.ForegroundColor = ConsoleColor.White;
            if (File.Exists(textFile))
            {

                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);
                foreach (string line in lines)
                {

                    for (int i = 0; i < configValues.Count; i++)
                        if (line.Contains(configValues.ElementAt(i).Key))
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine($"found {(configValues.ElementAt(i).Key)}");
                            Console.ForegroundColor = ConsoleColor.Green;
                            int commentHash = line.IndexOf("#"); //stops reading after comment hash

                            int configValueLength = commentHash - configValues.ElementAt(i).Key.Length; //length of the value between the keywork length and position of the comment #

                            configValues[configValues.ElementAt(i).Key] = line.Substring(configValues.ElementAt(i).Key.Length, configValueLength);


                        }

                }
                Console.ForegroundColor = ConsoleColor.Green;


            }
            Console.ForegroundColor = ConsoleColor.White;
            for (int ja = 0; ja < configValues.Count; ja++) {
                Console.WriteLine($"{(configValues.ElementAt(ja).Key)}{(configValues.ElementAt(ja).Value)}");

        }
           
            ValidateValues();
        }
        public static void ValidateValues()
        {
            //check the configValues values are valid

            //for each value in dictionary configValuesType

            for (int i = 0; i < configValues.Count; i++)

                  if (configValuesType.ElementAt(i).Value) // TRUE = INT, FALSE = CHAR
            {

                try
                {
                    //todo to move this to a later function
                    int intconfig= Int32.Parse((configValues.ElementAt(i).Value));
                   // configValues[configValues.ElementAt(i).Key] = baudrate.ToString(); //converts back to string and updates dictionary
                    Console.WriteLine(intconfig);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                        Console.WriteLine($"{configValues.ElementAt(i).Key} has invalid value");
                        //exit
                }
            }

        }

    }
}
