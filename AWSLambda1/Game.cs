using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Response.Ssml;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda1
{
    [JsonObject]
    public class Game
    {
        #region Properties and Fields

        /// <summary>
        /// The name of this particular game.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The URL to the introduction audio file.
        /// </summary>
        [JsonProperty]
        public string IntroductionAudioURL { get; set; }

        /// <summary>
        /// A list of all the questions in this game.
        /// </summary>
        [JsonProperty]
        public List<Question> Questions { get; set; }

        /// <summary>
        /// How many questions do we need to get right to pass.
        /// </summary>
        [JsonProperty]
        public int PassScore { get; set; }

        /// <summary>
        /// The URL to the audio file for if you passed the game.
        /// </summary>
        [JsonProperty]
        public string PassAudioURL { get; set; }

        /// <summary>
        /// The URL to the audio file for if you failed the game.
        /// </summary>
        [JsonProperty]
        public string FailAudioURL { get; set; }

        #endregion

        public SkillResponse StartGame(ILambdaLogger logger)
        {
            // build the speech response 
            Speech speech = new Speech();
            speech.Elements.Add(new Sentence("Launching " + Name));
            speech.Elements.Add(new Audio(IntroductionAudioURL));
            speech.Elements.Add(new Audio(Questions[0].QuestionAudioURL));

            logger.LogLine(speech.ToXml());

            // create the response using the ResponseBuilder
            SkillResponse response = ResponseBuilder.Tell(speech);
            response.Response.ShouldEndSession = false;

            return response;
        }

        public Tuple<bool, SkillResponse> AnswerQuestion(int index, string answer, int currentScore)
        {
            Question question = Questions[index];
            bool correct = answer == question.CorrectAnswer;

            // build the speech response 
            Speech speech = new Speech();
            speech.Elements.Add(new Audio(correct ? question.CorrectAnswerAudioURL : question.IncorrectAnswerAudioURL));

            if (index == Questions.Count - 1)
            {
                if (!string.IsNullOrEmpty(PassAudioURL) && !string.IsNullOrEmpty(FailAudioURL))
                {
                    speech.Elements.Add(new Audio(currentScore >= PassScore ? PassAudioURL : FailAudioURL));
                }
                else
                {
                    speech.Elements.Add(new Sentence("Your final score was: " + (correct ? (currentScore + 1) : currentScore) + " out of " + Questions.Count));
                }
            }
            else
            {
                speech.Elements.Add(new Audio(Questions[index + 1].QuestionAudioURL));
            }
            
            // create the response using the ResponseBuilder
            SkillResponse response = ResponseBuilder.Tell(speech);
            response.Response.ShouldEndSession = false;

            return new Tuple<bool, SkillResponse>(correct, response);
        }
    }
}
