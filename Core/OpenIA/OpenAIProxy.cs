namespace Akiled.Core.OpenIA;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
public class OpenAIProxy : IDisposable
{
    private readonly HttpClient _openAIClient;
    private DateTime _lastRequest;
    private bool _waitedAPI;

    public OpenAIProxy(string apiKey)
    {
        this._openAIClient = new()
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        this._openAIClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Wibbo", "1.0"));
        this._openAIClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(+" + AkiledEnvironment.GetSettingsManager().TryGetValue("openia.useragent") + ")"));
        this._openAIClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        this._lastRequest = DateTime.Now;
    }

    public async Task<ChatCompletionMessage> SendChatMessage(List<ChatCompletionMessage> messagesSend)
    {
        if (this.IsReadyToSend() == false || this._waitedAPI)
        {
            return null;
        }

        this._waitedAPI = true;

        var url = AkiledEnvironment.GetSettingsManager().TryGetValue("openia.url");
        

        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var request = new
        {
            messages = messagesSend.ToArray(),
            model = "gpt-3.5-turbo-1106",
            max_tokens = 150,
            temperature = 0.6
        };

        try
        {
            var requestJson = JsonConvert.SerializeObject(request);
            var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
            var httpResponseMessage = await this._openAIClient.PostAsync(url, requestContent);

            this._lastRequest = DateTime.Now;
            this._waitedAPI = false;

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new
            {
                choices = new[] { new { message = new ChatCompletionMessage { Role = string.Empty, Content = string.Empty } } },
                error = new { message = string.Empty }
            });

            if (responseObject == null || !string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
            {
                return null;
            }

            var messagesGtp = responseObject?.choices[0]?.message;

            if (messagesGtp == null)
            {
                return null;
            }

            return messagesGtp;
        }
        catch
        {
            this._waitedAPI = false;
        }

        return null;
    }

    public bool IsReadyToSend()
    {
        var timespan = DateTime.Now - this._lastRequest;
        return timespan.TotalSeconds > 3 && !this._waitedAPI;
    }

    public void Dispose()
    {
        this._openAIClient.Dispose();

        GC.SuppressFinalize(this);
    }
}
