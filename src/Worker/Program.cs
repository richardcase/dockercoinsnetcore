using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information("Starting DockerCoins worker");

            Log.Information("Parsing environment variables");
            MinerOptions options = ParseEnvVars();

            if (string.IsNullOrEmpty(options.CacheAddress))
            {
                Log.Error<string>(null, "Environment variable {EnvVarName} not set", "DOCKERCOINS_CACHE_ADDR");
                return;
            }
            if (string.IsNullOrEmpty(options.CachePassword))
            {
                Log.Error<string>(null, "Environment variable {EnvVarName} not set", "DOCKERCOINS_CACHE_PWD");
                return;
            }
            if (string.IsNullOrEmpty(options.HasherAddress))
            {
                Log.Error<string>(null, "Environment variable {EnvVarName} not set", "DOCKERCOINS_HASHER_ADDR");
                return;
            }
            if (string.IsNullOrEmpty(options.RngAddress))
            {
                Log.Error<string>(null, "Environment variable {EnvVarName} not set", "DOCKERCOINS_RNG_ADD");
                return;
            }

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            
            var miner = new Miner(options);
            Task t = Task.Factory.StartNew(() => miner.Start(token), token);
            Log.Information<int>(null,"Miner task started {taskid}",t.Id);
            t.Wait(token);


            Log.Information<int>(null, "Miner completed", t.Id);
            Log.CloseAndFlush();
        }

        private static MinerOptions ParseEnvVars()
        {
            MinerOptions options = new MinerOptions
            {
                CacheAddress = Environment.GetEnvironmentVariable("DOCKERCOINS_CACHE_ADDR"),
                CachePassword = Environment.GetEnvironmentVariable("DOCKERCOINS_CACHE_PWD"),
                HasherAddress = Environment.GetEnvironmentVariable("DOCKERCOINS_HASHER_ADDR"),
                RngAddress = Environment.GetEnvironmentVariable("DOCKERCOINS_RNG_ADDR")
            };


            return options;
        }

    }
}
