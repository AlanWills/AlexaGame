using Alexa.NET.Response;
using Alexa.NET.Response.Directive;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda1
{
    class MyAudioPlayerPlayDirective : IDirective
    {
        public MyAudioPlayerPlayDirective()
        {
        }

        [JsonProperty("type")]
        [JsonRequired]
        public string Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("playBehavior")]
        [JsonRequired]
        public PlayBehavior PlayBehavior { get; set; }

        [JsonProperty("audioItem")]
        [JsonRequired]
        public AudioItem AudioItem { get; set; }
    }
}
