using UnityEngine;
using System.IO;

public static class WavUtility
{
    public static byte[] FromAudioClip(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wav = ConvertToWav(samples, clip.frequency, clip.channels);
        return wav;
    }

    private static byte[] ConvertToWav(float[] samples, int sampleRate, int channels)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int length = samples.Length * 2;

        // Write WAV header
        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(36 + length);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1); // PCM
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2);
        writer.Write((short)(channels * 2));
        writer.Write((short)16);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(length);

        // Convert samples to 16-bit
        foreach (var s in samples)
        {
            short val = (short)(Mathf.Clamp(s, -1f, 1f) * short.MaxValue);
            writer.Write(val);
        }

        writer.Flush();
        return stream.ToArray();
    }
}
