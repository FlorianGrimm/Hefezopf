namespace Hefezopf.Contracts.DI
{
    using Brimborium.Funcstructors;

    /// <summary>
    /// Get an instance of funcstructor.
    /// </summary>
    public interface IDIService
    {
        /// <summary>
        /// Get an old or new instance of funcstructor.
        /// </summary>
        /// <returns>a funcstructor.</returns>
        IFuncstructor GetGlobalFuncstructor();
    }
}
