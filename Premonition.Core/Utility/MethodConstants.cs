using JetBrains.Annotations;

namespace Premonition.Core.Utility;

/// <summary>
/// A list of commonly used method names
/// </summary>
[PublicAPI]
public static class MethodConstants
{
    /// <summary>
    /// The IL name for constructors
    /// </summary>
    public const string Constructor = ".ctor";
    /// <summary>
    /// The IL name for destructors
    /// </summary>
    public const string Destructor = ".dtor";
    /// <summary>
    /// The IL name for <code>operator +(a,b)</code>
    /// </summary>
    public const string Add = "op_Addition";
    /// <summary>
    /// The IL name for <code>operator -(a,b)</code>
    /// </summary>
    public const string Subtract = "op_Subtraction";
    /// <summary>
    /// The IL name for <code>operator *(a,b)</code>
    /// </summary>
    public const string Multiply = "op_Multiply";
    /// <summary>
    /// The IL name for <code>operator /(a,b)</code>
    /// </summary>
    public const string Divide = "op_Divide";
    /// <summary>
    /// The IL name for <code>operator %(a,b)</code>
    /// </summary>
    public const string Remainder = "op_Modulus";
    /// <summary>
    /// The IL name for <code>operator ^(a,b)</code>
    /// </summary>
    public const string Xor = "op_ExclusiveOr";
    /// <summary>
    /// The IL name for <code>operator &amp;(a,b)</code>
    /// </summary>
    public const string BitwiseAnd = "op_BitwiseAnd";
    /// <summary>
    /// The IL name for <code>operator |(a,b)</code>
    /// </summary>
    public const string BitwiseOr = "op_BitwiseOr";
    /// <summary>
    /// The IL name for <code>operator &amp;&amp;(a,b)</code>
    /// </summary>
    public const string And = "op_LogicalAnd";
    /// <summary>
    /// The IL name for <code>operator ||(a,b)</code>
    /// </summary>
    public const string Or = "op_LogicalOr";
    /// <summary>
    /// The IL name for an assignment operator overload (not available in C#)
    /// </summary>
    public const string Assign = "op_Assign";
    /// <summary>
    /// The IL name for <code>operator &lt;&lt;(a,b)</code>
    /// </summary>
    public const string LeftShift = "op_LeftShift";
    /// <summary>
    /// The IL name for <code>operator &gt;&gt;(a,b)</code>
    /// </summary>
    public const string RightShift = "op_RightShift";
    /// <summary>
    /// The IL name for a signed left shift operator overload (not available in C#)
    /// </summary>
    public const string SignedLeftShift = "op_SignedLeftShift";
    /// <summary>
    /// The IL name for <code>operator &gt;&gt;&gt;(a,b)</code>
    /// </summary>
    public const string SignedRightShift = "op_SignedRightShift";
    /// <summary>
    /// The IL name for <code>operator ==(a,b)</code>
    /// </summary>
    public const string Equality = "op_Equals";
    /// <summary>
    /// The IL name for <code>operator !=(a,b)</code>
    /// </summary>
    public const string Inequality = "op_Inequality";
    /// <summary>
    /// The IL name for <code>operator &gt;(a,b)</code>
    /// </summary>
    public const string GreaterThan = "op_GreaterThan";
    /// <summary>
    /// The IL name for <code>operator &lt;(a,b)</code>
    /// </summary>
    public const string LessThan = "op_LessThan";
    /// <summary>
    /// The IL name for <code>operator &gt;=(a,b)</code>
    /// </summary>
    public const string GreaterThanOrEqual = "op_GreaterThanOrEqual";
    /// <summary>
    /// The IL name for <code>operator &lt;=(a,b)</code>
    /// </summary>
    public const string LessThanOrEqual = "op_LessThanOrEqual";
    /// <summary>
    /// The IL name for a multiplication assignment operator overload (not available in C#)
    /// </summary>
    public const string MultiplicationAssignment = "op_MultiplicationAssignment";
    /// <summary>
    /// The IL name for a subtraction assignment operator overload (not available in C#)
    /// </summary>
    public const string SubtractionAssignment = "op_SubtractionAssignment";
    /// <summary>
    /// The IL name for an exclusive or assignment operator overload (not available in C#)
    /// </summary>
    public const string ExclusiveOrAssignment = "op_ExclusiveOrAssignment";
    /// <summary>
    /// The IL name for a left shift assignment operator overload (not available in C#)
    /// </summary>
    public const string LeftShiftAssignment = "op_LeftShiftAssignment";
    /// <summary>
    /// The IL name for a modulus assignment operator overload (not available in C#)
    /// </summary>
    public const string ModulusAssignment = "op_ModulusAssignment";
    /// <summary>
    /// The IL name for an addition assignment operator overload (not available in C#)
    /// </summary>
    public const string AdditionAssignment = "op_AdditionAssginment";
    /// <summary>
    /// The IL name for a comma assignment operator overload (not available in C#)
    /// </summary>
    public const string Comma = "op_Comma";
    /// <summary>
    /// The IL name for a division assignment operator overload (not available in C#)
    /// </summary>
    public const string DivisionAssignment = "op_DivisionAssignment";
    /// <summary>
    /// The IL name for <code>operator --(a)</code>
    /// </summary>
    public const string Decrement = "op_Decrement";
    /// <summary>
    /// The IL name for <code>operator ++(a)</code>
    /// </summary>
    public const string Increment = "op_Increment";
    /// <summary>
    /// The IL name for <code>operator -(a)</code>
    /// </summary>
    public const string Negate = "op_UnaryNegation";
    /// <summary>
    /// The IL name for <code>operator +(a)</code>
    /// </summary>
    public const string Plus = "op_UnaryPlus";
    /// <summary>
    /// The IL name for <code>operator !(a)</code>
    /// </summary>
    public const string OnesComplement = "op_OnesComplement";
}