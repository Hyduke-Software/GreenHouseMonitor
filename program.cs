using System;
using System.IO.Ports;
using System.Threading;
using System.Data.SqlClient;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {

        static SerialPort _serialPort;
        public static string[] credentials;

        public static void Main()

        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Hyduke Sensor Reader 2000");
            Configuration.getConfig(); //calls getConfig() to get config from sensorConfig.txt 
            Console.ForegroundColor = ConsoleColor.White;
            credentials = getCreds(); //loads credentials

            _serialPort = new SerialPort();
            listPorts();

            Console.WriteLine($"{Configuration.configValues.ElementAt(1).Value}PortName");
            Console.WriteLine($"{Configuration.configValues.ElementAt(0).Value}PortNumb");
                Console.ReadLine();
            _serialPort.PortName = Configuration.configValues.ElementAt(1).Value;
            _serialPort.BaudRate = Int32.Parse((Configuration.configValues.ElementAt(0).Value));
            try
            {
                _serialPort.Open();
                while (true)
                {
                   getValues();


                    Thread.Sleep(3600000); //todo  take hour value from config file
                }
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Error 1: COM port not found, check Device Manager");

            }

        }

        public static string[] getCreds()
        {
            ///ideally this would be a service account running the exe which would use AD authentication
               string[] credential;
            credential = new string[3] {"","",""}; //Servername, username, password
    //
            Console.WriteLine($"Server loaded:{Configuration.configValues.ElementAt(2).Value}");
            credential[0] = Configuration.configValues.ElementAt(2).Value;
            Console.WriteLine($"Loaded username:{Configuration.configValues.ElementAt(2).Value}");
            credential[1] = Configuration.configValues.ElementAt(3).Value;
            Console.WriteLine("Loaded password: *********");
            credential[2] = Configuration.configValues.ElementAt(4).Value;

            return credential;



        }

        public static void listPorts()
       
            {

                // Get a list of serial port names.
                string[] ports = SerialPort.GetPortNames();

                Console.WriteLine("The following serial ports were found:");

                // Display each port name to the console.
                foreach (string port in ports)
                {
                    Console.WriteLine(port);
                }
            


        }

        public static void getValues()
        {
            float primaryTemp   = getPrimaryTemp();
            float secondaryTemp = getsecondTemp();
           
            int primaryHumidity = getPrimaryHumidity();
            float arduinoTemp   = getArduinoTemp();
            int arduinoHumidty  = getArduinoHumidity();
            float sunlight      = getSunlight();
            Console.WriteLine(primaryTemp + secondaryTemp + arduinoTemp + arduinoHumidty + primaryHumidity + sunlight);
            writeToDatabase(Program.credentials,primaryTemp, secondaryTemp, arduinoTemp, arduinoHumidty, primaryHumidity, sunlight);
        }

        public static float getPrimaryTemp()
        {
            //reads from the bmp180 module
            float value;
            for (int i = 0; i < 10; i++){ //ten tries
            _serialPort.WriteLine("get-primary-temp");
            Thread.Sleep(500); //appears to work, may need amending if sync issues appears
            string a = _serialPort.ReadExisting();
            if (a == "")
            {
                    Console.WriteLine("get-primary-temp failed, try #" + i);
                }
            if (a != "")
                    {
                        value = float.Parse(a);
                        return value;
                    }
             }
            return 100; //returns 100 if failed

        }

        public static float getArduinoTemp()
        {
            //reads from the bmp180 module
            float value = 0;
            for (int i = 0; i < 10; i++)
            { //ten tries
                _serialPort.WriteLine("get-arduino-temp");
                Thread.Sleep(500);
                string a = _serialPort.ReadExisting();
                if (a == "")
                {
                    Console.WriteLine("get-arduino-temp failed, try #" + i);
                }
                if (a != "") //if serial read is not empty
                {
                    value = float.Parse(a);
                    return value;
                }
            }
            return 100; //returns 100 if failed TODO: 10/03/20 change to a more industry standard way
        }
        public static int getArduinoHumidity()
        {
            //reads from the arduino case dht11 module
            int value = 0;
            for (int i = 0; i < 10; i++)
            { //ten tries
                _serialPort.WriteLine("get-arduino-humidity");
                Thread.Sleep(500);
                string a = _serialPort.ReadExisting();
                if (a == "")
                {
                    Console.WriteLine("get-arduino-humidity failed, try #" + i);
                }
                if (a != "") //if serial read is not empty
                {
                    value = int.Parse(a);
                    return value;
                }
            }
            return 100; //returns 100 if failed TODO: 10/03/20 change to a more industry standard way
        }

        public static int getPrimaryHumidity()
        {
            //reads from the dht11 module
            int value;
            for (int i = 0; i < 10; i++)
            { //ten tries
                _serialPort.WriteLine("get-primary-humidity");
                Thread.Sleep(500);
                string a = _serialPort.ReadExisting();
                if (a == "")
                {
                    Console.WriteLine("get-arduino-humidity failed, try #" + i);
                }
                if (a != "")
                {
                    value = int.Parse(a);
                    return value;
                }
            }
            return 100; //returns 100 if failed
        }

        public static float getsecondTemp()
        { //reads from the dht11 module
            float value;
            for (int i = 0; i < 10; i++)
            { //ten tries
                _serialPort.WriteLine("get-secondary-temp");
                Thread.Sleep(500);
                string a = _serialPort.ReadExisting();
                if (a == "")
                {
                    Console.WriteLine("get-secondary-temp failed, try #" + i);
                }
                if (a != "")
                {

                    //break;
                    value = float.Parse(a);
                    return value;
                }
            }
            return 100; //returns 100 if failed
        }

        public static float getSunlight()
        { //reads from the dht11 module
            float value;
            for (int i = 0; i < 10; i++)
            { //ten tries
                _serialPort.WriteLine("get-sunlight");
                Thread.Sleep(500);
                string a = _serialPort.ReadExisting();
                if (a == "")
                {
                    Console.WriteLine("get-sunlight failed, try #" + i);
                }
                if (a != "")
                {

                    //break;
                    value = int.Parse(a);
                    return value;
                }
            }
            return 9999; //returns 9999 if failed
        }


        public static void writeToDatabase(string[] credentials, float primaryTemp, float secondaryTemp, float arduinoTemp, int arduinoHumidity, int primaryHumidity, float sunlight)
            
        {
            SqlCommand loadNewDataCommand;
            SqlDataAdapter adapter = new SqlDataAdapter();
            //TODO: refactor into a more secure stored procedure
            String SQL = "Insert into sensor_readings (sunlight, primarytemp, primaryhumidity, arduinohumidity, secondarytemp, arduinotemp)";
                                      SQL += "VALUES ("+sunlight+","+primaryTemp+","+ primaryHumidity +","+ arduinoHumidity + ","+ secondaryTemp + ","+arduinoTemp+")";


            String connetionString;
            SqlConnection cnn;
            
            connetionString = "Server="+ credentials[0] + ";User Id="+ credentials[1] + ";Password="+ credentials[2] + ";Integrated Security=False";
            Console.WriteLine("attempting connection to DB");
            cnn = new SqlConnection(connetionString);
           cnn.Open();
            Console.WriteLine("Connection Open!");
            //to do add anexception handler
            loadNewDataCommand = new SqlCommand(SQL, cnn);
            adapter.InsertCommand = new SqlCommand(SQL, cnn);
            adapter.InsertCommand.ExecuteNonQuery();


            cnn.Close();
            Console.WriteLine("Connection closed!");

        }

    }

}
