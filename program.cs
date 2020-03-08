//08/03/2020
//This C# program is an interace between the Arduino to the SQL database
//it sends commands via serial to the Arduino to request the necessary values
//it then runs an SQL command to enter the data as a new row.


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
        public static float temp = 0;
        public static float temptwo = 0;
        public static int humidity = 0;
        public static void Main()
        {
            string inputString = "";
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
            while (true)
            {
            Console.WriteLine(getTemp());
            Console.WriteLine(getsecondTemp());
            Console.WriteLine(getHumidity());
                Thread.Sleep(1);
           // Console.ReadLine();
            writeToDatabase();
            Thread.Sleep(60000);
         }

        }

        public static float getTemp()
        {
            //reads from the bmp180 module
            string readString = "";
            for (int i = 0; i < 10; i++){ } //ten tries
            _serialPort.WriteLine("temperature");
            Thread.Sleep(500);
            string a = _serialPort.ReadExisting();
            if (a == "")
            {
                Console.WriteLine("no data yet");
            }
            if (a != "")
            {

                //break;
                temp = float.Parse(a);
                return temp;
            }
            temp = 100;
            return temp; //returns 100 if failed
            //Console.WriteLine("TEMP is " + readString);
        }

        public static int getHumidity()
        {
            //reads from the bmp180 module
            string readString = "";
            for (int i = 0; i < 10; i++) { } //ten tries
            _serialPort.WriteLine("humidity");
            Thread.Sleep(500);
            string a = _serialPort.ReadExisting();
            if (a == "")
            {
                Console.WriteLine("no data yet");
            }
            if (a != "")
            {

                //break;
                humidity = int.Parse(a);
                return humidity;
            }
            return 100; //returns 100 if failed
            //Console.WriteLine("TEMP is " + readString);
        }

        public static float getsecondTemp()
        { //reads from the dht11 module
            string readString = "";
            for (int i = 0; i < 10; i++) { } //ten tries
            _serialPort.WriteLine("temperature2");
            Thread.Sleep(500);
            string a = _serialPort.ReadExisting();
            if (a == "")
            {
                Console.WriteLine("no data yet");
            }
            if (a != "")
            {

                //break;
                temptwo = float.Parse(a);
                return temptwo;
            }
            temptwo = 100;
            return temptwo; //returns 100 if failed
            //Console.WriteLine("TEMP is " + readString);
        }


        public static void writeToDatabase()
        {
            SqlCommand loadNewDataCommand;
            SqlDataAdapter adapter = new SqlDataAdapter();
            String SQL = "Insert into sensor_readings (sunlight, temperature, humidity, temperaturetwo)";
            SQL += "VALUES (69,"+temp+","+ humidity+","+temptwo+")";


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
