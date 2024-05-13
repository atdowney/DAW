using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW
{
    internal class Audio
    {
        WaveFileWriter writer;
        WaveIn wave;

        public Audio()
        {
            //Create the variable to hold the recording
            wave = new WaveIn();

            //This will mean default mic. If we want we can change this number while connected to an interface for multipule mics
            wave.DeviceNumber = 0;

            //Format to be 44.1 kHz and 16 bit, 1 channel 
            wave.WaveFormat = new WaveFormat(44100, 16, 1);

            //Event handeler for when data is available to record
            wave.DataAvailable += Wave_DataAvailable;

            //Event handler for when the recording has stopped.
            wave.RecordingStopped += Wave_RecordingStopped;

            //File location
            string pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileLocation = GetNextFileName(pathToDesktop + "/DAWTestRecordings/TestRecording.wav");

            //Create a writer to make the Wave file
            writer = new WaveFileWriter(fileLocation, wave.WaveFormat);

        }

        public void Record()
        {
            wave.StartRecording();
        }

        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            //Write the data into the file
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }


        private void Wave_RecordingStopped(object sender, StoppedEventArgs e)
        {
            writer.Dispose();
        }

        public void StopRecording()
        {
            wave.StopRecording();
        }

        private string GetNextFileName(string baseFileName)
        {
            int count = 1;
            string fileName = baseFileName;
            string directory = Path.GetDirectoryName(baseFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFileName);
            string extension = Path.GetExtension(baseFileName);

            while (File.Exists(fileName))
            {
                fileName = Path.Combine(directory, $"{fileNameWithoutExtension}_{count}{extension}");
                count++;
            }

            return fileName;
        }
    }
}
