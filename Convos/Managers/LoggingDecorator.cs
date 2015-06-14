using Convos.Entities;
using Convos.Results;
using Convos.Utilities;
using System;

namespace Convos.Managers
{
    /// <summary>
    /// A very basic sample decorator that can be used with this architecture
    /// </summary>
    public class LoggingDecorator : IConvoManager
    {
        #region " Members "
        private readonly IConvoManager _convoManager;
        private readonly ILogger _logger;
        #endregion " Members "

        #region " Constructors and Public Methods "

        public LoggingDecorator(IConvoManager convoManager, ILogger logger)
        {
            _convoManager = convoManager;
            _logger = logger;
        }

        public bool TryDeleteConvo(long id, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryDeleteConvo(id, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(string.Format("Failed to Delete Convo: {0}", id));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryDeleteMessage(long convoId, long id, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryDeleteMessage(convoId, id, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(string.Format("Failed to Delete Message: {0}", id));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryGetAllConvos(DateTime before, int count, int index, out DateTime maxCreated, out ConvoResponse[] response, ref Result result, out int total, long userId)
        {
            bool success;
            maxCreated = default(DateTime);
            response = default(ConvoResponse[]);
            total = default(int);
            try
            {
                success = _convoManager.TryGetAllConvos(before, count, index, out maxCreated, out response, ref result, out total,
                userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to list Convos: before={0}, count={1}, index={2}, userId={3}",
                    before.ToString("s"),
                    count,
                    index,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryGetAllMessages(DateTime before, long convoId, int count, int index, out DateTime maxCreated, out MessageResponse[] responses, ref Result result, out int total, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryGetAllMessages(before, convoId, count, index, out maxCreated, out responses, ref result, out total,
                userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to list Messages: before={0}, convoId={4}, count={1}, index={2}, userId={3}",
                    before.ToString("s"),
                    count,
                    index,
                    userId,
                    convoId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryGetConvoById(long id, out ConvoResponse response, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryGetConvoById(id, out response, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to get Convo by Id: id={0}, userId={1}",
                    id,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryGetMessageById(long convoId, long id, out MessageResponse response, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryGetMessageById(convoId, id, out response, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to get Message by Id: convoId={2}, id={0}, userId={1}",
                    id,
                    userId, 
                    convoId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryInsertConvo(out long id, long participant, ref Result result, string subject, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryInsertConvo(out id, participant, ref result, subject, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to insert Convo: participant={0}, subject={1}, userId={2}",
                    participant,
                    subject,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryInsertMessage(string body, long convoId, out long id, long? parent, long recipient, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryInsertMessage(body, convoId, out id, parent, recipient, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to insert Message: body={0}, convoId={1}, parent={2}, recipient={3}, userId={4}",
                    body,
                    convoId,
                    parent, 
                    recipient,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryPatchConvo(long id, ref Result result, string subject, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryPatchConvo(id, ref result, subject, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to Patch Convo: id={0}, subject={1}, userId={2}",
                    id,
                    subject,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }

        public bool TryPatchMessage(string body, long convoId, long id, bool? isRead, ref Result result, long userId)
        {
            bool success;
            try
            {
                success = _convoManager.TryPatchMessage(body, convoId, id, isRead, ref result, userId);
            }
            catch (Exception ex)
            {
                _logger.LogMessage(
                    string.Format("Failed to Patch Message: body={0}, convoId={1}, id={2}, isRead={3}, userId={4}",
                    body,
                    convoId,
                    id,
                    isRead,
                    userId));
                _logger.LogMessage(ex.Message);
                _logger.LogMessage(ex.StackTrace);
                throw;
            }
            return success;
        }
        #endregion " Constructors and Public Methods "
    }
}