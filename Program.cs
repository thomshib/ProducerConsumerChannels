using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProducerConsumerChannels
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var max = 1000000;
            var count = 0;
            
            var channel = Channel.CreateBounded<int>(1000);

            //consumer
            var reading = Task.Run( async () =>
            {
                do
                {
                    while (channel.Reader.TryRead(out var i))
                    {
                        count += i;
                    }
                } while ( await channel.Reader.WaitToReadAsync());
               await channel.Reader.Completion;
               

            });

            //producer
            var writing = Task.Run(async () =>
            {
                for (int i = 0; i < max; i++)
                {
                    while (!channel.Writer.TryWrite(i))
                    {
                        await channel.Writer.WaitToWriteAsync();
                    }
                }
                channel.Writer.Complete();

            });








            await writing;
            await reading;

            Console.WriteLine(count);

            Console.ReadLine();

        }
    }
    
}
