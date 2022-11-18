﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RuntimeInspector.UI.ObjectViewer
{
    public class SearchQuery
    {
        public bool IncludeName { get; set; } = false;
        public string Name { get; set; }

        public static SearchQuery Empty => new SearchQuery();

        private SearchQuery()
        {
        }
        
        private SearchQuery( SearchQuery other )
        {
            this.IncludeName = other.IncludeName;
            this.Name = other.Name;
        }

        public SearchQuery WithName( string name )
        {
            return new SearchQuery( this )
            {
                IncludeName = true,
                Name = name
            };
        }

        /// <summary>
        /// Checks whether or not a specific object matches the criteria defined by this query.
        /// </summary>
        /// <param name="obj">The object to match.</param>
        /// <returns>True if the object matches all the criteria, otherwise false.</returns>
        public bool Matches( Object obj )
        {
            if( IncludeName )
            {
                if( !obj.name.Contains( Name ) )
                {
                    return false;
                }
            }

            return true;
        }
    }
}