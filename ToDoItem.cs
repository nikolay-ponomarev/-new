﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public enum ToDoItemState
    {
        Active,
        Completed
    }

    public class ToDoItem
    {
        public Guid Id { get; set; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }

        public ToDoItem(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            User = user;
            Name = name;
            CreatedAt = DateTime.Now;
            State = ToDoItemState.Active;
            StateChangedAt = null;
        }
    }
}
