﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Web;

using System.Net.Sockets;
using System.Net;
using System.IO;

namespace instrumentBE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filenameSerialConfig = "serial.conf";
            string serialPortName = "";

            //Introduksjon
            Console.WriteLine("instrumentBE has stared....");
            Console.WriteLine("please enter TCP port number");
            string serverPort = Console.ReadLine();

            try
            {
                int portNumber = Convert.ToInt32(serverPort);
            }
            catch (FormatException)
            {
                Console.WriteLine("Portnumber is not a number! Exiting....");
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                return;
            }

            //serial configuration. Load form file
            StreamReader serialConfReader = new StreamReader(filenameSerialConfig);
            serialPortName = serialConfReader.ReadLine();
            Console.WriteLine("Serial port Configured; " + serialPortName);
            serialConfReader.Close();

            /*string commandResponse = SerialCommand("COM3", "readconf");
            Console.WriteLine("Arduino response:  " + commandResponse);
            Console.ReadKey();
            */
            if(serialPortName == null || serialPortName.Length == 0)
            {
                serialPortName = "COM3";
            }
            int baudRate = 9600;
            SerialPort serialPort = new SerialPort(serialPortName, baudRate);


            //serialPort.Open();
            //Console.WriteLine("Connected to Ardurino");

            string serverIP = "127.0.0.1";
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(serverIP), 5000);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //string serialResponse = serialPort.ReadLine();
            //Console.WriteLine("Arduino response:  " + serialResponse);

            //TCP socet server
            //bind to endpoint and start servertry

            try
            {
                server.Bind(endpoint);
                server.Listen(10);
            }
            catch (SocketException ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine("Exiting-...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Server started. Waiting for connection...");


            //server.Bind(endpoint);

            //Output info
            //Console.WriteLine("Server started. Waiting for connection...");
            //serialPort.Open();
            //if(log)WriteToLogFile(("Server started. Waiting for clients

            while (true)
            {
                try
                {
                    Socket client = server.Accept();

                    //Socket client = server.Accept();
                    Console.WriteLine("Client connected...");
                    //data recived
                    byte[] buffer = new byte[1024];
                    int bytesReceived = client.Receive(buffer);

                    string commandReceived = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine("Received command: " + commandReceived);
                    if (commandReceived.Substring(0, 7) == "comport:")
                    {
                        serialPortName = commandReceived.Substring(8, commandReceived.Length - 8);
                        Console.WriteLine("Serial port Configured; " + serialPortName);
                        StreamWriter serialConfWrite = new StreamWriter(filenameSerialConfig);
                        serialConfWrite.WriteLine(serialPortName);
                        serialConfWrite.Close();

                        client.Send(Encoding.ASCII.GetBytes("Serial port configurated: " + serialPortName));
                        client.Close();
                    }
                    else
                    {
                        string commandResponse = SerialCommand(serialPort, commandReceived);
                        Console.WriteLine(commandResponse);

                        //send to client
                        client.Send(Encoding.ASCII.GetBytes("Command received was: " + commandResponse));
                        client.Close();
                        Console.WriteLine("Clinet disconnected...");
                    }


                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            /*Console.WriteLine("Waiting for response");

            serialPort.Open();
            Console.WriteLine("Connected to Ardurino");
            serialPort.WriteLine("readscaled");
            
            //string serialResponse = serialPort.ReadLine();
            //Console.WriteLine("Arduino response:  " + serialResponse);



                        string serialResponse = serialPort.ReadLine();

                        Console.WriteLine("Arduino response:  " + serialResponse);
                        Console.ReadKey();

                        serialPort.Close();*/

        }

        static string SerialCommand(SerialPort serialPort, string command)
        {

            string serialResponse = "";

            try
            {
                serialPort.Open();
                serialPort.WriteLine(command);
                serialResponse = serialPort.ReadLine();
                serialPort.Close();
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("SerialPort failed....");
                serialResponse = "SerialPort failed";
            }
            return serialResponse;
        }
    }

}
//}
/*static string SerialCommand(string portName, string command) 
{

    int baudRate = 9600;
    string serialResponse =""
    SerialPort serialPort = new SerialPort(portName, baudRate);
    try 
        {
            serialPort.Open();
        serialPort.WriteLine(command);
        serialResponse = serialPort.ReadLine();
        serialPort.Close();
        }
        catch (System.IO.IOExcrption)
        {
            Console.Writeline("SerialPort failed....");
            serialResponse = "failed"
           
        }
        return serialResponse;
   
}*/

