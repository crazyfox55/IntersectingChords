using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IntersectingChords
{
    struct Segment
    {
        public int Index;
        public HashSet<int> Contents;

        public int Low;
        public int High;

        public Segment(int index, int low, int high)
        { 
            Index = index;
            Contents = new HashSet<int>();

            Low = low;
            High = high;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string fileFilter = "*.txt";
            if (args.Length > 0)
            {
                fileFilter = args[0];
            }
            foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "\\ExampleFiles\\", fileFilter))
            {
                // Read each line of the file into a string array. Each element
                // of the array is one line of the file.
                string[] lines = File.ReadAllLines(file);
                
                if (int.TryParse(lines[0], out int n))
                {
                    Segment[] segments = new Segment[n];
                    int[] segmentMap = new int[n * 2];
                    
                    int index = 0;
                    while (index < n)
                    {
                        string[] segment = lines[index + 1].Split(' ');
                        int low = int.Parse(segment[0])-1;
                        int high = int.Parse(segment[1])-1;
                        segmentMap[low] = (index+1);
                        segmentMap[high] = 0 - (index+1);
                        segments[index] = new Segment(index, low, high);
                        index += 1;
                    }

                    int total = 0;
                    Task[] runningTasks = new Task[n];

                    index = 0;
                    while (index < n)
                    {
                        Segment seg = segments[index];
                        runningTasks[index] = Task.Factory.StartNew(() => Intersections(ref total, ref seg, ref segmentMap));
                        index += 1;
                    }

                    Console.WriteLine("Waiting for result");

                    Task.WaitAll(runningTasks);

                    Console.WriteLine($"Total Intersections: {total}");

                    void Intersections(ref int count, ref Segment seg, ref int[] segMap)
                    {
                        int i = seg.Low + 1;
                        while (i < seg.High)
                        {
                            if (segMap[i] > 0)
                            {
                                seg.Contents.Add(segMap[i]);
                            }
                            else
                            {
                                seg.Contents.Remove(0 - segMap[i]);
                            }
                            i += 1;
                        }
                        Interlocked.Add(ref count, seg.Contents.Count);
                    };

                }
                else
                {
                    Console.WriteLine($"Invalid formating of {file}");
                }
            }
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
