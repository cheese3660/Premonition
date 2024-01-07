namespace Premonition;

public enum PatchType
{
    Prefix,
    Postfix,
    Trampoline // This is basically just replace method with call out to patched method
}