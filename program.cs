using System;
using System.IO.Ports;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
namespace ConsoleApp1
{
    class Program
    {
        static SerialPort _serialPort;

        public static void Main()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
            while (true)
            {
                getValues();
            Thread.Sleep(60000);
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
            writeToDatabase(primaryTemp, secondaryTemp, arduinoTemp, arduinoHumidty, primaryHumidity, sunlight);
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
                    Console.WriteLine("get-primay-temp failed, try #" + i);
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


        public static void writeToDatabase(float primaryTemp, float secondaryTemp, float arduinoTemp, int arduinoHumidity, int primaryHumidity, float sunlight)
            
        {
            SqlCommand loadNewDataCommand;
            SqlDataAdapter adapter = new SqlDataAdapter();
            String SQL = "Insert into sensor_readings (sunlight, primarytemp, primaryhumidity, arduinohumidity, secondarytemp, arduinotemp)";
            SQL += "VALUES ("+sunlight+","+primaryTemp+","+ primaryHumidity +","+ arduinoHumidity + ","+ secondaryTemp + ","+arduinoTemp+")";


            String connetionString;
            SqlConnection cnn;
            connetionString = @"Server=SERVERNAME;Database=DATABASENAME;User Id=ACCOUNTNAME;Password=PASSWORD;Integrated Security=False";
            Console.WriteLine("attempting connection to DB");
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            Console.WriteLine("Connection Open  !");
            //to do add anexception handler
            loadNewDataCommand = new SqlCommand(SQL, cnn);
            adapter.InsertCommand = new SqlCommand(SQL, cnn);
            adapter.InsertCommand.ExecuteNonQuery();


            cnn.Close();


        }

    }

}
