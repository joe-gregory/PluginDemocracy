﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PluginDemocracy.API.Translations {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class TranslationResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TranslationResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PluginDemocracy.API.Translations.TranslationResources", typeof(TranslationResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please check your inbox to confirm your email.
        /// </summary>
        public static string ConfirmEmailCheckInbox {
            get {
                return ResourceManager.GetString("ConfirmEmailCheckInbox", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirm Email.
        /// </summary>
        public static string ConfirmEmailLink {
            get {
                return ResourceManager.GetString("ConfirmEmailLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you for signing up to Plugin Democracy. A new society awaits your input..
        /// </summary>
        public static string ConfirmEmailP1 {
            get {
                return ResourceManager.GetString("ConfirmEmailP1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please confirm your email by clicking on the following link:.
        /// </summary>
        public static string ConfirmEmailP2 {
            get {
                return ResourceManager.GetString("ConfirmEmailP2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please confirm your email.
        /// </summary>
        public static string ConfirmEmailTitle {
            get {
                return ResourceManager.GetString("ConfirmEmailTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New user created.
        /// </summary>
        public static string NewUserCreated {
            get {
                return ResourceManager.GetString("NewUserCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to create new user.
        /// </summary>
        public static string UnableToCreateNewUser {
            get {
                return ResourceManager.GetString("UnableToCreateNewUser", resourceCulture);
            }
        }
    }
}
