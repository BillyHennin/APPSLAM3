﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from MANAGER INC. team.
//  
// Copyrights (c) 2014 MANAGER INC. All rights reserved.

namespace MANAGER.Classes
{
    public class Category
    {
        public Category(int ID, string Description)
        {
            this.ID = ID;
            this.Description = Description;
        }

        public string Description { get; set; }
        public int ID { get; set; }
    }
}