﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RecipeBot.Domain.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WeekendBot.Domain.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be less or equal to {1} characters..
        /// </summary>
        internal static string Argument_0_must_be_less_or_equal_to_number_of_1_characters {
            get {
                return ResourceManager.GetString("Argument_0_must_be_less_or_equal_to_number_of_1_characters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Additional notes.
        /// </summary>
        internal static string RecipeDomainEntity_FieldName_AdditionalNotes {
            get {
                return ResourceManager.GetString("RecipeDomainEntity_FieldName_AdditionalNotes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cooking steps.
        /// </summary>
        internal static string RecipeDomainEntity_FieldName_CookingSteps {
            get {
                return ResourceManager.GetString("RecipeDomainEntity_FieldName_CookingSteps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ingredients.
        /// </summary>
        internal static string RecipeDomainEntity_FieldName_Ingredients {
            get {
                return ResourceManager.GetString("RecipeDomainEntity_FieldName_Ingredients", resourceCulture);
            }
        }
    }
}
