using System;
using System.Collections.Concurrent;
using System.Net.Http;
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

        public async void Start(CancellationToken token)
        {
            redisConnection = await OpenConnection(Options.CacheAddress);

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
                string randomStr = GetRandomStringAsync(Options.RngAddress).Result;
                string hashed = HashStringAsync(Options.HasherAddress, randomStr).Result;

                if (hashed.StartsWith("0"))
                {
                    Log.Debug("Coin found {coinid", randomStr);
                    IDatabase db = redisConnection.GetDatabase();
                    db.HashSet("wallet", randomStr, hashed);
                }

                Interlocked.Increment(ref workDone);
            }
        }

        private void ReportUsage(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);

                IDatabase db = redisConnection.GetDatabase();
                db.StringIncrement("hashes", workDone);

                Interlocked.Exchange(ref workDone, 0);
            }
        }

        private async Task<ConnectionMultiplexer> OpenConnection (string cacheAddress)
        {
            ConfigurationOptions redisConfig = new ConfigurationOptions
            {
                EndPoints =
                {
                    { cacheAddress }
                },
                AbortOnConnectFail = false
            };

            return await ConnectionMultiplexer.ConnectAsync(redisConfig);  
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
