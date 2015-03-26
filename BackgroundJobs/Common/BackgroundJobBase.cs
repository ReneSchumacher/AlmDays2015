//--------------------------------------------------------------------------------
// This file is part of a Microsoft sample.
//
// (c) 2013 Microsoft Corporation. All rights reserved. 
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//--------------------------------------------------------------------------------

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;

namespace Microsoft.PSfD.TeamFoundation.BackgroundJobs.Common
{
    /// <summary>
    /// Base class for defining Team Foundation Server background jobs that are run by the
    /// Team Foundation Background Job Agent. The class contains basic logging functionality.
    /// </summary>
    public abstract class BackgroundJobBase : ITeamFoundationJobExtension
    {
        #region Fields

        /// <summary>
        /// The number of errors that have been logged.
        /// </summary>
        private int errors = 0;
        /// <summary>
        /// The number of warnings that have been logged.
        /// </summary>
        private int warnings = 0;
        /// <summary>
        /// The log level for this job. 0 does not log anything, 1 logs errors, 2 logs warnings and errors.
        /// </summary>
        private int logLevel = 0;

        /// <summary>
        /// The maximum number of errors and warnings that is being logged to the job history.
        /// If the number is exceeded, the log is truncated and <see cref="errorsWarningsTruncated"/> is
        /// set to True.
        /// </summary>
        private static readonly int maxErrorsWarningsLogged = 1000;
        private int errorsWarningsLogged = 0;
        /// <summary>
        /// True, if the number of errors and warnings exceeds the limit specified in <see cref="maxErrorsWarningsLogged"/>,
        /// otherwise False.
        /// </summary>
        private bool errorsWarningsTruncated;
        /// <summary>
        /// A StringBuilder object containing all logged information. Use this to set the resultMessage parameter
        /// in your implementation of the <see cref="Run"/> method.
        /// </summary>
        private StringBuilder errorsWarningsBuilder = new StringBuilder();

        #endregion

        #region ITeamFoundationJobExtension

        /// <summary>
        /// This method is called, when your job is run by the Team Foundation Background Job Agent.
        /// </summary>
        /// <param name="requestContext">
        /// The request context of the current job run. Use this to get to the Team Foundation Services as necessary.
        /// </param>
        /// <param name="jobDefinition">
        /// The definition of the current job, which contains scheduling information as well as the job data.
        /// </param>
        /// <param name="queueTime">The time this job was scheduled to run.</param>
        /// <param name="resultMessage">
        /// Any result message that should be logged to the job history. Use the <see cref="o:LogError"/> and <see cref="o:LogWarning"/>
        /// methods to create standard error and warning messages or log exceptions. At the end of your Run method, assign the contents
        /// of the <see cref="errorsWarningsBuilder"/> to the resultMessage parameter.
        /// </param>
        /// <returns></returns>
        public TeamFoundationJobExecutionResult Run(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime, out string resultMessage)
        {
            TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
            resultMessage = string.Empty;
            try
            {
                InitializeLogging(requestContext);

                bool skipFurtherResultAnalysis = false;

                DoWork(requestContext, jobDefinition, queueTime, ref result, ref resultMessage, ref skipFurtherResultAnalysis);

                if (!skipFurtherResultAnalysis)
                {
                    AnalyzeResults(ref resultMessage, out result);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, out resultMessage, out result);
            }

            return result;
        }

        #endregion

        #region Protected Members

        /// <summary>
        /// The actual work of the job is being implemented here.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="jobDefinition"></param>
        /// <param name="queueTime"></param>
        /// <param name="result"></param>
        /// <param name="resultMessage"></param>
        /// <param name="skipFurtherResultAnalysis"></param>
        protected abstract void DoWork(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime, ref TeamFoundationJobExecutionResult result, ref string resultMessage, ref bool skipFurtherResultAnalysis);

        /// <summary>
        /// For logging, the base class needs to now a friendly name for this job. Via this abstract method it forces children to provide one.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetFriendlyJobName();

