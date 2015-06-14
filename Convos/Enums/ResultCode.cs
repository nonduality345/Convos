namespace Convos.Enums
{
    /// <summary>
    /// This enumeration is a list of codes that can be used by the application internally to describe the outcome of an operation
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// Everything behaved as expected. Nothing went wrong.
        /// </summary>
        OK = 0,
        /// <summary>
        /// The input into the operation was invalid
        /// </summary>
        INVALID_INPUT_DATA = 1,
        /// <summary>
        /// The specific entity that was requested could not be found
        /// </summary>
        ENTITY_NOT_FOUND = 2,
        /// <summary>
        /// There were no results from the operation
        /// </summary>
        NO_RESULTS = 3,
        /// <summary>
        /// The caller is unauthorized to perform the operation
        /// </summary>
        UNAUTHORIZED = 4,
        /// <summary>
        /// A new resource was created as a result of the operation
        /// </summary>
        CREATED = 5,
        /// <summary>
        /// A resource was deleted as a result of the operation
        /// </summary>
        DELETED = 6,
        /// <summary>
        /// The operation did not behave as expected and the reason for it is not known
        /// </summary>
        UNKNOWN = 7
    }
}
