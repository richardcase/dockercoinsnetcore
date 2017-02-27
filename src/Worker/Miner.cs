using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using StackExchange.Redis;

namespace Worker
{
    public class Miner
    {
        private ConnectionMultiplexer redisConnection;
        private int workDone;

        public MinerOptions Options { get; private set; }

        public Miner(MinerOptions options)
        {
            Options = options;
        }

        public void Start(CancellationToken token)
        {
            redisConnection = OpenConnection(Options.CacheAddress, Options.CachePassword);

            var tasks = new ConcurrentBag<Task>();
            Task minerTask = Task.Factory.StartNew(() => Mine(token), token);
            tasks.Add(minerTask);
            Task reporterTask = Task.Factory.StartNew(() => ReportUsage(token), token);
            tasks.Add(reporterTask);

            Task.WaitAll(tasks.ToArray());

        }

        private void Mine(CancellationToken token)
        {
            var sw = new SpinWait();
           
            while (!token.IsCancellationRequested)
            {
                sw.SpinOnce();
                //NOTE: could add a sleep of 100ms if needed
                Thread.Sleep(100);
                try
                {
                    string randomStr = GetRandomStringAsync(Options.RngAddress).Result;
                    string hashed = HashStringAsync(Options.HasherAddress, randomStr).Result;

                    if (hashed.StartsWith("0"))
                    {
                        Log.Information("Coin found {coinid}", hashed);
                        IDatabase db = redisConnection.GetDatabase();
                        db.HashSet("wallet", randomStr, hashed);
                    }

                    Interlocked.Increment(ref workDone);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Issue occred with the miner");
                }
            }
        }

        private void ReportUsage(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);

                try
                {
                    IDatabase db = redisConnection.GetDatabase();
                    db.StringIncrement("hashes", workDone);
                    Log.Information("Mining iterations done {iterations}", workDone);

                    Interlocked.Exchange(ref workDone, 0);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Issue occred reporting usage");
                }
            }
        }

        private  ConnectionMultiplexer OpenConnection (string cacheAddress, string cachePassword)
        {
            ConfigurationOptions redisConfig = new ConfigurationOptions
            {
                EndPoints =
                {
                    { cacheAddress }
                },
                AbortOnConnectFail = false,
                Password = cachePassword,
                Ssl = true
            };

            return ConnectionMultiplexer.Connect(redisConfig);  
        }

        private async Task<string> GetRandomStringAsync(string rngAddr)
        {
            Log.Debug("Calling Rng Service");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0,0,10);
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(rngAddr);
                    request.Method = HttpMethod.Get;
                    request.Headers.Accept.Clear();

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return responseBody;
                    }

                    // No return
                    throw new Exception("Calling RNG returned a failure error code");
                }
            }
        }

        private async Task<string> HashStringAsync(string hasherAddr, string toHash)
        {
            Log.Debug("Calling Hasher Service");

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 10);
                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    request.RequestUri = new Uri(hasherAddr);
                    request.Method = HttpMethod.Post;
                    request.Headers.Accept.Clear();

                    using (StringContent content = new StringContent(toHash))
                    {
                        request.Content = content;
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            return responseBody;
                        }

                        // No return
                        throw new Exception("Calling Hashers returned a failure error code");
                    }


                }
            }
        }
    }
}
