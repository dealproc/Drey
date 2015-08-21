//using System;

//namespace Drey.Nut
//{
//    /// <summary>
//    /// This marks the startup class for the application.
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
//    public class CrackingAttribute : Attribute
//    {
//        /// <summary>
//        /// Gets the startup class.
//        /// </summary>
//        public Type StartupClass { get; private set; }

//        /// <summary>
//        /// Gets a value indicating whether the nut requires configuration storage ability.
//        /// </summary>
//        public bool RequiresConfigurationStorage { get; private set; }

//        /// <summary>
//        /// Gets the name of the application domain.
//        /// </summary>
//        public string ApplicationDomainName { get; private set; }

//        /// <summary>
//        /// Gets the package identifier.
//        /// </summary>
//        public string PackageId { get; private set; }

//        /// <summary>
//        /// Human readable name of this package.
//        /// </summary>
//        public string DisplayAs { get; private set; }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="CrackingAttribute" /> class.
//        /// </summary>
//        /// <param name="startupClass">The startup class.</param>
//        /// <param name="requiresConfigurationStorage">
//        /// <para>if set to <c>true</c> we will call the Configure() method, passing an IApplicationSettings object.</para>
//        /// <para>if set to <c>false</c> we will call the Configure() method, passing no parameters to the method.</para>
//        /// </param>
//        /// <param name="applicationDomainName">The application domain's name</param>
//        public CrackingAttribute(Type startupClass, bool requiresConfigurationStorage, string applicationDomainName, string packageId, string displayAs)
//        {
//            StartupClass = startupClass;
//            RequiresConfigurationStorage = requiresConfigurationStorage;
//            ApplicationDomainName = applicationDomainName;
//            PackageId = packageId;
//            DisplayAs = displayAs;
//        }
//    }
//}