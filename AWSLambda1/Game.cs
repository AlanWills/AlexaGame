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

        public SkillResponse AnswerQuestion(int index, string answer, ILambdaLogger logger)
        {
            Question question = Questions[index];

            // build the speech response 
            Speech speech = new Speech();
            speech.Elements.Add(new Audio(answer == question.CorrectAnswer ? question.CorrectAnswerAudioURL : question.IncorrectAnswerAudioURL));

            logger.LogLine(speech.ToXml());

            // create the response using the ResponseBuilder
            SkillResponse response = ResponseBuilder.Tell(speech);
            response.Response.ShouldEndSession = false;

            return response;
        }
    }
}
