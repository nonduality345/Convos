using Convos.Entities;
using Convos.Enums;
using Convos.Utilities;
using Convos.Results;
using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Convos.Managers
{
    /// <summary>
    /// This class is the concrete implementation of IConvoManager that interfaces with the database
    /// </summary>
    public class ConvoManager : IConvoManager
    {
        #region " Members "
        /// <summary>
        /// The maximum number of characters that are allowed for the body of the message
        /// </summary>
        private const int BODY_MAX_SIZE = 64000;

        /// <summary>
        /// The maximum number of characters that are allowed for the subject of the message
        /// </summary>
        private const int SUBJECT_MAX_SIZE = 140;

        /// <summary>
        /// The key of the connection string in the configuration file
        /// </summary>
        private const string CONNECTION_STRING_KEY = "Convos";

        /// <summary>
        /// The connection string to the database
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// An instance of an ISQLPlatform to perform database CRUD
        /// </summary>
        private readonly ISQLPlatform _sqlPlatform;
        #endregion " Members "

        #region " Constructors and Public Methods "
        /// <summary>
        /// Constructor which takes in its dependency on an ISQLPlatform instance
        /// </summary>
        /// <param name="sqlPlatform">The concrete instance of ISQLPlatform to perform database operations.</param>
        public ConvoManager(ISQLPlatform sqlPlatform)
        {
            _sqlPlatform = sqlPlatform;
            _connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_KEY].ConnectionString;

            // If the connection string is not specified, throw an exception. The application can not run.
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException("A valid connection string must be supplied in the configuration file to use the ConvoManager class");
            }
        }

        /// <summary>
        /// This method deletes a Convo. Once a Convo is deleted, all of its messages are no longer accessible by anyone participating
        /// in the Convo.
        /// </summary>
        /// <param name="id">The Id of the Convo to delete.</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryDeleteConvo(long id, ref Result result, long userId)
        {
            var valid = IsValidId(id, "Convo", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            _sqlPlatform.ExecuteStoredProc(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Id", id},
                    {"UserId", userId}
                }, "usp_ConvoEntity_Delete", out resultCode);

            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method deletes a Message. Once a message is deleted, it is no longer accessible by the sender or the receiver of the
        /// Message.
        /// </summary>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="id">The Id of the Message to delete</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryDeleteMessage(long convoId, long id, ref Result result, long userId)
        {
            var valid = IsValidId(convoId, "Convo", ref result) && 
                        IsValidId(id, "Message", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            _sqlPlatform.ExecuteStoredProc(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Id", id},
                    {"ConvoId", convoId},
                    {"UserId", userId}
                }, "usp_MessageEntity_Delete", out resultCode);

            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method retrieves Convos from the database using paging.
        /// </summary>
        /// <param name="before">The time from which to retrieve results</param>
        /// <param name="count">The number of convos on the page to retrieve</param>
        /// <param name="index">The index of the page of convos. This is zero based.</param>
        /// <param name="maxCreated">The date of the latest created Convo before the "before" time</param>
        /// <param name="response">The returned convos from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="total">The total number of convos</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryGetAllConvos(DateTime before, int count, int index, out DateTime maxCreated, out ConvoResponse[] response, ref Result result, out int total, long userId)
        {
            response = null;
            total = 0;
            maxCreated = DateTime.MinValue;
            var valid = IsValidInt(count, "Count", ref result) &&
                        IsValidInt(index, "Index", ref result) &&
                        IsValidId(userId, "User", ref result);
            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Before", before},
                    {"Count", count},
                    {"Index", index},
                    {"UserId", userId}
                }, "usp_ConvoEntity_GetAll", out resultCode);
            result.AddMessage(message);
            result.ResultCode = resultCode;
            if (ds.Tables.Count > 0)
            {
                response = GetConvosFromDataTable(ds.Tables[0]);
                total = (int)ds.Tables[1].Rows[0]["Total"];
                maxCreated = (DateTime) ds.Tables[2].Rows[0]["MaxCreated"];
            }
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method retrieves Messages from the database
        /// </summary>
        /// <param name="before">The time from which to retrieve results</param>
        /// <param name="convoId">The Id of the Convo from which to get the messages</param>
        /// <param name="count">The number of messages on the page to retrieve</param>
        /// <param name="index">The index of the page of Messages. This is zero based.</param>
        /// <param name="maxCreated">The date of the last Message before the "before" time</param>
        /// <param name="responses">The returned messages from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="total">The total number of messages</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryGetAllMessages(DateTime before, long convoId, int count, int index, out DateTime maxCreated, out MessageResponse[] responses, ref Result result, out int total, long userId)
        {
            responses = null;
            total = 0;
            maxCreated = DateTime.MinValue;
            var valid = IsValidId(userId, "User", ref result) &&
                        IsValidInt(index, "Index", ref result) &&
                        IsValidId(convoId, "Convo", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Before", before},
                    {"Count", count},
                    {"Index", index},
                    {"ConvoId", convoId},
                    {"UserId", userId}
                }, "usp_MessageEntity_GetAll", out resultCode);
            result.AddMessage(message);
            result.ResultCode = resultCode;
            if (ds.Tables.Count > 0)
            {
                responses = GetMessagesFromDataTable(ds.Tables[0]);
                total = (int)ds.Tables[1].Rows[0]["Total"];
                maxCreated = (DateTime)ds.Tables[2].Rows[0]["MaxCreated"];
            }
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method retrieves a specific Convo based on its Id
        /// </summary>
        /// <param name="id">The Id of the Convo</param>
        /// <param name="response">The returned Convo from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryGetConvoById(long id, out ConvoResponse response, ref Result result, long userId)
        {
            response = null;
            var valid = IsValidId(id, "Convo", ref result) && 
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"ConvoId", id},
                    {"UserId", userId}
                }, "usp_ConvoEntity_GetById", out resultCode);
            result.AddMessage(message);
            result.ResultCode = resultCode;
            var entities = GetConvosFromDataTable(ds.Tables[0]);
            if (entities.Length > 0)
            {
                response = entities[0];
            }
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method retrieves a specific Message based on its Id
        /// </summary>
        /// <param name="convoId">The Convo Id that this message belongs to</param>
        /// <param name="id">The Id of the Message</param>
        /// <param name="response">The returned Message from the database</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryGetMessageById(long convoId, long id, out MessageResponse response, ref Result result, long userId)
        {
            response = null;
            var valid = IsValidId(convoId, "Convo", ref result) && 
                        IsValidId(id, "Message", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Rows.Add(id);

            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"ConvoId", convoId},
                    {"MessageIds", dt},
                    {"UserId", userId}
                }, "usp_MessageEntity_GetByIds", out resultCode);
            result.AddMessage(message);
            result.ResultCode = resultCode;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var entities = GetMessagesFromDataTable(ds.Tables[0], true);
                response = entities[0];
            }
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method inserts a new Convo into the database
        /// </summary>
        /// <param name="id">The returned Id of the newly created Convo</param>
        /// <param name="participant">The Id of the user who is also participating in this Convo</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="subject">The subject of this Convo</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryInsertConvo(out long id, long participant, ref Result result, string subject, long userId)
        {
            id = 0;
            var valid = IsValidId(participant, "Participant", ref result) &&
                        IsValidSubject(subject, ref result) && 
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Creator", userId},
                    {"Participant", participant},
                    {"Subject", subject}
                }, "usp_ConvoEntity_Insert", out resultCode);
            if (ds.Tables.Count > 0 && 
                ds.Tables[0].Rows.Count > 0)
            {
                id = (long)(decimal)ds.Tables[0].Rows[0]["Id"];
            }
            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.CREATED);
        }

        /// <summary>
        /// This method inserts a new Message into the database
        /// </summary>
        /// <param name="body">The body of the message</param>
        /// <param name="convoId">The Id of the Convo to which this Message belongs</param>
        /// <param name="id">The returned Id of the newly created Message</param>
        /// <param name="parent">If this Message is a reply to another Message, the id of that Message should be specified as
        /// the parent.</param>
        /// <param name="recipient">The user id of the recipient of this message</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryInsertMessage(string body, long convoId, out long id, long? parent, long recipient, ref Result result, long userId)
        {
            id = 0;
            var valid = IsValidBody(body, ref result) && 
                        IsValidId(convoId, "Convo", ref result) &&
                        (parent == null || IsValidId((long)parent, "Parent", ref result)) && 
                        IsValidId(recipient, "Recipient", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            var ds = _sqlPlatform.ExecuteStoredProcWithResults(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Body", body},
                    {"ConvoId", convoId},
                    {"Recipient", recipient},
                    {"Parent", parent},
                    {"Sender", userId}
                }, "usp_MessageEntity_Insert", out resultCode);
            if (ds.Tables.Count > 0 && 
                ds.Tables[0].Rows.Count > 0)
            {
                id = (long)(decimal)ds.Tables[0].Rows[0]["Id"];
            }
            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.CREATED);
        }

        /// <summary>
        /// This method partially updates a Convo for only the allowable fields. The only field that can be updated is the subject.
        /// </summary>
        /// <param name="id">The Id of the Convo to partially update</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="subject">The new value for the subject</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryPatchConvo(long id, ref Result result, string subject, long userId)
        {
            var valid = IsValidSubject(subject, ref result) &&
                        IsValidId(id, "Convo", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            _sqlPlatform.ExecuteStoredProc(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Id", id},
                    {"Subject", subject},
                    {"UserId", userId}
                }, "usp_ConvoEntity_Patch", out resultCode);
            
            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.OK);
        }

        /// <summary>
        /// This method partially updates a Message for only the allowable fields. The only fields that can be updated are the Body and IsRead.
        /// </summary>
        /// <param name="body">The new value for the body (if it is null, it is ignored)</param>
        /// <param name="convoId">The Id of the Convo to which this message belongs</param>
        /// <param name="id">The Id of the message to partially update</param>
        /// <param name="isRead">The new value for the IsRead field (if this is null, it is ignored)</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="userId">The user id of the user making the request</param>
        /// <returns>A boolean flag that indicates whether or not the function behaved as expected.</returns>
        public bool TryPatchMessage(string body, long convoId, long id, bool? isRead, ref Result result, long userId)
        {
            var valid = IsValidBody(body, ref result) &&
                        IsValidId(convoId, "Convo", ref result) &&
                        IsValidId(id, "Message", ref result) &&
                        IsValidId(userId, "User", ref result);

            if (!valid)
            {
                return false;
            }

            string message;
            ResultCode resultCode;
            _sqlPlatform.ExecuteStoredProc(_connectionString, out message,
                new Dictionary<string, object>
                {
                    {"Id", id},
                    {"Body", body},
                    {"ConvoId", convoId},
                    {"IsRead", isRead},
                    {"UserId", userId}
                }, "usp_MessageEntity_Patch", out resultCode);

            result.AddMessage(message);
            result.ResultCode = resultCode;
            return result.ResultCode.Equals(ResultCode.OK);
        }
        #endregion " Constructors and Public Methods "

        #region " Private Methods "
        /// <summary>
        /// This method returns an array of ConvoEntities that is constructed from the data that is in the table parameter.
        /// </summary>
        /// <param name="dt">The data table that is holding all of the Convo information</param>
        /// <returns>An array of fully constructed ConvoEntities</returns>
        private ConvoResponse[] GetConvosFromDataTable(DataTable dt)
        {
            var retVal = new List<ConvoResponse>();
            foreach (DataRow row in dt.Rows)
            {
                var convo = new ConvoResponse();
                convo.Creator = (long)row["Creator"];
                convo.DateCreated = (DateTime)row["DateCreated"];
                convo.DateOfLastMessage = row["DateOfLastMessage"] == DBNull.Value ? null : (DateTime?)row["DateOfLastMessage"];
                convo.DateUpdated = (DateTime)row["DateUpdated"];
                convo.Id = (long)row["Id"];
                convo.NumMessages = (int)row["NumMessages"];
                convo.Participant = (long)row["Participant"];
                convo.Subject = row["Subject"] == DBNull.Value ? null : (string)row["Subject"];
                retVal.Add(convo);
            }
            return retVal.ToArray();
        }

        /// <summary>
        /// This method returns an array of MessageEntities that is constructed from the data that is in the table parameter.
        /// </summary>
        /// <param name="dt">The data table that is holding all of the Message information</param>
        /// <param name="includeThread">Includes the reply thread in the object</param>
        /// <returns>An array of fully constructed MessageEntities</returns>
        private MessageResponse[] GetMessagesFromDataTable(DataTable dt, bool includeThread = false)
        {
            var retVal = new List<MessageResponse>();
            var lookup = new Dictionary<long, MessageResponse>();
            foreach (DataRow row in dt.Rows)
            {
                var message = new MessageResponse();
                message.Body = row["Body"] == DBNull.Value ? null : (string)row["Body"];
                message.ConvoId = (long)row["ConvoId"];
                message.DateCreated = (DateTime)row["DateCreated"];
                message.DateUpdated = (DateTime)row["DateUpdated"];
                message.Id = (long)row["Id"];
                message.IsRead = (bool)row["IsRead"];
                message.Parent = row["Parent"] == DBNull.Value ? null : (long?)row["Parent"];
                message.Recipient = (long)row["Recipient"];
                message.Sender = (long)row["Sender"];

                if ((int) row["Level"] == 0)
                {
                    retVal.Add(message);
                }
                lookup.Add(message.Id, message);
            }

            if (includeThread)
            {
                // Loop through the lookup table and create the hierarchy of replies
                lookup.ForEach(kv =>
                {
                    var parent = lookup[kv.Key].Parent;
                    if (parent != null && lookup.ContainsKey((long)parent))
                    {
                        if (kv.Value.Thread == null)
                        {
                            kv.Value.Thread = new List<MessageResponse>();
                        }
                        kv.Value.Thread.Add(lookup[(long)parent]);
                    }
                });
            }
            return retVal.ToArray();
        }

        /// <summary>
        /// This method validates the body of a message
        /// </summary>
        /// <param name="body">The body of the message to validate</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean indicating whether or not the body is valid</returns>
        private bool IsValidBody(string body, ref Result result)
        {
            if (!string.IsNullOrWhiteSpace(body) && body.Length > BODY_MAX_SIZE)
            {
                result.AddMessage(string.Format("The maximum number of characters allowed for the body is {0}", BODY_MAX_SIZE));
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method validates Ids to make sure they are >= 0
        /// </summary>
        /// <param name="id">The Id to validate</param>
        /// <param name="fieldName">The name of the field to append to any error message (providing the caller something identifiable 
        /// to fix).</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean indicating whether or not the Id is valid</returns>
        private bool IsValidId(long id, string fieldName, ref Result result)
        {
            if (id <= 0)
            {
                result.AddMessage(string.Format("Invalid Id value for {0}", fieldName));
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method validates integers to make sure they in the range of min and max. 
        /// This is mainly used to validate pageIndex and pageSize
        /// </summary>
        /// <param name="val">The integer to validate</param>
        /// <param name="fieldName">The name of the field to append to any error message (providing the caller something identifiable
        /// to fix)</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <param name="min">The minimum acceptable value for the integer</param>
        /// <param name="max">The maximum acceptable value for the integer</param>
        /// <returns>A boolean indicating whether or not the integer is valid</returns>
        private bool IsValidInt(int val, string fieldName, ref Result result, int min = 0, int max = int.MaxValue)
        {
            if (val < min || val > max)
            {
                result.AddMessage(string.Format("Invalid value for {0}", fieldName));
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }

        /// <summary>
        /// This method validates the subject of a Convo
        /// </summary>
        /// <param name="subject">The subject to validate</param>
        /// <param name="result">A Result object that describes the result of the operation</param>
        /// <returns>A boolean indicating whether or not the integer is valid</returns>
        private bool IsValidSubject(string subject, ref Result result)
        {
            if (!string.IsNullOrWhiteSpace(subject) && subject.Length > SUBJECT_MAX_SIZE)
            {
                result.AddMessage(string.Format("The maximum number of characters allowed for the subject is {0}", SUBJECT_MAX_SIZE));
                result.ResultCode = ResultCode.INVALID_INPUT_DATA;
                return false;
            }
            return true;
        }
        #endregion " Private Methods "
    }
}