        /// <summary>
        /// For logging, the base class needs to now the log level registry key for this job. Via this abstract method it forces children to provide one.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetLogLevelRegistryKey();

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message that should be logged.</param>
        /// <param name="exception">An optional exception that should be logged with the message.</param>
        protected void LogError(String message, Exception exception)
        {
            errors++;

            if (logLevel > 0)
            {
                AppendWarningError(message, exception);
            }
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message that should be logged.</param>
        protected void LogError(String message)
        {
            LogError(message, null);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message that should be logged.</param>
        /// <param name="exception">An optional exception that should be logged with the message.</param>
        protected void LogWarning(String message, Exception exception)
        {
            warnings++;

            if (logLevel > 1)
            {
                AppendWarningError(message, exception);
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message that should be logged.</param>
        protected void LogWarning(String message)
        {
            LogWarning(message, null);
        }

        #endregion

        #region Members

        private void InitializeLogging(TeamFoundationRequestContext context)
        {
            var registry = context.GetService<TeamFoundationRegistryService>();
            this.logLevel = registry.GetValue<int>(context, this.GetLogLevelRegistryKey(), 0);
        }

        private void HandleException(Exception ex, out string resultMessage, out TeamFoundationJobExecutionResult result)
        {
            StringBuilder resultMessageBuilder = new StringBuilder();

            resultMessageBuilder.AppendLine("The " + this.GetFriendlyJobName() + " job failed with an exception:");
            resultMessageBuilder.AppendLine(ex.ToString());

            resultMessage = resultMessageBuilder.ToString();
            result = TeamFoundationJobExecutionResult.Failed;
        }

        private void AnalyzeResults(ref string resultMessage, out TeamFoundationJobExecutionResult result)
        {
            if (errors == 0 && warnings == 0)
            {
                result = TeamFoundationJobExecutionResult.Succeeded;
            }
            else
            {
                if (errors == 0)
                {
                    result = TeamFoundationJobExecutionResult.PartiallySucceeded;
                }
                else
                {
                    result = TeamFoundationJobExecutionResult.Failed;
                }

                resultMessage += CreateResultMessage();
            }
        }

        private string CreateResultMessage()
        {
            StringBuilder resultMessageBuilder = new StringBuilder();

            resultMessageBuilder.AppendLine("There were errors or warnings during " + this.GetFriendlyJobName() + ".");
            resultMessageBuilder.AppendLine();
            resultMessageBuilder.AppendLine(string.Format("{0} errors.", errors));
            resultMessageBuilder.AppendLine(string.Format("{0} warnings.", warnings));
            resultMessageBuilder.AppendLine();

            // If we're not logging every warning and error, tell them how to increase the logging level.
            if (logLevel < 2)
            {
                resultMessageBuilder.AppendLine("Set " + this.GetLogLevelRegistryKey() + " in the TF registry to 2 to see errors and warnings.");
                resultMessageBuilder.AppendLine();
            }

            if (errorsWarningsTruncated)
            {
                resultMessageBuilder.AppendLine(string.Format("The first {0} warnings/errors are below.", maxErrorsWarningsLogged));
                resultMessageBuilder.AppendLine();
            }

            resultMessageBuilder.Append(errorsWarningsBuilder);

            return resultMessageBuilder.ToString();
        }

        private void AppendWarningError(String message, Exception exception)
        {
            if (errorsWarningsLogged < maxErrorsWarningsLogged)
            {
                errorsWarningsLogged++;

                errorsWarningsBuilder.AppendLine("-------------------------------");
                errorsWarningsBuilder.AppendLine(message);

                if (exception != null)
                {
                    errorsWarningsBuilder.AppendLine();
                    errorsWarningsBuilder.Append("Exception: ");
                    errorsWarningsBuilder.AppendLine(exception.ToString());
                }
            }
            else
            {
                errorsWarningsTruncated = true;
            }
        }

        #endregion
    }
}
