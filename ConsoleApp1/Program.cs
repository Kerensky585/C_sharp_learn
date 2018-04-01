using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TeleprompterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTeleprompter().Wait();
        }
        
        static IEnumerable<string> ReadFrom(string file)
        {

            string line;

            using (var reader = File.OpenText(file))
            {
                while((line = reader.ReadLine()) != null )
                {
                    var words = line.Split(' ');
                    var lineLength = 0;
                    
                    foreach (var word in words)
                    {
                        yield return word + " ";
                        
                        lineLength += word.Length + 1;      
                        
                        if (lineLength > 70)
                        {
                            yield return Environment.NewLine;
                            lineLength = 0;
                        } 
                    }                        
                }          
            }       
        }

        private static async Task ShowTeleprompter(TelePrompterConfig telePrompterConfig)
        {
            var words = ReadFrom("sampleQuotes.txt");

            foreach (var line in words)
            {
                Console.Write(line);

                if (!string.IsNullOrWhiteSpace(line))
                {
                    await Task.Delay(telePrompterConfig.DelayInMilliSeconds);
                }
            }

            telePrompterConfig.SetDone();
        }

        private static async Task GetInput(TelePrompterConfig telePrompterConfig)
        {
            Action work = () =>
            {
                do
                {
                    var key = Console.ReadKey(true);

                    if (key.KeyChar == '>')
                    {
                        telePrompterConfig.UpdateDelay(-10);
                    }
                    else if (key.KeyChar == '<')
                    {
                        telePrompterConfig.UpdateDelay(10);
                    }
                } while (!telePrompterConfig.Done);
            };

            await Task.Run(work);
        }

        private static async Task RunTeleprompter()
        {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);

            var speedTask = GetInput(config);

            await Task.WhenAny(displayTask, speedTask);
        }
    }
}