using System.Reflection;
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("brimborium")]
[assembly: AssemblyProduct("Hefezopf")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
#if Version1000
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#else
[assembly: AssemblyVersion(Hefezopf.HZVersion.AssemblyVersion)]
[assembly: AssemblyFileVersion(Hefezopf.HZVersion.AssemblyVersion)]
#endif
namespace Hefezopf
{
    internal class HZVersion
    {
        public const string AssemblyVersion = "1.0.0.0";
        public const string AssemblyFileVersion = "1.0.0.0";
    }
}