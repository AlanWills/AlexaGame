using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda1
{
    [JsonObject]
    public class Question
    {
        #region Properties and Fields

        /// <summary>
        /// The url to the question audio file that will be played to the user.
        /// </summary>
        [JsonProperty]
        public string QuestionAudioURL { get; set; }

        /// <summary>
        /// The correct answer the user should say to answer this question correctly.
        /// </summary>
        [JsonProperty]
        public string CorrectAnswer { get; set; }

        /// <summary>
        /// The url to the audio file that we should play if the user said the correct answer.
        /// </summary>
        [JsonProperty]
        public string CorrectAnswerAudioURL { get; set; }

        /// <summary>
        /// The url to the audio file that we should play if the user said the incorrect answer.
        /// </summary>
        [JsonProperty]
        public string IncorrectAnswerAudioURL { get; set; }

        #endregion
    }
}
