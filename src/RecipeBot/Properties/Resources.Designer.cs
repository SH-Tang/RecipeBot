﻿// Copyright (C) 2022 Dennis Tang. All rights reserved.
//
// This file is part of RecipeBot.
//
// RecipeBot is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RecipeBot.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("RecipeBot.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Recipe could not be successfully formatted: {0}..
        /// </summary>
        internal static string Embed_could_not_be_created_reason_0_ {
            get {
                return ResourceManager.GetString("Embed_could_not_be_created_reason_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recipe titled &apos;{0}&apos; with id &apos;{1}&apos; and author &apos;{2}&apos; was successfully deleted..
        /// </summary>
        internal static string RecipeController_DeleteRecipeAsync_RecipeTitle_0_with_RecipeId_1_and_AuthorName_2_was_succesfully_deleted {
            get {
                return ResourceManager.GetString("RecipeController_DeleteRecipeAsync_RecipeTitle_0_with_RecipeId_1_and_AuthorName_2" +
                        "_was_succesfully_deleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No saved recipes are found..
        /// </summary>
        internal static string RecipeEntriesController_GetAllRecipesAsync_No_saved_recipes_are_found {
            get {
                return ResourceManager.GetString("RecipeEntriesController_GetAllRecipesAsync_No_saved_recipes_are_found", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No saved recipes are found with the given category..
        /// </summary>
        internal static string RecipeEntriesController_GetAllRecipesAsync_No_saved_recipes_are_found_with_category {
            get {
                return ResourceManager.GetString("RecipeEntriesController_GetAllRecipesAsync_No_saved_recipes_are_found_with_catego" +
                        "ry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No saved recipes are found with the tag &apos;{0}&apos;..
        /// </summary>
        internal static string RecipeEntriesController_GetAllRecipesByTagAsync_No_saved_recipes_are_found_with_Tag_0_ {
            get {
                return ResourceManager.GetString("RecipeEntriesController_GetAllRecipesByTagAsync_No_saved_recipes_are_found_with_T" +
                        "ag_0_", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Additional notes.
        /// </summary>
        internal static string RecipeFieldName_Additional_Notes_DisplayName {
            get {
                return ResourceManager.GetString("RecipeFieldName_Additional_Notes_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cooking steps.
        /// </summary>
        internal static string RecipeFieldName_Cooking_Steps_DisplayName {
            get {
                return ResourceManager.GetString("RecipeFieldName_Cooking_Steps_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ingredients.
        /// </summary>
        internal static string RecipeFieldName_Ingredients_DisplayName {
            get {
                return ResourceManager.GetString("RecipeFieldName_Ingredients_DisplayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tag &apos;{0}&apos; with id &apos;{1}&apos; was successfully deleted..
        /// </summary>
        internal static string RecipeTagEntriesController_DeleteTagAsync_Tag_0_with_Id_1_was_successfully_deleted {
            get {
                return ResourceManager.GetString("RecipeTagEntriesController_DeleteTagAsync_Tag_0_with_Id_1_was_successfully_delete" +
                        "d", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No saved tags are found..
        /// </summary>
        internal static string RecipeTagEntriesController_No_saved_tags_are_found {
            get {
                return ResourceManager.GetString("RecipeTagEntriesController_No_saved_tags_are_found", resourceCulture);
            }
        }
    }
}
