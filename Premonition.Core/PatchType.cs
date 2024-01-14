namespace Premonition.Core;

/// <summary>
/// What type of patch to do
/// </summary>
public enum PatchType
{
    /// <summary>
    /// A prefix patch, runs code before the main method
    /// </summary>
    Prefix,
    /// <summary>
    /// A postfix patch, runs code when the main method returns
    /// </summary>
    Postfix,
    /// <summary>
    /// A trampoline patch, replaces the code with a call to another method
    /// </summary>
    Trampoline
}