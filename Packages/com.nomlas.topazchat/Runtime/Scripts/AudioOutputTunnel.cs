
using System;
using UdonSharp;
using UnityEngine;

namespace nomlas.TopazChat {
    public class AudioOutputTunnel : UdonSharpBehaviour
    {
        public AudioSource input;
        public AudioSource stereoOutput;
        public AudioSource leftOutput;
        public AudioSource rightOutput;
        public int bufferLength = 1024 * 4;

        private AudioClip stereoOutputClip;
        private AudioClip leftOutputClip;
        private AudioClip rightOutputClip;
        
        private float[][] readBuffer;
        private float[] stereoWriteBuffer;
        private float[] monoWriteBuffer;
        private long previousDspTimeSample = -1;
        private int stereoOutputClipWriteHead;
        private int leftOutputClipWriteHead;
        private int rightOutputClipWriteHead;

        private const int StereoChannelCount = 2;
        private int outputClipFrames;

        private void Start()
        {
            outputClipFrames = bufferLength * 2;
            stereoOutputClip = AudioClip.Create("Stereo Output Clip", outputClipFrames, StereoChannelCount, AudioSettings.outputSampleRate, false);
            if (stereoOutput != null)
            {
                stereoOutput.loop = true;
                stereoOutput.clip = stereoOutputClip;
            }

            leftOutputClip = AudioClip.Create("Left Output Clip", outputClipFrames, 1, AudioSettings.outputSampleRate, false);
            if (leftOutput != null)
            {
                leftOutput.loop = true;
                leftOutput.clip = leftOutputClip;
            }
            
            rightOutputClip = AudioClip.Create("Right Output Clip", outputClipFrames, 1, AudioSettings.outputSampleRate, false);
            if (rightOutput != null)
            {
                rightOutput.loop = true;
                rightOutput.clip = rightOutputClip;
            }

            readBuffer = new[] { new float[bufferLength], new float[bufferLength] };
            stereoWriteBuffer = new float[bufferLength * StereoChannelCount];
            monoWriteBuffer = new float[bufferLength];
        }
    
        private void Update()
        {
            var currentDspTimeSample = (long)Math.Floor(AudioSettings.dspTime * AudioSettings.outputSampleRate);
            if (previousDspTimeSample < 0)
            {
                previousDspTimeSample = currentDspTimeSample;
                return;
            }
    
            var freshDataFrames = (int)(currentDspTimeSample - previousDspTimeSample);
            if (freshDataFrames <= 0) return;
            if (freshDataFrames > bufferLength)
            {
                // Main thread stopped too much. Clear buffer and restart.
                previousDspTimeSample = currentDspTimeSample;
                stereoOutputClipWriteHead = 0;
                if (stereoOutput) stereoOutput.Stop();
                
                leftOutputClipWriteHead = 0;
                if (leftOutput) leftOutput.Stop();
                
                rightOutputClipWriteHead = 0;
                if (rightOutput) rightOutput.Stop();
                return;
            }
            
            var readBeginIndex = bufferLength - freshDataFrames;
    
            previousDspTimeSample = currentDspTimeSample;
            
            input.GetOutputData(readBuffer[0], 0);
            input.GetOutputData(readBuffer[1], 1);
            
            // mono left
            if (leftOutput)
            {
                if (!leftOutput.gameObject.activeInHierarchy)
                {
                    leftOutputClipWriteHead = 0;
                }
                else
                {
                    var timeSamples = leftOutput.timeSamples;
                    if (leftOutput.isPlaying &&
                        (
                            (leftOutputClipWriteHead <= timeSamples && timeSamples < leftOutputClipWriteHead + bufferLength)
                            || (timeSamples < leftOutputClipWriteHead && timeSamples + outputClipFrames < leftOutputClipWriteHead + bufferLength)
                         )
                        )
                    {
                        // ring buffer exhausted
                        leftOutputClipWriteHead = 0;
                        rightOutputClipWriteHead = 0;
                        rightOutput.Stop();
                        leftOutput.Stop();
                        return;
                    }
                    
                    Array.Copy(readBuffer[0], readBeginIndex, monoWriteBuffer, 0, freshDataFrames);
                    leftOutputClip.SetData(monoWriteBuffer, leftOutputClipWriteHead);
                
                    leftOutputClipWriteHead += freshDataFrames;
                
                    if (!leftOutput.isPlaying && leftOutputClipWriteHead >= bufferLength)
                    {
                        leftOutput.Play();
                    }

                    if (leftOutputClipWriteHead >= outputClipFrames)
                    {
                        leftOutputClipWriteHead -= outputClipFrames;
                    }
                }
            }

            // mono right
            if (rightOutput)
            {
                if (!rightOutput.gameObject.activeInHierarchy)
                {
                    rightOutputClipWriteHead = 0;
                }
                else
                {
                    var timeSamples = rightOutput.timeSamples;
                    if (rightOutput.isPlaying &&
                        (
                            (rightOutputClipWriteHead <= timeSamples && timeSamples < rightOutputClipWriteHead + bufferLength)
                            || (timeSamples < rightOutputClipWriteHead && timeSamples + outputClipFrames < rightOutputClipWriteHead + bufferLength)
                        )
                       )
                    {
                        // ring buffer exhausted
                        leftOutputClipWriteHead = 0;
                        rightOutputClipWriteHead = 0;
                        rightOutput.Stop();
                        leftOutput.Stop();
                        return;
                    }

                    Array.Copy(readBuffer[1], readBeginIndex, monoWriteBuffer, 0, freshDataFrames);
                    rightOutputClip.SetData(monoWriteBuffer, rightOutputClipWriteHead);

                    rightOutputClipWriteHead += freshDataFrames;

                    if (!rightOutput.isPlaying && rightOutputClipWriteHead >= bufferLength)
                    {
                        rightOutput.Play();
                    }

                    if (rightOutputClipWriteHead >= outputClipFrames)
                    {
                        rightOutputClipWriteHead -= outputClipFrames;
                    }
                }
            }
            
            // stereo interleave
            if (stereoOutput)
            {
                if (!stereoOutput.gameObject.activeInHierarchy)
                {
                    stereoOutputClipWriteHead = 0;
                }
                else
                {
                    var timeSamples = stereoOutput.timeSamples;
                    if (stereoOutput.isPlaying &&
                        (
                            (stereoOutputClipWriteHead <= timeSamples && timeSamples < stereoOutputClipWriteHead + bufferLength)
                            || (timeSamples < stereoOutputClipWriteHead && timeSamples + outputClipFrames < stereoOutputClipWriteHead + bufferLength)
                        )
                       )
                    {
                        // ring buffer exhausted
                        stereoOutputClipWriteHead = 0;
                        stereoOutput.Stop();
                        return;
                    }

                    for (var frame = 0; frame < freshDataFrames; frame++)
                    {
                        stereoWriteBuffer[frame * StereoChannelCount] = readBuffer[0][readBeginIndex + frame];
                        stereoWriteBuffer[frame * StereoChannelCount + 1] = readBuffer[1][readBeginIndex + frame];
                    }

                    stereoOutputClip.SetData(stereoWriteBuffer, stereoOutputClipWriteHead);
                    stereoOutputClipWriteHead += freshDataFrames;

                    if (!stereoOutput.isPlaying && stereoOutputClipWriteHead >= bufferLength)
                    {
                        stereoOutput.Play();
                    }

                    if (stereoOutputClipWriteHead >= outputClipFrames)
                    {
                        stereoOutputClipWriteHead -= outputClipFrames;
                    }
                }
            }
        }
    }
}
