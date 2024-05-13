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
        // Writer for the WAV file
        WaveFileWriter writer;

        // WaveIn instance for audio recording
        WaveIn wave;

        // Text to be displayed on the button
        public bool AliveRecording { get; set; }

        public Audio()
        {
            AliveRecording = false;

            // Create the WaveIn instance for recording
            wave = new WaveIn();

            // Set the device number to the default microphone
            wave.DeviceNumber = 0;

            // Set the wave format to 44.1 kHz, 16 bit, 1 channel
            wave.WaveFormat = new WaveFormat(44100, 16, 1);

            // Event handler for when data is available to record
            wave.DataAvailable += Wave_DataAvailable;

            // Event handler for when the recording has stopped
            wave.RecordingStopped += Wave_RecordingStopped;

            // Get the path to the desktop
            string pathToDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // Generate a unique file name for the recording
            string fileLocation = GetNextFileName(pathToDesktop + "/DAWTestRecordings/TestRecording.wav");

            // Create a WaveFileWriter to write the WAV file
            writer = new WaveFileWriter(fileLocation, wave.WaveFormat);
        }

        // Start the recording
        public void Record()
        {
            AliveRecording = true;
            wave.StartRecording();
        }

        // Event handler for when audio data is available
        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            // Write the data to the WAV file
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        // Event handler for when the recording has stopped
        private void Wave_RecordingStopped(object sender, StoppedEventArgs e)
        {
            // Dispose of the writer to finalize the WAV file
            writer.Dispose();
        }

        // Stop the recording
        public void StopRecording()
        {
            wave.StopRecording();
            AliveRecording = false;
        }

        // Generate a unique file name by appending a number if the file already exists
        private string GetNextFileName(string baseFileName)
        {
            int count = 1;
            string fileName = baseFileName;
            string directory = Path.GetDirectoryName(baseFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFileName);
            string extension = Path.GetExtension(baseFileName);

            // Increment the number until a unique file name is found
            while (File.Exists(fileName))
            {
                fileName = Path.Combine(directory, $"{fileNameWithoutExtension}_{count}{extension}");
                count++;
            }

            return fileName;
        }
    }
}
